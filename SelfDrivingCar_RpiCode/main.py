#!/usr/bin/python3
from VehicleControl import *
from ml import *
from multisensor import *
import numpy
import threading
import time
import copy
import bluetooth
import sys
import struct
import os
import logging
import subprocess

#The main script of Raspberry Pi, which controlls the car
def main():
    logFormatter = logging.Formatter("%(asctime)s [%(threadName)-15.15s] [%(levelname)-5.5s]  %(message)s") #Sets the format of the log file.
    rootLogger = logging.getLogger()#Stores root logger to log all events.

    fileHandler = logging.FileHandler('/home/pi/Desktop/project/selfdrivingrpi/sdrpi.log')#Indicates the file to store the log.
    fileHandler.setFormatter(logFormatter)#Sets the format to root logger.
    rootLogger.addHandler(fileHandler)#sets the file handler to root logger.

    consoleHandler = logging.StreamHandler()#Adds console log stream to root logger. Allows to catch meseges from console.
    consoleHandler.setFormatter(logFormatter)#Adds the same format to console log.
    rootLogger.addHandler(consoleHandler)#Adds console handler to root logger.
    rootLogger.setLevel(level=logging.INFO)#Sets the level of awerness of the logger to be able to catch all messages.
    
    rootLogger.info('--<Running Main>--')
    
    car = VehicleControl()#Initializes the car object
    #Resets the car
    car.lightLED()
    car.setSpeed(0)
    car.turnByFloat(0)

    try:
        #Copies the latest neural network from gogole drive to the local storage
        cmdOut = subprocess.check_output(['rclone copy gdrive:selfdrivingrpi/bestBrain.txt /home/pi/Desktop/project/selfdrivingrpi/'], shell=True, timeout=5)
        car.lightLED(1,1,0)#Post code yellow to indicate successful loading from google drive
        rootLogger.info('-<Loaded file from GDrive>-')
        time.sleep(1)
        
    except (subprocess.CalledProcessError, subprocess.TimeoutExpired) as e:
        rootLogger.error(e)
        car.lightLED(1,0,1)#Post code purple if loading failed.
        time.sleep(1)
        
    try:
        #loads locally stored neural network to be used in the scipt.
        bestBrainFile = open('/home/pi/Desktop/project/selfdrivingrpi/bestBrain.txt', 'r')
        genome = bestBrainFile.readline()
        car.lightLED(0,1,1)#Post code Light-Blue to indicate successful loading of the local file.
        bestBrainFile.close()
        rootLogger.info('-<Opened local genome>-')
        #Formats the content of the file to used as the structure of the neural network and as weights(DNA).
        genome = genome.split(';')
        structure = genome[0]
        dna = genome[1]
        structure = list(map(lambda x: int(x), structure.split()))
        dna = list(map(lambda x: float(x), dna.split()))
        time.sleep(1)

    except Exception as e:
        rootLogger.error(e)
        car.lightLED(1,0,0)#Post code Red indicating failure in loading the local file
        quit()#Program ends.
    
    rootLogger.info('Loading genome: ' + str(genome))
    isManual = True #Initial mode is Manual
    toWork = True #Boolean variable to be used in infinite loop
    offset = 0.12#The shaft of the car is tilted to the left, offset is used to keep the car straight
    maxAISpeed = 0.6#AI has a speed limit of 60% for demostrtion purposes.
    sensorOffset = -0.15#AI has a sensor offset for faster respons for demonstration purposes
    maxSensorLen = 1.0#Maximum lenght of sensor readings, mimics the sensor lenght from simulation application.
    car.turnHeadByFloat(offset)#Resets the car front sensor to point forwars.
    brain = Species([lambda x: numpy.tanh(x), lambda x: x * (1 - x)], structure, dna)#Initializes neural network species from obtained data.
    #The pins are besed on GPIO number.
    sensors = [{"TRIG": 14, "ECHO": 15}, #Front sensor
               {"TRIG": 23, "ECHO": 24}, #FrontRight sensor
               {"TRIG":  5, "ECHO":  6}, #FrontLeft sensor
               {"TRIG": 20, "ECHO": 21}, #Back sensor
               {"TRIG":  8, "ECHO":  7}, #BackRight sensor
               {"TRIG": 19, "ECHO": 26}] #BackLeft sensor
    
    try:
        clearSensors()#Clears GPIO pins if the are assigned.
    except Exception as e:
        rootLogger.warning(e)
    setupSensors(sensors)#Sets up sensor PINs
    
    try:
        #Main loop:
        while toWork:
            server_socket = bluetooth.BluetoothSocket(bluetooth.RFCOMM)#Initialized bluetooth server with RFCOMM protocol.
            port = 1
            server_socket.bind(("", port))
            server_socket.listen(1)

            rootLogger.info('-<Waiting for new connection>-')
            client_socket, address = server_socket.accept()#Connect to the client device.
            rootLogger.info("Accepted connection from " + str(address))
            try:
                #While connected to device:
                while toWork:
                    data = client_socket.recv(1024)#Reads the data fromthe client
                    data = data.decode("utf-8")#Decodes the data using UTF-8
                    rootLogger.info('Bluetooth input: ' + data)
                    #Formats the data to be used byscript.
                    data = data.split()
                    
                    if 'MANUAL' in data:
                        #If the client sent MANUAL, changes the mode to be Manual and the LED is set to be BLUE
                        isManual = True
                        car.lightLED(0,0,1)
                    elif 'AUTOMATIC' in data:
                        #If the AUTOMATIC is recieved, sets the mode to be Auto, changes the LED to be GREEN.
                        isManual = False
                        car.lightLED(0,1,0)
                        
                        carSensors = getSensorData(sensors)#Gets the data from sensors as array of distances to obsticals.
                        carSensors = list(map(lambda x: numMap(max(min(maxSensorLen, x + sensorOffset), 0.0), 0.0, maxSensorLen, -1.0, 1.0), carSensors))#Formats the sensor data to be in range from -1.0 to 1.0 (as in simulation)
                        inputData = copy.deepcopy(carSensors)
                        inputData.append(car.currentSpeed)#Adds the speed of the car to the input array.
                        
                        outputData = brain.guess(inputData)#Neural Network determines the optimal speed and direction not to crash.
                        
                        rootLogger.info('NN Input: ' + str(inputData))
                        rootLogger.info('NN Output: ' + str(outputData))
                        #Updates speed and direction towards values predicted by neural network.
                        car.setSpeed(float(outputData[0]) * maxAISpeed)
                        car.turnByFloat(float(outputData[1]) + offset)
                        time.sleep(1.0/60.0)#Waites for time equivalelt to frame rate of simulation 
                        
                    elif isManual:
                        #if the mode is manual, obtains the data from the bluetooth connection and formats is to be used in s\the script
                        data = list(map(lambda x: x.split(':'), data))
                        logging.info('Bluetooth data: ' + str(data))
                        #The data array consists of at least 2 elements:speed and direction indicated by the user.
                        mode, magnitude = data[0]
                        if mode == 'Horizontal':
                            #Changes the direction of the car according to user command.
                            car.turnByFloat(float(magnitude) + offset)
                            rootLogger.info("Angle -<{0}>-".format(car.currentAngle))
                        elif mode == 'Vertical':
                            #Changes the speed of the car according to the user command.
                            car.setSpeed(float(magnitude))
                            rootLogger.info("Speed -<{0}>-".format(car.currentSpeed))
                        mode, magnitude = data[1]
                        if mode == 'Horizontal':
                            car.turnByFloat(float(magnitude) + offset)
                            rootLogger.info("Angle -<{0}>-".format(car.currentAngle))
                        elif mode == 'Vertical':
                            car.setSpeed(float(magnitude))
                            rootLogger.info("Speed -<{0}>-".format(car.currentSpeed))

            except IOError as e:
                rootLogger.error(e)

            finally:
                #When the user drops the connection, changes mode to be manual and waits for new connection. LED => Bright-Blue
                client_socket.close()
                server_socket.close()
                isManual = True
                car.lightLED(0,1,1)
                rootLogger.info("-<Cleaning up bluetooth connection>-")
    
    except KeyboardInterrupt as e:
        toWork = False
        
    except Exception as e:
        rootLogger.error(e)
        
    finally:
        #Performs final cleanup.
        toWork = False
        clearSensors()#Clears GPIO of the Rasbperry Pi
        #Stops the car.
        car.setSpeed(0)
        car.turnByFloat(0)
        car.lightLED()
        rootLogger.info('-<AI is done>-')
        rootLogger.info('-<Bluetooth is done>-')
        
if __name__ == "__main__":
    # execute only if run as a script
    main()