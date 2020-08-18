import controlP5.*;
import processing.serial.*;

PFont basicFont;
ControlP5 cp5;
Serial mySerial;

MainView mainView;
Palette palette;
Brush brush;
PlayMatrix playMatrix;
View3D view3D;
int heightPerUnit[][] = new int[20][60];
color colorByheight[] = new color[11];
int myColor;
int brushSize;
int delayTime1 = 600;
int delayTime2 = 950;
JSONObject savedJSON;
JSONObject loadedJSON;
boolean loaded;
boolean loadSuccess;
PGraphics model3DView;

void setup(){
  size(1280, 720, P2D);
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

void draw(){
  background(0,0,50);
  mainView.draw();
  mainView.changeColor(mouseX, mouseY, brushSize);
  palette.draw();
  brush.draw();
  brush.drawBrush();
  view3D.draw();
}

void mousePressed(){
  myColor = palette.selectColor(mouseX,mouseY);
  mainView.drawOn(mouseX, mouseY);
}

void mouseReleased(){
  mainView.drawOff();
}

void mouseDragged(){
  if(mouseButton == RIGHT && (mouseX >= 680) && (mouseX <= 1230) && (mouseY >= 110) && (mouseY <= 510)){
    float difx = mouseX - pmouseX;
    float dify = mouseY - pmouseY;
    view3D.rot = new PVector(-dify, difx, 0);
  }
}

void mouseWheel(MouseEvent event){
  if((mouseX >= 680) && (mouseX <= 1230) && (mouseY >= 110) && (mouseY <= 510)){
      float e = event.getCount();
      if(e > 0 && view3D.scroll < 700){
        view3D.scroll += 20;
      }
      
      if(e < 0 && view3D.scroll > 90){
        view3D.scroll -= 20;
      }
  }
}

void keyPressed(){
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

  void draw(){
    for(int j = 0; j < 60; j ++){
      for(int i = 0; i < 20; i ++){
        fill(colorByheight[heightPerUnit[i][j]]);
        strokeWeight(1);
        stroke(0);
        rect(300 + i * 10, 650 - j * 10, 10, 10);
      }
    }
  }
  
  void drawOn(int mx, int my){
    if(mx >= 300 && mx <= 500){
      if(my >= 60 && my <= 660){ drawing = true; }
    }
  }

  void drawOff(){
    drawing = false;
  }

  void changeColor(int mx, int my, int size){
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

  int selectColor(int mx, int my){
    if(mx >= 200 && mx <= 220){
      if(my >= 440 && my <= 660){ return 32 - (int)(my / 20); }
    }
    return myColor;
  }

  void draw(){
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
  void draw(){
    fill(255);
    noStroke();
    textFont(basicFont);
    textAlign(CENTER);
    text("size", 210, 320);
    text(brushSize, 210, 360);
  }

  void drawBrush(){
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
