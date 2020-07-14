import processing.serial.*;

Serial mySerial;

SideViewGraph side_view_graph;
MouseLine ml;
int a[] = new int[80];

void setup(){
    size(1080, 720);
    background(200);
    for(int i = 0; i < 80; i++){ a[i]= 0; }
    String myPort = Serial.list()[0];
    mySerial = new Serial(this, myPort, 115200);
    side_view_graph = new SideViewGraph();
    side_view_graph.setSideViewGraph(a);
    ml = new MouseLine();
}

void draw(){
  if(!ml.clicked){
    side_view_graph.draw();
  }
  else{
    ml.draw();
  }
}

void mousePressed(){
  ml.Pressed();
}

void mouseReleased(){
  ml.Released();
  side_view_graph.setSideViewGraph(ml.a);
}


class SideViewGraph
{
  int a[] = new int[20];
  void setSideViewGraph(int a[]){
    for(int i = 0; i < 20; i++){
      this.a[i] = a[i];
    }
  }
  
  void draw(){
    background(200);
    fill(255);
    noStroke();
    rect(100,240, 880, 240);
    for(int i = 0; i < 20; i++){
      fill(200,200,255);
      rect(100 + i * 44, 480 - a[i] * 20, 44, a[i] * 20);
    }
    
    for(int i = 0; i < 20; i++){
      stroke(0);
      noFill();
      rect(100 + i * 44, 240, 44, 240);
    }
    
    for(int i = 0; i < 20; i++){
      fill(0);
      text(a[i], 122 + i * 44, 495);
    }
  }
}

class MouseLine
{
  int lastX = -1;
  int lastY = -1;
  boolean clicked = false;
  int a[] = new int[20];
  int aSum[] = new int[20];
  int aNum[] = new int[20];
  
  void draw(){
    stroke(0,0,255);
    if(lastX != -1 && lastY != -1)line(lastX, lastY, mouseX, mouseY);
    lastX = mouseX;
    lastY = mouseY;
    if(100 < mouseX && mouseX < 980) aNum[(mouseX - 100) / 44]++;
    if(100 < mouseX && mouseX < 980) {
      int aVal = (240 - mouseY) / 20 + 12;
      if(aVal < 1) aVal = 1;
      else if(aVal > 12) aVal = 12;
      aSum[(mouseX - 100) / 44] += aVal;
    }
  }
  
  void Pressed(){
    clicked = true;
  }
  
  void Released(){
    clicked = false;
    lastX = -1;
    lastY = -1;
    for(int i = 0; i < 20; i++){
      a[i] = 0;
      if (aNum[i] != 0) a[i] = aSum[i] / aNum[i];
      if(i >= 2 && a[i - 1] == 0) a[i - 1] = (a[i - 2] + a[i]) / 2;
      aSum[i] = 0;
      aNum[i] = 0;
    }
    for(int i = 0; i < 2; i += 2){
      String s = "{\"i\":" + 11 + ",\"c\":\"motor\",\"d\":[" + a[i] + "," + a[i+1] + "]}\n";
      mySerial.write(s);
    }
    if(a[2] <= 6){
      String s = "{\"i\":" + 12 + ",\"c\":\"led\",\"d\":" + 0 + "}\n";
      mySerial.write(s);
    }
    else{
      String s = "{\"i\":" + 12 + ",\"c\":\"led\",\"d\":" + 1 + "}\n";
      mySerial.write(s);
    }
  }
}
