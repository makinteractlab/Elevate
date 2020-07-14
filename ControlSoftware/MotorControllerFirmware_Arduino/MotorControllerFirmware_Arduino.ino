#include<ArduinoJson.h>

String buff = "";
const byte machine_id = 12;
const int ledpin = 12;

const size_t capacity = JSON_OBJECT_SIZE(4) + JSON_ARRAY_SIZE(2) + 7;
StaticJsonDocument<capacity> jb;

int oneHeight = 5;
int twoHeight = 5;

void setup() {
  Serial.begin(115200);
  pinMode(ledpin, OUTPUT);
}

void loop() {
  if(one_playCheck() == 3 && two_playCheck() == 3){
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
            oneHeight = (int)jb["d"][0];
            twoHeight = (int)jb["d"][1];
            one_reset();
            two_reset();
          }
        }
        buff = "";
      }
      else{
        buff += inChar;
      }
    }
  }
    one_play(oneHeight);
    two_play(twoHeight);
}
