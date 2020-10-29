#include<ArduinoJson.h>

String buff = "";
const byte machine_id = 9;
const int ledpin = 12;
const int M1APin = 5;
const int M1BPin = 4;
const int M2APin = 3;
const int M2BPin = 2;
const int MAG1Pin = 7;
const int MAG2Pin = 6;
const int S1Pin = 8;
const int S2Pin = 9;

const size_t capacity = JSON_OBJECT_SIZE(4) + JSON_ARRAY_SIZE(4) + 7;
StaticJsonDocument<capacity> jb;

int M1Height = 1;
int M2Height = 1;
int P1Height = 1;
int P2Height = 1;

void setup() {
  Serial.begin(115200);
  pinMode(ledpin, OUTPUT);
  pinMode(M1APin, OUTPUT);
  pinMode(M1BPin, OUTPUT);
  pinMode(M2APin, OUTPUT);
  pinMode(M2BPin, OUTPUT);
  pinMode(MAG1Pin, OUTPUT);
  pinMode(MAG2Pin, OUTPUT);
  pinMode(S1Pin, INPUT);
  pinMode(S2Pin, INPUT);
}

void loop() {
  if(M1_playCheck() == 3 && M2_playCheck() == 3){
    while(Serial.available()){
      char inChar =(char)Serial.read();
      if(inChar == '\n'){

        DeserializationError err = deserializeJson(jb, buff);
         if (err) {
          Serial.print(F("deserializeJson() failed: "));
          Serial.println(err.c_str());
        }

        if(jb["i"] == machine_id){
          if(jb["c"] == "led"){
            if(jb["d"] == 1){
              digitalWrite(ledpin, HIGH);
            }
            else if(jb["d"] == 0){
              digitalWrite(ledpin, LOW);
            }
          }
          if(jb["c"] == "motor"){
            M1Height = (int)jb["d"][0];
            M2Height = (int)jb["d"][1];
            P1Height = (int)jb["d"][2];
            P2Height = (int)jb["d"][3];
            M1_reset();
            M2_reset();
          }
        }
        buff = "";
      }
      else{
        buff += inChar;
      }
    }
  }
    M1_play(M1Height, P1Height);
    M2_play(M2Height, P2Height);
}
