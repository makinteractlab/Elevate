const int hallSensor2 = A7;

int M2_switch = LOW; // swith clicked: HIGH, not clikced: LOW
int M2_hallVal = 0; // Value of hallSensor
int M2_stepCounter; // Counting of pin moving (minimum: 1)
bool M2_polarity; // Current Magnet's polarity (first magnet is True)
bool M2_raisingChecker = false; // Check the signal of raising
bool M2_raisingState = false; // Current state of raising
int M2_magnetState = 0; // State of magnet 
int M2_playChecker = 3; // State of playing 0: start, 1:moveUp, 2: moveDown, 3:Done
int M2_errorCounter = 0; // Counting error moment;

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
  if(M2_switch == LOW) M2_moveDown();
  else M2_moveStop();
}

void M2_magnetOn(){
  digitalWrite(MAG2Pin, HIGH);
}

void M2_magnetOff(){
  digitalWrite(MAG2Pin, LOW);
}

int M2_hallValue(){
  return analogRead(hallSensor2);
}

bool M2_hallPolarity(){
  M2_hallVal = M2_hallValue();
  if(M2_hallVal <= 400) return true; //if "id" is 4 or 10, minimun value is 350, else 400
  if(M2_hallVal >= 620) return false;
  if(M2_stepCounter % 2 == 1) return true;
  return false;
}

void M2_stepMove(int M2_stepNum){
  M2_switch =  digitalRead(S2Pin);
  if(M2_switch == HIGH){
    M2_stepCounter = 1;
    M2_polarity = true;
    M2_raisingChecker = true;
  }
  
  if(!M2_raisingChecker){
     M2_moveHome();
  }
  else{
    M2_moveUp();
    if(M2_polarity != M2_hallPolarity()){
       M2_stepCounter++;
       M2_polarity = M2_hallPolarity();
    }
    if(M2_stepCounter == M2_stepNum){
       M2_raisingChecker = false;
    }
  }
}

void M2_play(int M2_stepNum, int M2_previousNum){
  if(M2_stepNum == M2_previousNum){
    M2_playChecker = 3;
  }
  else if(M2_stepNum > M2_previousNum){
    if(M2_playChecker < 3){
      if(M2_stepNum == 1) M2_playChecker = 3;
      else{
        M2_stepMove(M2_stepNum);
        M2_errorCounter++;
        if(M2_raisingState != M2_raisingChecker){
          M2_playChecker++;
          M2_raisingState = M2_raisingChecker;
        }
        if(M2_playChecker == 1 && M2_errorCounter > 32000){
          M2_playChecker++;
          M2_raisingChecker = false;
          M2_raisingState = false;
        }
      }
    }
    else M2_moveHome();
  }
  else{
    if(M2_playChecker < 3){
      if(M2_stepNum == 1){
        M2_stepMove(M2_previousNum + 1);
        if(M2_raisingState != M2_raisingChecker){
          M2_playChecker++;
          if(M2_playChecker == 1 && M2_magnetState == 0){
            M2_magnetState ++;
            M2_magnetOn();
          }
          if(M2_playChecker == 3 && M2_magnetState == 1){
            M2_magnetState++;
            M2_magnetOff();
          }
          M2_raisingState = M2_raisingChecker;
        }
      }         
      else{
        if(M2_magnetState == 1) M2_stepMove(M2_previousNum + 1);
        else M2_stepMove(M2_stepNum);

        if(M2_raisingState != M2_raisingChecker){
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
          M2_raisingState = M2_raisingChecker;
        }
      }
    }
    else M2_moveHome();
  }
}

void M2_reset(){
  M2_magnetState = 0;
  M2_playChecker = 0;
  M2_raisingState = false;
  M2_raisingChecker = false;
  M2_errorCounter = 0;
}

int M2_playCheck(){
  return M2_playChecker;
}
