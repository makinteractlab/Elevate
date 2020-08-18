class PlayMatrix
{
  int heightPerMatrix[][] = new int[20][60];
  boolean checkEmpty = true;
  int count = 0;
  int blocksToGo[] = new int[60];
  int finalRow = 0;

  PlayMatrix(){
    for(int j = 0; j < 60; j ++){
      for(int i = 0; i < 20; i ++){
        heightPerMatrix[i][j] = 0;
      }
      blocksToGo[j] = 0;
    }
  }

  void makeMatrix(){
    for(int j = 0; j < 60; j ++){
      for(int i = 0; i < 20; i ++){
        heightPerMatrix[i][j] = heightPerUnit[i][j];
        if(heightPerMatrix[i][j] != 0){checkEmpty = false;}
      }
      if(checkEmpty){count++;}
      else{
        blocksToGo[j] = count + 1;
        count = 0;
        finalRow = j;
        checkEmpty = true;
      }
    }
  }

  void reset(){
    for(int j = 0; j < 60; j ++){
      for(int i = 0; i < 20; i ++){
        heightPerMatrix[i][j] = 0;
      }
      blocksToGo[j] = 0;
    }
    count = 0;
    finalRow = 0;
  }
}

public void play(){
  playMatrix.makeMatrix();
  thread("serialForPlay");
}

public void home(){
  thread("serialForHome");
}

void serialForPlay(){
  mySerial.write("\n");
  for(int j = 0; j <= playMatrix.finalRow; j++){
    if(playMatrix.blocksToGo[j] != 0){
      if(j != 0){
        String s1 = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[0," + playMatrix.blocksToGo[j] + "]}\n";
        mySerial.write(s1);
        delay(delayTime2  * playMatrix.blocksToGo[j]);
      }
      int maxValue = 0;
      for(int i = 1; i <= 10; i ++){
        int motor1Height = playMatrix.heightPerMatrix[(2 * i) - 2][j] + 1;
        int motor2Height = playMatrix.heightPerMatrix[(2 * i) - 1][j] + 1;
        if(maxValue < motor1Height){maxValue = motor1Height;}
        if(maxValue < motor2Height){maxValue = motor2Height;}
        String s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[" + motor1Height + "," + motor2Height + "]}\n";
        mySerial.write(s2);
      }
      delay(delayTime1 * (maxValue - 1));
    }
  }
  //[1,0] is going to home
  String s = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[1,0]}\n";
  mySerial.write(s);
  playMatrix.reset();
}

void serialForHome(){
  mySerial.write("\n");
  //[1,0] is going to home
  String s = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[1,0]}\n";
  mySerial.write(s);
  playMatrix.reset();
}
