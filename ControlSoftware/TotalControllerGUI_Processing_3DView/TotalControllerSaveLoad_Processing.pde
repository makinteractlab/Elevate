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
    savedJSON.setJSONArray("board_data_list", boardData);
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
    JSONArray boardData = loadedJSON.getJSONArray("board_data_list");
    for(int i = 0; i < boardData.size(); i ++){
       JSONObject unitData = boardData.getJSONObject(i);
       heightPerUnit[unitData.getInt("col")][unitData.getInt("row")] = unitData.getInt("step_val");
    }
  }
  loaded = false;
}

void loadFileSelected(File selection) {
  if (selection == null) {
    println("Window was closed or the user hit cancel.");
  } else {
    loadedJSON = loadJSONObject(selection);
    loadSuccess = true;
  }
  loaded = true;
}

void saveFileSelected(File selection) {
  if (selection == null) {
    println("Window was closed or the user hit cancel.");
  } else {
    saveJSONObject(savedJSON, selection + ".json");
  }
}
