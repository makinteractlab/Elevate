const int hallSensor1 = A6;

int M1_switch = LOW; // swith clicked: HIGH, not clikced: LOW
int M1_hallVal = 0; // Value of hallSensor
int M1_stepCounter; // Counting of pin moving (minimum: 1)
bool M1_polarity; // Current Magnet's polarity (first magnet is True)
bool M1_raisingChecker = false; // Check the signal of raising
bool M1_raisingState = false; // Current state of raising
int M1_magnetState = 0; // State of magnet 
int M1_playChecker = 3; // State of playing 0: start, 1:moveUp, 2: moveDown, 3:Done
int M1_errorCounter = 0; // Counting error moment;

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
  if(M1_switch == LOW) M1_moveDown();
  else M1_moveStop();
}

void M1_magnetOn(){
  digitalWrite(MAG1Pin, HIGH);
}

void M1_magnetOff(){
  digitalWrite(MAG1Pin, LOW);
}

int M1_hallValue(){
  return analogRead(hallSensor1);
}

bool M1_hallPolarity(){
  M1_hallVal = M1_hallValue();
  if(M1_hallVal <= 400) return true; //if "id" is 4 or 10, minimun value is 350, else 400
  if(M1_hallVal >= 620) return false;
  if(M1_stepCounter % 2 == 1) return true;
  return false;
}

void M1_stepMove(int M1_stepNum){
  M1_switch =  digitalRead(S1Pin);
  if(M1_switch == HIGH){
    M1_stepCounter = 1;
    M1_polarity = true;
    M1_raisingChecker = true;
  }
  
  if(!M1_raisingChecker){
     M1_moveHome();
  }
  else{
    M1_moveUp();
    if(M1_polarity != M1_hallPolarity()){
       M1_stepCounter++;
       M1_polarity = M1_hallPolarity();
    }
    if(M1_stepCounter == M1_stepNum){
       M1_raisingChecker = false;
    }
  }
}

void M1_play(int M1_stepNum, int M1_previousNum){
  if(M1_stepNum == M1_previousNum){
    M1_playChecker = 3;
  }
  else if(M1_stepNum > M1_previousNum){
    if(M1_playChecker < 3){
      if(M1_stepNum == 1) M1_playChecker = 3;
      else{
        M1_stepMove(M1_stepNum);
        M1_errorCounter++;
        if(M1_raisingState != M1_raisingChecker){
          M1_playChecker++;
          M1_raisingState = M1_raisingChecker;
        }
        if(M1_playChecker == 1 && M1_errorCounter > 32000){
          M1_playChecker++;
          M1_raisingChecker = false;
          M1_raisingState = false;     
        }     
      }
    }
    else M1_moveHome();
  }
  else{
    if(M1_playChecker < 3){
      if(M1_stepNum == 1){
        M1_stepMove(M1_previousNum + 1);
        if(M1_raisingState != M1_raisingChecker){
          M1_playChecker++;
          if(M1_playChecker == 1 && M1_magnetState == 0){
            M1_magnetState ++;
            M1_magnetOn();
          }
          if(M1_playChecker == 3 && M1_magnetState == 1){
            M1_magnetState++;
            M1_magnetOff();
          }
          M1_raisingState = M1_raisingChecker;
        }
      }         
      else{
        if(M1_magnetState == 1) M1_stepMove(M1_previousNum + 1);
        else M1_stepMove(M1_stepNum);

        if(M1_raisingState != M1_raisingChecker){
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
          M1_raisingState = M1_raisingChecker;
        }
      }
    }
    else M1_moveHome();
  }
}

void M1_reset(){
  M1_magnetState = 0;
  M1_playChecker = 0;
  M1_raisingState = false;
  M1_raisingChecker = false;
  M1_errorCounter = 0;
}

int M1_playCheck(){
  return M1_playChecker;
}
