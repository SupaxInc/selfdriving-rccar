import smbus, time
from decimal import Decimal


def numMap(value, fromLow, fromHigh, toLow, toHigh):
    return (toHigh - toLow) * (value - fromLow) / (fromHigh - fromLow) + toLow


class VehicleControl:
    '''This class is provided by Freenove, but augmented by the project team.'''
    CMD_SERVO1 = 0
    CMD_SERVO2 = 1
    CMD_SERVO3 = 2
    CMD_SERVO4 = 3
    CMD_PWM1 = 4
    CMD_PWM2 = 5
    CMD_DIR1 = 6
    CMD_DIR2 = 7
    CMD_BUZZER = 8
    CMD_IO1 = 9
    CMD_IO2 = 10
    CMD_IO3 = 11
    CMD_SONIC = 12
    SERVO_MAX_PULSE_WIDTH = 2500
    SERVO_MIN_PULSE_WIDTH = 500
    Is_IO1_State_True = False
    Is_IO2_State_True = False
    Is_IO3_State_True = False
    Is_Buzzer_State_True = False

    def __init__(self, addr=0x18):
        self.address = addr  # default address of motor controller
        self.bus = smbus.SMBus(1)
        self.bus.open(1)
        self.currentSpeed = 0
        self.currentAngle = 0

    def i2cRead(self, reg):
        self.bus.read_byte_data(self.address, reg)

    def i2cWrite1(self, cmd, value):
        self.bus.write_byte_data(self.address, cmd, value)

    def i2cWrite2(self, value):
        self.bus.write_byte(self.address, value)

    def writeReg(self, cmd, value):
        try:
            self.bus.write_i2c_block_data(self.address, cmd, [value >> 8, value & 0xff])
            time.sleep(0.001)
        except Exception as e:
            print(Exception, "I2C Error :", e)

    def readReg(self, cmd):
        [a, b] = self.bus.read_i2c_block_data(self.address, cmd, 2)
        return a << 8 | b

    def getSonicEchoTime(self):
        SonicEchoTime = self.readReg(VehicleControl.CMD_SONIC)
        return SonicEchoTime

    def getSonic(self):
        SonicEchoTime = self.readReg(VehicleControl.CMD_SONIC)
        distance = (SonicEchoTime * 343.202) / (2.0 * 1000000.0)
        return distance
    
    def getSonicNormalized(self):
        distance = self.getSonic()
        distance = distance if distance < 1.0 else 1.0
        return distance        

    def setShieldI2cAddress(self, addr):  # addr: 7bit I2C Device Address
        if (addr < 0x03) or (addr > 0x77):
            return
        else:
            self.writeReg(0xaa, (0xbb << 8) | (addr << 1))
            
    def turnByAngle(self, angle):
        angle = numMap(angle, 0, 180, 1.0, -1.0)
        self.turnByFloat(angle)
        
    def turnByFloat(self, angle):
        #Turns the car's shaft by indicated percent (-1.0 <-; 1.0 ->)
        if angle > 1.0:
            angle = 1.0
        elif angle < -1.0:
            angle = -1.0
        self.currentAngle = angle
        angle *= 0.75
        angle = int(numMap(angle, 1.0, -1.0, VehicleControl.SERVO_MIN_PULSE_WIDTH, VehicleControl.SERVO_MAX_PULSE_WIDTH))
        if angle > VehicleControl.SERVO_MAX_PULSE_WIDTH:
            angle = VehicleControl.SERVO_MAX_PULSE_WIDTH
        elif angle < VehicleControl.SERVO_MIN_PULSE_WIDTH:
            angle = VehicleControl.SERVO_MIN_PULSE_WIDTH
        self.writeReg(VehicleControl.CMD_SERVO1, angle)
            
    def turnHeadByAngle(self, num):
        num = int(numMap(num, 0, 180, VehicleControl.SERVO_MIN_PULSE_WIDTH, VehicleControl.SERVO_MAX_PULSE_WIDTH))
        if num > VehicleControl.SERVO_MAX_PULSE_WIDTH:
            num = VehicleControl.SERVO_MAX_PULSE_WIDTH
        elif num < VehicleControl.SERVO_MIN_PULSE_WIDTH:
            num = VehicleControl.SERVO_MIN_PULSE_WIDTH
        self.writeReg(VehicleControl.CMD_SERVO2, num)
        
    def turnHeadByFloat(self, num):
        #Turns the car's front sensor by indicated percent (-1.0 <-; 1.0 ->)
        num = int(numMap(num, 1.0, -1.0, VehicleControl.SERVO_MIN_PULSE_WIDTH, VehicleControl.SERVO_MAX_PULSE_WIDTH))
        if num > VehicleControl.SERVO_MAX_PULSE_WIDTH:
            num = VehicleControl.SERVO_MAX_PULSE_WIDTH
        elif num < VehicleControl.SERVO_MIN_PULSE_WIDTH:
            num = VehicleControl.SERVO_MIN_PULSE_WIDTH
        self.writeReg(VehicleControl.CMD_SERVO2, num)
        
    def buzz(self, sec = 0.1, freq = 2000):
        self.writeReg(VehicleControl.CMD_BUZZER, freq)
        time.sleep(sec)
        self.writeReg(VehicleControl.CMD_BUZZER, 0)
        
    def lightLED(self, red = 0, green = 0, blue = 0):
        #Changes the color of the LED.
        red ^= 1
        green ^= 1
        blue ^= 1
        self.writeReg(VehicleControl.CMD_IO1, red)
        self.writeReg(VehicleControl.CMD_IO2, green)
        self.writeReg(VehicleControl.CMD_IO3, blue)
        
    def lightLEDFromInt(self, num):
        #Changes the color of LED based on 3 bit integer number.
        red = (num >> 2) & 0b1
        green = (num >> 1) & 0b1
        blue = num & 0b1
        self.lightLED(red, green, blue)
        
    def setSpeed(self, newSpeed):
        #Sets the speed of the car from the percent value fro the function parameter.
        if newSpeed > 1.0:
            newSpeed = 1.0
        elif newSpeed < -1.0:
            newSpeed = -1.0
        dir = 1 if newSpeed > 0 else 0
        self.writeReg(VehicleControl.CMD_DIR1, dir)
        self.writeReg(VehicleControl.CMD_DIR2, dir)
        self.writeReg(VehicleControl.CMD_PWM1, int(numMap(abs(newSpeed), 0.0, 1.0, 0, 1000)))
        self.writeReg(VehicleControl.CMD_PWM2, int(numMap(abs(newSpeed), 0.0, 1.0, 0, 1000)))
        self.currentSpeed = newSpeed
        
            
##
##VehicleControl = VehicleControl()
##VehicleControl.turnByFloat(0)

##for i in range(-85, 85, 1):
##    VehicleControl.turnHeadByFloat(i / 100.0)
##    time.sleep(0.005)
##time.sleep(0.5)
##for i in range(85, -85, -1):
##    VehicleControl.turnHeadByFloat(i / 100.0)
##    time.sleep(0.005)
##VehicleControl.turnHeadByFloat(0)
##
##VehicleControl.buzz(0.1, 1000)
##
##for i in range(3):
##    for num in range(8):
##        VehicleControl.lightLEDFromInt(num)
##        time.sleep(0.25)
##VehicleControl.lightLED()
##
##for i in range(100):
##    print("Sonic: {0:.3e}".format(VehicleControl.getSonicNormalized()))
##    time.sleep(0.01)
##
##VehicleControl.setSpeed(0.0)
##VehicleControl.setSpeed(0.5)
##VehicleControl.turnByFloat(0.25)
##time.sleep(1)
##VehicleControl.setSpeed(-0.5)
##time.sleep(1)
##VehicleControl.turnByFloat(-0.25)
##time.sleep(1)
##VehicleControl.turnByFloat(0)
##time.sleep(0.5)
##VehicleControl.setSpeed(0)

##VehicleControl.setSpeed(1)
##VehicleControl.turnByFloat(0)
##time.sleep(4.75/8.0)
##VehicleControl.turnByFloat(0.5)
##time.sleep(4.75*3.0/8.0)
##VehicleControl.turnByFloat(0)
##time.sleep(4.75/4.0)
##VehicleControl.turnByFloat(-0.5)
##time.sleep(4.75*3.0/8.0)
##VehicleControl.turnByFloat(0)
##time.sleep(4.75/8.0)
##VehicleControl.setSpeed(0)

##angle = 90
##VehicleControl.turnByAngle(angle)
##angle = 90
##VehicleControl.turnHeadByAngle(angle)

##num = 0
##VehicleControl.turnByFloat(num)    
##num = 0
##VehicleControl.turnHeadByFloat(num)
    
##for i in range(-100, 100, 1):
##    VehicleControl.turnByFloat(i / 100.0)
##    time.sleep(0.005)
##time.sleep(0.5)
##for i in range(100, -100, -1):
##    VehicleControl.turnByFloat(i / 100.0)
##    time.sleep(0.005)
##VehicleControl.turnByFloat(0)

##VehicleControl.writeReg(VehicleControl.CMD_DIR1, 0)
##VehicleControl.writeReg(VehicleControl.CMD_DIR2, 0)
##for i in range(0, 1000, 10):
##VehicleControl.writeReg(VehicleControl.CMD_PWM1, 0)
##VehicleControl.writeReg(VehicleControl.CMD_PWM2, 0)
##    time.sleep(0.005)
##time.sleep(5)
##for i in range(1000, 0, -10):
##    VehicleControl.writeReg(VehicleControl.CMD_PWM1, i)
##    VehicleControl.writeReg(VehicleControl.CMD_PWM2, i)
##    time.sleep(0.005)
##VehicleControl.writeReg(VehicleControl.CMD_DIR1, 1)
##VehicleControl.writeReg(VehicleControl.CMD_DIR2, 1)
##for i in range(0, 1000, 10):
##    VehicleControl.writeReg(VehicleControl.CMD_PWM1, i)
##    VehicleControl.writeReg(VehicleControl.CMD_PWM2, i)
##    time.sleep(0.005)
##time.sleep(1)
##for i in range(1000, 0, -10):
##    VehicleControl.writeReg(VehicleControl.CMD_PWM1, i)
##    VehicleControl.writeReg(VehicleControl.CMD_PWM2, i)
##    time.sleep(0.005)

##
##
##for i in range(-85, 85, 1):
##    VehicleControl.turnByFloat(i / 100.0)
##    time.sleep(0.005)
##time.sleep(0.5)
##for i in range(85, -85, -1):
##    VehicleControl.turnByFloat(i / 100.0)
##    time.sleep(0.005)
##VehicleControl.turnByFloat(0)
