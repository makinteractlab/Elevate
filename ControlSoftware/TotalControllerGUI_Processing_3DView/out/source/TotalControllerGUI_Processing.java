import processing.core.*; 
import processing.data.*; 
import processing.event.*; 
import processing.opengl.*; 

import controlP5.*; 
import processing.serial.*; 

import java.util.HashMap; 
import java.util.ArrayList; 
import java.io.File; 
import java.io.BufferedReader; 
import java.io.PrintWriter; 
import java.io.InputStream; 
import java.io.OutputStream; 
import java.io.IOException; 

public class TotalControllerGUI_Processing extends PApplet {




PFont basicFont;
ControlP5 cp5;
Serial mySerial;

MainView mainView;
Palette palette;
Brush brush;
PlayMatrix playMatrix;
View3D view3D;
int heightPerUnit[][] = new int[20][60];
int colorByheight[] = new int[11];
int myColor;
int brushSize;
int delayTime1 = 300;
int delayTime2 = 950;
JSONObject savedJSON;
JSONObject loadedJSON;
boolean loaded;
boolean loadSuccess;

public void setup(){
  
  background(0,0,50);
  
  basicFont = createFont("ProductSans-Light.ttf", 20);
  cp5 = new ControlP5(this);
  cp5.addButton("play").setPosition(1130, 620).setSize(100,50);
  cp5.addButton("home").setPosition(980, 620).setSize(100,50);
  cp5.addButton("saveJSON").setPosition(830, 620).setSize(100,50);
  cp5.addButton("loadJSON").setPosition(680, 620).setSize(100,50);
  cp5.addButton("clean").setPosition(50, 620).setSize(100,50);  
  String myPort = Serial.list()[0];
  mySerial = new Serial(this, myPort, 115200);
 
  for(int j = 0; j < 60; j ++){
    for(int i = 0; i < 20; i ++){ heightPerUnit[i][j] = 0; }
  }

  for(int i = 0; i < 11; i ++) {
    colorByheight[i] = color(255 - i * 21, 255 - i * 21, 255- i * 5);
  }

  mainView = new MainView();
  palette = new Palette();
  brush = new Brush();
  playMatrix = new PlayMatrix();
  view3D = new View3D();
  myColor = 0;
  brushSize = 10;
  loaded = false;
  loadSuccess = false;
}

public void draw(){
  background(0,0,50);
  mainView.draw();
  mainView.changeColor(mouseX, mouseY, brushSize);
  palette.draw();
  brush.draw();
  brush.drawBrush();
  view3D.draw();
}

public void mousePressed(){
  myColor = palette.selectColor(mouseX,mouseY);
  mainView.drawOn(mouseX, mouseY);
}

public void mouseReleased(){
  mainView.drawOff();
}

public void keyPressed(){
  if(key == '+'){
    if(brushSize < 99) brushSize++;
  }
  if(key == '-'){
    if(brushSize > 1) brushSize--;
  }
}

class MainView
{ 
  boolean drawing = false;

  public void draw(){
    for(int j = 0; j < 60; j ++){
      for(int i = 0; i < 20; i ++){
        fill(colorByheight[heightPerUnit[i][j]]);
        strokeWeight(1);
        stroke(0);
        rect(300 + i * 10, 650 - j * 10, 10, 10);
      }
    }
  }
  
  public void drawOn(int mx, int my){
    if(mx >= 300 && mx <= 500){
      if(my >= 60 && my <= 660){ drawing = true; }
    }
  }

  public void drawOff(){
    drawing = false;
  }

  public void changeColor(int mx, int my, int size){
    if(drawing){
      for(int j = 0; j < 60; j ++){
        for(int i = 0; i < 20; i ++){
          float distance = sqrt(pow(305 + (i * 10) - mx, 2) + pow(655 - (j * 10) - my, 2));
          if((size + 5 - distance) > 0){
            heightPerUnit[i][j] = myColor;
          }
        }
      }
    }
  }
}

class Palette
{
  Palette(){
  }

  public int selectColor(int mx, int my){
    if(mx >= 200 && mx <= 220){
      if(my >= 440 && my <= 660){ return 32 - (int)(my / 20); }
    }
    return myColor;
  }

  public void draw(){
    fill(255);
    noStroke();
    textFont(basicFont);
    textAlign(CENTER, CENTER);
    text("level", 210 , 420);

    for(int i = 0; i < 11; i++){
      if(myColor == i){
        fill(255);
        textFont(basicFont, 15);
        text(i + 1, 235, 648 - i * 20);
        strokeWeight(2);
        stroke(100, 255, 100);
      }
      else {noStroke();}
      fill(colorByheight[i]);
      rect(200, 640 - i * 20, 20, 20);
    }
  }
}

class Brush
{
  public void draw(){
    fill(255);
    noStroke();
    textFont(basicFont);
    textAlign(CENTER);
    text("size", 210, 320);
    text(brushSize, 210, 360);
  }

  public void drawBrush(){
    if(mouseX >= 300 && mouseX <= 500 && mouseY >= 60 && mouseY <= 660){
      fill(colorByheight[myColor]);
      ellipse(mouseX, mouseY, brushSize * 2, brushSize * 2);
    }
  }
}

public void clean(){
  for(int j = 0; j < 60; j ++){
    for(int i = 0; i < 20; i ++){ heightPerUnit[i][j] = 0; }
  }
}
class View3D
{
    public void setup(){
        createCanvas(200,200);
    }

    public void draw(){
        translate(width / 2, height / 2, 0);
        pushMatrix();
        
        box(150);
        popMatrix();
    }
}
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

  public void makeMatrix(){
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

  public void reset(){
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

public void serialForPlay(){
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

public void serialForHome(){
  mySerial.write("\n");
  //[1,0] is going to home
  String s = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[1,0]}\n";
  mySerial.write(s);
  playMatrix.reset();
}
public void saveJSON(){
  playMatrix.makeMatrix();
  savedJSON = new JSONObject();
  savedJSON.setInt("board_width", 20);
  savedJSON.setInt("board_height", 60);
  JSONArray boardData = new JSONArray();
  int count = -1;
  for(int j = 0; j <= playMatrix.finalRow; j ++){
    if(playMatrix.blocksToGo[j] != 0){
      for(int i = 0; i < 20; i ++){
        if(playMatrix.heightPerMatrix[i][j] != 0){
          count++;
          JSONObject unitData = new JSONObject();
          unitData.setInt("row", j);
          unitData.setInt("col", i);
          unitData.setInt("step_val", playMatrix.heightPerMatrix[i][j]);
          boardData.setJSONObject(count, unitData);
        }
      }
    }
    savedJSON.setJSONArray("board_data", boardData);
  }
  selectOutput("Select a file to write to:", "saveFileSelected");
  playMatrix.reset();
}

public void loadJSON(){
  selectInput("Select a file to process:", "loadFileSelected");
  clean();
  while(!loaded){
    println("loading");
  }
  if(loadSuccess){
    JSONArray boardData = loadedJSON.getJSONArray("board_data");
    for(int i = 0; i < boardData.size(); i ++){
       JSONObject unitData = boardData.getJSONObject(i);
       heightPerUnit[unitData.getInt("col")][unitData.getInt("row")] = unitData.getInt("step_val");
    }
  }
  loaded = false;
}

public void loadFileSelected(File selection) {
  if (selection == null) {
    println("Window was closed or the user hit cancel.");
  } else {
    loadedJSON = loadJSONObject(selection);
    loadSuccess = true;
  }
  loaded = true;
}

public void saveFileSelected(File selection) {
  if (selection == null) {
    println("Window was closed or the user hit cancel.");
  } else {
    saveJSONObject(savedJSON, selection + ".json");
  }
}
  public void settings() {  size(1280, 720); }
  static public void main(String[] passedArgs) {
    String[] appletArgs = new String[] { "TotalControllerGUI_Processing" };
    if (passedArgs != null) {
      PApplet.main(concat(appletArgs, passedArgs));
    } else {
      PApplet.main(appletArgs);
    }
  }
}
