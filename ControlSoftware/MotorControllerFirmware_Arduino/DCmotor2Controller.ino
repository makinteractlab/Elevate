const int hallSensor2 = A7;

bool M2_numChecker = true;
int M2_val = LOW;
int M2_hallVal = 0;
int M2_stepCounter;
bool M2_isPosition;
bool M2_raisingChecker = false;
bool M2_isPlay = false;
int M2_magnetState = 0;
int M2_playChecker = 3;

void M2_moveDown(){
  digitalWrite(M2APin, HIGH);
  digitalWrite(M2BPin, LOW);
}

void M2_moveUp(){
  digitalWrite(M2APin, LOW);
  digitalWrite(M2BPin, HIGH);
}

void M2_moveStop(){
  digitalWrite(M2APin, LOW);
  digitalWrite(M2BPin, LOW);
}

void M2_moveHome(){
  if(M2_val == LOW)M2_moveDown();
  if(M2_val == HIGH)M2_moveStop();
}

void M2_magnetOn(){
    digitalWrite(MAG2Pin, HIGH);
}

void M2_magnetOff(){
    digitalWrite(MAG2Pin, LOW);
}

bool M2_hallChecker(){
  int M2_hallVal = M2_hallValue();
  //minimun value is 350 for id 4, 10 else 400
  if(M2_hallVal <= 350) return true; //Serial.println("oddNum");
  if(M2_hallVal >= 620) return false; //Serial.println("evenNum");
  if(M2_stepCounter % 2 == 1) return true;
  return false;
}

void M2_homeChecker(){
  M2_val =  digitalRead(S2Pin);

  if(M2_val == HIGH){
    //Serial.println("home");
    M2_stepCounter = 1;
    M2_isPosition = true;
    M2_raisingChecker = true;
  }
}

int M2_hallValue(){
    M2_hallVal = analogRead(hallSensor2);
    return M2_hallVal;
}

void M2_stepMove(int M2_stepNum){
  M2_homeChecker();
  if(!M2_raisingChecker){
     M2_moveHome();
  }
  else{
     M2_moveUp();
    bool M2_hallVal = M2_hallChecker();
    if(M2_isPosition != M2_hallVal){
       M2_stepCounter++;
       M2_isPosition = M2_hallVal;
       //Serial.println(M2_stepCounter);
    }
    if(M2_stepCounter == M2_stepNum){
       M2_raisingChecker = false;
    }
  }
}

// when we give the input, we have to change playChecker = 0;
void M2_play(int M2_stepNum, int M2_previousNum){
  if(M2_stepNum == M2_previousNum){
    M2_playChecker = 3;
  }
  else if(M2_stepNum < M2_previousNum){
    if(M2_playChecker < 3 && M2_stepNum > 1) {
      if(M2_magnetState == 1) M2_stepMove(M2_previousNum + 1);
      else M2_stepMove(M2_stepNum);
      if(M2_isPlay != M2_raisingChecker){
        M2_playChecker++;
        if(M2_playChecker == 1 && M2_magnetState == 0){
          M2_magnetState ++;
          M2_magnetOn();
        }
        if(M2_playChecker == 3 && M2_magnetState == 1){
          M2_playChecker = 1;
          M2_magnetState++;
          M2_magnetOff();
        }
        M2_isPlay = M2_raisingChecker;
      }
    }
    if(M2_playChecker < 3 && M2_stepNum == 1) {
      M2_stepMove(M2_previousNum + 1);
      if(M2_isPlay != M2_raisingChecker){
        M2_playChecker++;
        if(M2_playChecker == 1 && M2_magnetState == 0){
          M2_magnetState ++;
          M2_magnetOn();
        }
        if(M2_playChecker == 3 && M2_magnetState == 1){
          M2_magnetState++;
          M2_magnetOff();
        }
        M2_isPlay = M2_raisingChecker;
      }
    }
    else M2_moveStop();
  }
  else{
    if(M2_playChecker < 3 && M2_stepNum > 1) {
      M2_stepMove(M2_stepNum);
      if(M2_isPlay != M2_raisingChecker){
        M2_playChecker++;
        M2_isPlay = M2_raisingChecker;
      }
    }
    else if(M2_playChecker < 3 && M2_stepNum == 1) {
      M2_playChecker = 3;
    }
    else M2_moveStop();
  }
}

void M2_reset(){
  M2_magnetState = 0;
  M2_playChecker = 0;
  M2_isPlay = false;
  M2_raisingChecker = false;
}

int M2_playCheck(){
  return M2_playChecker;
}
