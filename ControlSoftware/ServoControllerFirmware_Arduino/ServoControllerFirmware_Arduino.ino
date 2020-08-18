#include<Wire.h>
#include<Adafruit_PWMServoDriver.h>

Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver();
Adafruit_PWMServoDriver pwm2 = Adafruit_PWMServoDriver(0x41);

#define SERVOMIN 110
#define SERVOMAX 620

uint8_t servonum = 0;

void setup() {
  Serial.begin(9600);

  pwm.begin();
  pwm2.begin();

  pwm.setPWMFreq(60); // run ~60Hz
  pwm2.setPWMFreq(60);

  //yield();

}

void loop() {
  for(int i = 0; i < 16; i++){
    pwm.setPWM(i,0,pulse(90));
    pwm2.setPWM(i,0,pulse(90));
    delay(1000);
    pwm.setPWM(i,0,pulse(130));
    pwm2.setPWM(i,0,pulse(50));
    delay(1000);
  }
}

int pulse(int angle){
  return map(angle, 0, 180, SERVOMIN, SERVOMAX);
}
