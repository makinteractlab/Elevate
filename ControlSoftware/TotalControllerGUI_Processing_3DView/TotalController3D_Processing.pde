class View3D
{    
    PMatrix3D q;
    PVector rot = new PVector(0, 0, 0);
    int scroll = 500;
    
    View3D(){
      model3DView = createGraphics(550, 400, P3D);
      q = new PMatrix3D(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1);
    }

    void draw(){
      model3DView.beginDraw();
      model3DView.background(0,0,50);
      //model3DView.camera(0, 0, scroll, 275, 200, 0, 0, 1, 0);
      
      model3DView.translate(0, 450 - (scroll / 5), -550 + scroll);
      model3DView.fill(255);
      model3DView.rotateX(PI/2);
      model3DView.rotateZ(-PI/2);
      model3DView.translate(100, 300, 0);
      
      
      updateq(rot);
      rot = new PVector(0,0,0);
      model3DView.applyMatrix(q);
      
      model3DView.translate(-100, -300, 0);
      
      model3DView.fill(200);
      for(int j = 0; j < 60; j++){
        for(int i = 0; i < 20; i++){
          model3DView.fill(204,175,129);
          model3DView.translate(0, 0, heightPerUnit[i][j] * 5 / 2);
          model3DView.box(10,10, heightPerUnit[i][j] * 5);
          model3DView.translate(0, 0, - (heightPerUnit[i][j] * 5 / 2));
          model3DView.translate(10, 0, 0);
        }
        model3DView.translate(-200, 10, 0);
      }
      model3DView.endDraw();
      image(model3DView, 680, 110);
    }
    
    void updateq(PVector rot){
      if(rot.x != 0 || rot.y != 0){
        PMatrix qt = q.get();
        qt.transpose();
        qt.mult(rot, rot);
        q.rotate(PI / 30, 0, 0, rot.x);
      }
    }
}
