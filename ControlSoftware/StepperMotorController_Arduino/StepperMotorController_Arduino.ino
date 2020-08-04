#include<ArduinoJson.h>

#define motorLCW 6
#define motorLCCW 7
#define motorRCW 4
#define motorRCCW 5
#define stepSwitch 3

String buff = "";
const byte machine_id = 11;

const size_t capacity = JSON_OBJECT_SIZE(4) + JSON_ARRAY_SIZE(2) + 7;
StaticJsonDocument<capacity> jb;

// resolution : 5000 step = 360 degree, 1 step = 0.072 degree
int resolution = 5000;
int stepSwitchState = 0;
int motorSpeed = 200;

bool homeChecker = false;

void setup() {
  Serial.begin(115200);
  pinMode(motorLCW, OUTPUT);
  pinMode(motorLCCW, OUTPUT);
  pinMode(motorRCW, OUTPUT);
  pinMode(motorRCCW, OUTPUT);
  pinMode(stepSwitch, INPUT);
}

void loop() {
  while(Serial.available()){
    char inChar =(char)Serial.read();
    if(inChar == '\n'){

      DeserializationError err = deserializeJson(jb, buff);
      if (err) {
       Serial.print(F("deserializeJson() failed: "));
       Serial.println(err.c_str());
      }
      if(jb["i"] == machine_id){
        if(jb["c"] == "stepper"){
          if(jb["d"][0] == 0){
            for(int i = 0; i < (int)jb["d"][1]; i ++){
              moveForwardOneBlock();
            }
          }
          else if(jb["d"][0] == 1){
            if(jb["d"][1] == 0){
              moveHome();
            }
            else{
              for(int i = 0; i < (int)jb["d"][1]; i ++){
                moveBackwardOneBlock();
              }
            }
          }
        }
      }
      buff = "";
    }
    else{
      buff += inChar;
    }
  }
}

void moveForward(int motorStep){
  digitalWrite(motorLCCW, LOW);
  digitalWrite(motorRCW, LOW);
  for(int i = 0; i < motorStep; i++){
    digitalWrite(motorLCW, HIGH);
    digitalWrite(motorRCCW, HIGH);
    delayMicroseconds(motorSpeed);
    digitalWrite(motorLCW, LOW);
    digitalWrite(motorRCCW, LOW);
    delayMicroseconds(motorSpeed);
  }
}

void moveBackward(int motorStep){
  digitalWrite(motorLCW, LOW);
  digitalWrite(motorRCCW, LOW);
  for(int i = 0; i < motorStep; i++){
    digitalWrite(motorLCCW, HIGH);
    digitalWrite(motorRCW, HIGH);
    delayMicroseconds(motorSpeed);
    digitalWrite(motorLCCW, LOW);
    digitalWrite(motorRCW, LOW);
    delayMicroseconds(motorSpeed);
  }
}

void moveForwardOneBlock(){
  moveForward(2090);
}

void moveBackwardOneBlock(){
  moveBackward(2090);
}

void moveHome(){
  stepSwitchState = digitalRead(stepSwitch);
  while(stepSwitchState != HIGH){
    moveBackward(1);
    stepSwitchState = digitalRead(stepSwitch);
  }
  digitalWrite(motorLCW, LOW);
  digitalWrite(motorRCW, LOW);
  digitalWrite(motorLCCW, LOW);
  digitalWrite(motorRCCW, LOW);
  homeChecker = true;
  
}
