const int hallSensor1 = A6;

bool M1_numChecker = true;
int M1_val = LOW;
int M1_hallVal = 0;
int M1_stepCounter;
bool M1_isPosition;
bool M1_raisingChecker = false;
bool M1_isPlay = false;
int M1_magnetState = 0;
int M1_playChecker = 3;

void M1_moveDown(){
  digitalWrite(M1APin, LOW);
  digitalWrite(M1BPin, HIGH);
}

void M1_moveUp(){
  digitalWrite(M1APin, HIGH);
  digitalWrite(M1BPin, LOW);
}

void M1_moveStop(){
  digitalWrite(M1APin, LOW);
  digitalWrite(M1BPin, LOW);
}

void M1_moveHome(){
  if(M1_val == LOW)M1_moveDown();
  if(M1_val == HIGH)M1_moveStop();
}

void M1_magnetOn(){
    digitalWrite(MAG1Pin, HIGH);
}

void M1_magnetOff(){
    digitalWrite(MAG1Pin, LOW);
}

bool M1_hallChecker(){
  int M1_hallVal = M1_hallValue();
  //minimun value is 350 for id 4, 10 else 400
  if(M1_hallVal <= 350) return true; //Serial.println("oddNum");
  if(M1_hallVal >= 620) return false; //Serial.println("evenNum");
  if(M1_stepCounter % 2 == 1) return true;
  return false;
}

void M1_homeChecker(){
  M1_val =  digitalRead(S1Pin);

  if(M1_val == HIGH){
    //Serial.println("home");
    M1_stepCounter = 1;
    M1_isPosition = true;
    M1_raisingChecker = true;
  }
}

int M1_hallValue(){
    M1_hallVal = analogRead(hallSensor1);
    return M1_hallVal;
}

void M1_stepMove(int M1_stepNum){
  M1_homeChecker();
  if(!M1_raisingChecker){
     M1_moveHome();
  }
  else{
     M1_moveUp();
    bool M1_hallVal = M1_hallChecker();
    if(M1_isPosition != M1_hallVal){
       M1_stepCounter++;
       M1_isPosition = M1_hallVal;
    }
    if(M1_stepCounter == M1_stepNum){
       M1_raisingChecker = false;
    }
  }
}

// when we give the input, we have to change playChecker = 0;
void M1_play(int M1_stepNum, int M1_previousNum){
  if(M1_stepNum == M1_previousNum){
    M1_playChecker = 3;
  }
  else if(M1_stepNum < M1_previousNum){
    if(M1_playChecker < 3 && M1_stepNum > 1) {
      if(M1_magnetState == 1) M1_stepMove(M1_previousNum + 1);
      else M1_stepMove(M1_stepNum);
      if(M1_isPlay != M1_raisingChecker){
        M1_playChecker++;
        if(M1_playChecker == 1 && M1_magnetState == 0){
          M1_magnetState ++;
          M1_magnetOn();
        }
        if(M1_playChecker == 3 && M1_magnetState == 1){
          M1_playChecker = 1;
          M1_magnetState++;
          M1_magnetOff();
        }
        M1_isPlay = M1_raisingChecker;
      }
    }
    if(M1_playChecker < 3 && M1_stepNum == 1) {
      M1_stepMove(M1_previousNum + 1);
      if(M1_isPlay != M1_raisingChecker){
        M1_playChecker++;
        if(M1_playChecker == 1 && M1_magnetState == 0){
          M1_magnetState ++;
          M1_magnetOn();
        }
        if(M1_playChecker == 3 && M1_magnetState == 1){
          M1_magnetState++;
          M1_magnetOff();
        }
        M1_isPlay = M1_raisingChecker;
      }
    }
    else M1_moveStop();
  }
  else{
    if(M1_playChecker < 3 && M1_stepNum > 1) {
      M1_stepMove(M1_stepNum);
      if(M1_isPlay != M1_raisingChecker){
        M1_playChecker++;
        M1_isPlay = M1_raisingChecker;
      }
    }
    else if(M1_playChecker < 3 && M1_stepNum == 1) {
      M1_playChecker = 3;
    }
    else M1_moveStop();
  }
}

void M1_reset(){
  M1_magnetState = 0;
  M1_playChecker = 0;
  M1_isPlay = false;
  M1_raisingChecker = false;
}

int M1_playCheck(){
  return M1_playChecker;
}
