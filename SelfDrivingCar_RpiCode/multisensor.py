import RPi.GPIO as GPIO
import time
from VehicleControl import *


def getSensorData(sensors, timeSpan=0):
    #Initializes empy array to hold all distances from all sensors provided as function parameter.
    sensorData = []
    delay = timeSpan / (len(sensors) - 1)

    for sensor in sensors:
        if delay != 0:
            time.sleep(delay)
        
        pulse_start = pulse_end = time.time()
        
        #Sends the high frequency sound wave:
        GPIO.output(sensor["TRIG"], False)
        time.sleep(0.01)
        GPIO.output(sensor["TRIG"], True)
        time.sleep(0.00001)
        GPIO.output(sensor["TRIG"], False)

        while 0 == GPIO.input(sensor["ECHO"]) and time.time() - pulse_start < 0.15:
            continue
        #Records strart time of the pulse.
        pulse_start = time.time()

        #Recieves sound wave:
        while GPIO.input(sensor["ECHO"]) == 1 and time.time() - pulse_start < 0.15:
            continue
        #Records end time of the pulse.
        pulse_end = time.time()

        pulse_duration = pulse_end - pulse_start

        distance = pulse_duration * (343.21 / 2.0) #~=171.5, calculates the distance to the object based on the speed of sound.

        sensorData.append(distance)#Saves calculated distance in the local array.
    #Returns distances to the objects.
    return sensorData


def setupSensors(sensors):
    #Sets up TRIGER PIN of Raspberry pi to be output pin, and ECHO pin to be input.
    GPIO.setmode(GPIO.BCM)
    for sensor in sensors:
        GPIO.setup(sensor["TRIG"], GPIO.OUT)
        GPIO.setup(sensor["ECHO"], GPIO.IN)
        
        
def clearSensors():
    #Resets the GPIO pins of raspberry pi.
    GPIO.cleanup()
    
    
if __name__ == "__main__":
    #Little script to test all sensors.
    maxSensorLen = 1.0
    try:
        sensors = [{"TRIG": 14, "ECHO": 15}, {"TRIG": 23, "ECHO": 24}, {"TRIG": 8, "ECHO": 7}, {"TRIG": 20, "ECHO": 21}, {"TRIG": 19, "ECHO": 26}, {"TRIG": 5, "ECHO": 6}]
        setupSensors(sensors)
        
        while True:
            carSensors = getSensorData(sensors)
            carSensors = list(map(lambda x: numMap(max(min(maxSensorLen, x), 0.0), 0.0, maxSensorLen, -1.0, 1.0), carSensors))
            print(carSensors)
            time.sleep(2)
            
    finally:  
        clearSensors()
    

##sensors = [{"TRIG": 23, "ECHO": 24}, {"TRIG": 25, "ECHO": 8}]
##
##while True:
##    for sensorData in getSensorData(sensors):
##        print("Distance is " + str(sensorData) + " m.")

