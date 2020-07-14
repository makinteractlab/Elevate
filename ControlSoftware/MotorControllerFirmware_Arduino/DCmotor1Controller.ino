int hallSensor1 = A0;
int switchPin1 = 8;
int motorA1Pin = 3;
int motorA2Pin = 2;

bool one_numChecker = true;
int one_val = LOW;
int one_hallVal = 0;
int one_stepCounter;
bool one_isPosition;
bool one_raisingChecker = false;
bool one_isPlay = false;
int one_playChecker = 3;

void one_moveDown(){
  digitalWrite(motorA1Pin, HIGH);
  digitalWrite(motorA2Pin, LOW);
}

void one_moveUp(){
  digitalWrite(motorA1Pin, LOW);
  digitalWrite(motorA2Pin, HIGH);
}

void one_moveStop(){
  digitalWrite(motorA1Pin, LOW);
  digitalWrite(motorA2Pin, LOW);
}

void one_moveHome(){
  if(one_val == LOW)one_moveDown();
  if(one_val == HIGH)one_moveStop();
}

bool one_hallChecker(){
  int one_hallVal = one_hallValue();
  if(one_hallVal <= 300) return true; //Serial.println("oddNum");
  if(one_hallVal >= 650) return false; //Serial.println("evenNum");
}

void one_homeChecker(){
  one_val =  digitalRead(switchPin1);

  if(one_val == HIGH){
    //Serial.println("home");
    one_stepCounter = 1;
    one_isPosition = true;
    one_raisingChecker = true;
  }
}

int one_hallValue(){
    one_hallVal = analogRead(hallSensor1);
    return one_hallVal;
}

void one_stepMove(int one_stepNum){
  one_homeChecker();
  if(!one_raisingChecker){
     one_moveHome();
  }
  else{
     one_moveUp();
    bool one_hallVal = one_hallChecker();
    if(one_isPosition != one_hallVal){
       one_stepCounter++;
       one_isPosition = one_hallVal;
       //Serial.println(one_stepCounter);
    }
    if(one_stepCounter == one_stepNum){
       one_raisingChecker = false;
    }
  }
}

// when we give the input, we have to change playChecker = 0;
void one_play(int one_stepNum){
  if(one_playChecker < 3 && one_stepNum > 1) {
    one_stepMove(one_stepNum);
    if(one_isPlay != one_raisingChecker){
      one_playChecker++;
      one_isPlay = one_raisingChecker;
    }
  }
  else if(one_playChecker < 3 && one_stepNum == 1) {
    one_playChecker = 3;
    one_moveStop();
  }
  else one_moveStop();
  //Serial.println(one_playChecker);
}

void one_reset(){
  one_playChecker = 0;
  one_isPlay = false;
  one_raisingChecker = false;
}

int one_playCheck(){
  return one_playChecker;
}
