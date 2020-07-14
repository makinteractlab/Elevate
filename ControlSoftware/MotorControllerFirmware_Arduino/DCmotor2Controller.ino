int hallSensor2 = A1;
int switchPin2 = 9;
int motor2APin = 4;
int motor2BPin = 5;

bool two_numChecker = true;
int two_val = LOW;
int two_hallVal = 0;
int two_stepCounter;
bool two_isPosition;
bool two_raisingChecker = false;
bool two_isPlay = false;
int two_playChecker = 3;

void two_moveDown(){
  digitalWrite(motor2APin, HIGH);
  digitalWrite(motor2BPin, LOW);
}

void two_moveUp(){
  digitalWrite(motor2APin, LOW);
  digitalWrite(motor2BPin, HIGH);
}

void two_moveStop(){
  digitalWrite(motor2APin, LOW);
  digitalWrite(motor2BPin, LOW);
}

void two_moveHome(){
  if(two_val == LOW)two_moveDown();
  if(two_val == HIGH)two_moveStop();
}

bool two_hallChecker(){
  int two_hallVal = two_hallValue();
  if(two_hallVal <= 300) return true; //Serial.println("oddNum");
  if(two_hallVal >= 650) return false; //Serial.println("evenNum");
}

void two_homeChecker(){
  two_val =  digitalRead(switchPin2);

  if(two_val == HIGH){
    //Serial.println("home");
    two_stepCounter = 1;
    two_isPosition = true;
    two_raisingChecker = true;
  }
}

int two_hallValue(){
    two_hallVal = analogRead(hallSensor2);
    return two_hallVal;
}

void two_stepMove(int two_stepNum){
  two_homeChecker();
  if(!two_raisingChecker){
     two_moveHome();
  }
  else{
     two_moveUp();
    bool two_hallVal = two_hallChecker();
    if(two_isPosition != two_hallVal){
       two_stepCounter++;
       two_isPosition = two_hallVal;
       //Serial.println(two_stepCounter);
    }
    if(two_stepCounter == two_stepNum){
       two_raisingChecker = false;
    }
  }
}

// when we give the input, we have to change playChecker = 0;
void two_play(int two_stepNum){
  if(two_playChecker < 3 && two_stepNum > 1) {
    two_stepMove(two_stepNum);
    if(two_isPlay != two_raisingChecker){
      two_playChecker++;
      two_isPlay = two_raisingChecker;
    }
  }
  else if(two_playChecker < 3 && two_stepNum == 1) {
    two_playChecker = 3;
    two_moveStop();
  }
  else two_moveStop();
  //Serial.println(two_playChecker);
}

void two_reset(){
  two_playChecker = 0;
  two_isPlay = false;
  two_raisingChecker = false;
}

int two_playCheck(){
  return two_playChecker;
}
