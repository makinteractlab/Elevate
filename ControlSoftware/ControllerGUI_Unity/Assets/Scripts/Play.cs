using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Ports;

public class Play : MonoBehaviour
{
    [Header("Port Setting")]
    public string BoardComPort;
    public string LockingComPort;
    public int baudRate = 115200;

    [Header("Map Setting")]
    public int mapWidth = 20;
    public int mapLength = 60;

    [Header("Play Mode(Develop/Serial)")]
    public string mode = "Develop";

    [Header("JSON path")]
    public string JSONPath = "/Resources/MatrixData/currentMatrix.json";

    public static SerialPort BoardSerial;
    public static SerialPort LockingSerial;
    PlayMatrix playMatrix;

    void Awake()
    {
        if(mode == "Serial")
        {
            BoardSerial = new SerialPort(BoardComPort, baudRate);
            LockingSerial = new SerialPort(LockingComPort, baudRate);
            BoardSerial.Open();
            LockingSerial.Open();
            LockingSerial.ReadTimeout = 1;            
        }
        playMatrix = new PlayMatrix(BoardSerial, LockingSerial, mapWidth, mapLength, mode);
    }

    //play currentMatrix.json in Resources
    public void play()
    {
        string JsonBoardData = File.ReadAllText(Application.dataPath + JSONPath);
        JObject jobj = JObject.Parse(JsonBoardData);
        sendToQueue(jobj);
    }

    public void sendToQueue(JObject jobj)
    {
        if(playMatrix.isPlaying) { playMatrix.updateNextMatrix(jobj); } // the input data is not json file but just in format of json.
        else
        {
            playMatrix.isPlaying = true;
            StartCoroutine(playMatrix.playSerial(jobj));
        }
    }

    public bool isReady()
    {
        return !(playMatrix.isPlaying);
    }

    public void resetBoard()
    {
        JsonSerializerSettings setting = new JsonSerializerSettings();
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        Board_Data resetBoardData;
        List<Pin_Data> reset_board_data_list = new List<Pin_Data>();
        for (int j = 0; j < mapLength; j++)
            for (int i = 0; i < mapWidth; i++)
                reset_board_data_list.Add(new Pin_Data(i, j, 1));
        resetBoardData = new Board_Data(mapWidth, mapLength, reset_board_data_list);
        string JsonBoardData = JsonConvert.SerializeObject(resetBoardData, setting);
        JObject jobj = JObject.Parse(JsonBoardData);
        sendToQueue(jobj);
    }

    public void goHome()
    {
        if(!playMatrix.isPlaying)
        {
            playMatrix.goHome();
        }
    }

    public void pinHome()
    {
        if(!playMatrix.isPlaying)
        {
            StartCoroutine(playMatrix.pinHome());
        }
    }
}

class PlayMatrix
{
    Board_Data totalBoardData;
    Board_Data nextBoardData;

    public int mapWidth;
    public int mapLength;
    public string mode;

    public int[,] heightPerMatrix; // target height
    public int[,] previousHeight; // existing/current height
    public bool checkEmpty;
    public int count;
    public int[] blocksToGo; // number of rows to just pass by
    public bool[] lockingChecker;
    public int currentRow;
    public int finalRow;
    public int playOrderLength;
    ArrayList playOrderList;

    private int delayTime1 = 1100; // row speed delay servo
    // private int delayTime2 = 300; // pin up speed delay motorUp
    // private int delayTime3 = 400; // pin down speed delay motorDown
    private int delayTime4 = 300; // locking
    SerialPort BoardSerial;
    SerialPort LockingSerial;
    public bool isPlaying;
    public bool firstPlay;
    public bool nextPlay;

    public PlayMatrix(SerialPort BoardSerial, SerialPort LockingSerial, int mapWidth, int mapLength, string mode)
    {
        this.mapWidth = mapWidth;
        this.mapLength = mapLength;
        this.mode = mode;
        heightPerMatrix = new int[mapWidth, mapLength];
        previousHeight = new int[mapWidth, mapLength];
        checkEmpty = true;
        count = 0;
        blocksToGo = new int[mapLength];
        lockingChecker = new bool[mapLength / 4];
        currentRow = 0;
        finalRow = 0;
        playOrderLength = 0;
        playOrderList = new ArrayList();
        
        this.BoardSerial = BoardSerial;
        this.LockingSerial = LockingSerial;

        isPlaying = false;
        firstPlay = true;
        nextPlay = false;

        for (int j = 0; j < mapLength; j++)
        {
            for(int i = 0; i < mapWidth; i++)
                heightPerMatrix[i, j] = 0;
            blocksToGo[j] = 0;
        }
        for (int i = 0; i < 15; i++)
            lockingChecker[i] = false;
    }

    // preparing json file for arduino
    public void makeMatrix()
    {
        for (int j = 0; j < mapLength; j++)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                int newHeight;
                if (totalBoardData.board_data_list.Exists(item => item.col == i && item.row == j)) // not all pins in the matrix are written on json file. if the step val is 0 then deleted.
                    newHeight = totalBoardData.board_data_list.Find(item => item.col == i && item.row == j).step_val;
                else { newHeight = 0; }

                if (heightPerMatrix[i, j] != newHeight)
                {
                    checkEmpty = false;
                }
                previousHeight[i, j] = heightPerMatrix[i, j];
                heightPerMatrix[i, j] = newHeight;
            }
            if (checkEmpty) { count++; }
            else
            {
                count = 0;
                finalRow = j;
                checkEmpty = true;
                lockingChecker[j / 4] = true; // unlocking
                playOrderList.Add(j);
                playOrderLength++;
            }
        }
        float middle = ((int)playOrderList[0] + (int)playOrderList[playOrderLength - 1]) / 2;
        if(middle < currentRow)
        {
            playOrderList.Reverse();
        }
    }

    // purging the matrix
    public void reset()
    {
        for(int j = 0; j < mapLength; j++)
        {
            for (int i = 0; i < mapWidth; i++)
                previousHeight[i, j] = 0;
            blocksToGo[j] = 0;
        }

        count = 0;
        finalRow = 0;
        for (int i = 0; i < 15; i++)
            lockingChecker[i] = false;
        
        playOrderList.Clear();
        playOrderLength = 0;
    }

    //stepper to move the row
    public void stepperMove(int j)
    {
        string s;
        if(currentRow <= (int)playOrderList[j])
        {
            blocksToGo[j] = (int)playOrderList[j] - currentRow;
            s = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[0," + ((int)playOrderList[j] - currentRow) + "]}\n";
        }
        else
        {
            blocksToGo[j] = currentRow - (int)playOrderList[j];
            s = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[1," + (currentRow - (int)playOrderList[j]) + "]}\n";
        }
        if(mode == "Serial") BoardSerial.Write(s);
    }

    public void motorMove(int j)
    {
        for (int i = 1; i <= 10; i++)
        {
            int motor1Height = heightPerMatrix[(2 * i) - 2, j] + 1;
            int motor2Height = heightPerMatrix[(2 * i) - 1, j] + 1;
            int previous1Height = previousHeight[(2 * i) - 2, j] + 1;
            int previous2Height = previousHeight[(2 * i) - 1, j] + 1;

            // if (maxValueInRow < motor1Height) { maxValueInRow = motor1Height; }
            // if (maxValueInRow < motor2Height) { maxValueInRow = motor2Height; }
            // if (prevMaxValue < previous1Height && motor1Height != previous1Height) { prevMaxValue = previous1Height; }
            // if (prevMaxValue < previous2Height && motor2Height != previous2Height) { prevMaxValue = previous2Height; }

            // to modify 3d model in unity
            int num1 = (2 * i) - 1 + mapWidth * j;
            int num2 = (2 * i) + mapWidth * j;
            float cubeX = GameObject.Find("ModelControl").transform.position.x;
            float cubeZ = GameObject.Find("ModelControl").transform.position.z;
            GameObject.Find("cube container/cube" + num1).transform.position = new Vector3(-((2 * i) - 2) * 50 - 25 + cubeX, 50 * motor1Height / 2, -(j * 50 + 25) + cubeZ);
            GameObject.Find("cube container/cube" + num2).transform.position = new Vector3(-((2 * i) - 1) * 50 - 25 + cubeX, 50 * motor2Height / 2, -(j * 50 + 25) + cubeZ);
            GameObject.Find("cube container/cube" + num1).transform.localScale = new Vector3(50, 50 * motor1Height, 50);
            GameObject.Find("cube container/cube" + num2).transform.localScale = new Vector3(50, 50 * motor2Height, 50);
        }
    }

    public void goHome()
    {
        if(mode == "Serial") BoardSerial.Write("\n");
        string s = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[1,0]}\n";
        if(mode == "Serial") BoardSerial.Write(s);            
        currentRow = 0;
    }

    public IEnumerator pinHome()
    {
        for (int i = 1; i <= 5; i++)
        {
            string s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[2,2,0,0]}\n";
            if(mode == "Serial") BoardSerial.Write(s2);            
        }
        yield return new WaitForSeconds(0.001f);
        for (int i = 6; i <= 10; i++)
        {
            string s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[2,2,0,0]}\n";
            if(mode == "Serial") BoardSerial.Write(s2);            
        }
    }

    public IEnumerator playSerial(JObject jobj)
    {
        while(isPlaying)
        {
            if (firstPlay)
            {
                firstMatrix(jobj);
                firstPlay = false;
            }
            else
            {
                totalBoardData.board_data_list.Clear();
                totalBoardData.board_data_list = new List<Pin_Data>(nextBoardData.board_data_list); 
                nextPlay = false;
            }
            makeMatrix(); // about total board data
            if(mode == "Serial") BoardSerial.Write("\n");
            if(mode == "Serial") LockingSerial.Write("\n");
            for (int j = 0; j < playOrderLength; j++)
            {
                stepperMove(j);
                yield return new WaitForSeconds(delayTime1 * blocksToGo[j] / 1000f);

                string openLock = "{\"id\":" + j / 4 + ",\"c\":\"on\"}\n";
                if(mode == "Serial") LockingSerial.Write(openLock);
                yield return new WaitForSeconds(delayTime4 / 1000f);

                if(mode == "Serial")
                {
                    bool isMagnet = false;
                    for (int i = 1; i <= 5; i++)
                    {
                        int motor1Height = heightPerMatrix[(2 * i) - 2, (int)playOrderList[j]] + 1;
                        int motor2Height = heightPerMatrix[(2 * i) - 1, (int)playOrderList[j]] + 1;
                        int previous1Height = previousHeight[(2 * i) - 2, (int)playOrderList[j]] + 1;
                        int previous2Height = previousHeight[(2 * i) - 1, (int)playOrderList[j]] + 1;
                        if(motor1Height < previous1Height)
                        {
                            motor1Height = 1;
                            isMagnet = true;
                        }
                        if(motor2Height < previous2Height)
                        {
                            motor2Height = 1;
                            isMagnet = true;
                        }
                        string s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[" + motor1Height + "," + motor2Height +  "," + previous1Height + "," + previous2Height + "]}\n";
                        if(mode == "Serial") BoardSerial.Write(s2);
                    }
                    yield return new WaitForSeconds(0.001f);
                    for (int i = 6; i <= 10; i++)
                    {
                        int motor1Height = heightPerMatrix[(2 * i) - 2, (int)playOrderList[j]] + 1;
                        int motor2Height = heightPerMatrix[(2 * i) - 1, (int)playOrderList[j]] + 1;
                        int previous1Height = previousHeight[(2 * i) - 2, (int)playOrderList[j]] + 1;
                        int previous2Height = previousHeight[(2 * i) - 1, (int)playOrderList[j]] + 1;
                        if(motor1Height < previous1Height)
                        {
                            motor1Height = 1;
                            isMagnet = true;
                        }
                        if(motor2Height < previous2Height)
                        {
                            motor2Height = 1;
                            isMagnet = true;
                        }
                        string s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[" + motor1Height + "," + motor2Height +  "," + previous1Height + "," + previous2Height + "]}\n";
                        if(mode == "Serial") BoardSerial.Write(s2);
                    }
                    if(isMagnet == true)
                    {
                        if(mode == "Serial")
                        {
                            int pinFin;
                            do
                            {
                                pinFin = BoardSerial.ReadChar();
                                MonoBehaviour.print(pinFin);
                                yield return new WaitForSeconds(0.01f);
                            }
                            while(pinFin != 'a');
                            BoardSerial.BaseStream.Flush();                    
                        }
                        else
                        {
                            yield return new WaitForSeconds(0.1f);
                        }

                        for (int i = 1; i <= 5; i++)
                        {
                            int motor1Height = heightPerMatrix[(2 * i) - 2, (int)playOrderList[j]] + 1;
                            int motor2Height = heightPerMatrix[(2 * i) - 1, (int)playOrderList[j]] + 1;
                            int previous1Height = previousHeight[(2 * i) - 2, (int)playOrderList[j]] + 1;
                            int previous2Height = previousHeight[(2 * i) - 1, (int)playOrderList[j]] + 1;
                            if(motor1Height < previous1Height) previous1Height = 1;
                            else
                            {
                                motor1Height = 1;
                                previous1Height = 1;
                            }
                            if(motor2Height < previous2Height) previous2Height = 1;
                            else
                            {
                                motor2Height = 1;
                                previous2Height = 1;
                            }                         
                            string s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[" + motor1Height + "," + motor2Height +  "," + previous1Height + "," + previous2Height + "]}\n";
                            if(mode == "Serial") BoardSerial.Write(s2);
                        }
                        yield return new WaitForSeconds(0.001f);
                        for (int i = 6; i <= 10; i++)
                        {
                            int motor1Height = heightPerMatrix[(2 * i) - 2, (int)playOrderList[j]] + 1;
                            int motor2Height = heightPerMatrix[(2 * i) - 1, (int)playOrderList[j]] + 1;
                            int previous1Height = previousHeight[(2 * i) - 2, (int)playOrderList[j]] + 1;
                            int previous2Height = previousHeight[(2 * i) - 1, (int)playOrderList[j]] + 1;
                            if(motor1Height < previous1Height) previous1Height = 1;
                            else
                            {
                                motor1Height = 1;
                                previous1Height = 1;
                            }
                            if(motor2Height < previous2Height) previous2Height = 1;
                            else
                            {
                                motor2Height = 1;
                                previous2Height = 1;
                            }  
                            string s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[" + motor1Height + "," + motor2Height +  "," + previous1Height + "," + previous2Height + "]}\n";
                            if(mode == "Serial") BoardSerial.Write(s2);
                        }
                    }                    
                }
                else
                {
                    motorMove((int)playOrderList[j]);
                }

                currentRow = (int)playOrderList[j];

                if(mode == "Serial")
                {
                    int pinFin;
                    do
                    {
                        pinFin = BoardSerial.ReadChar();
                        MonoBehaviour.print(pinFin);
                        yield return new WaitForSeconds(0.01f);
                    }
                    while(pinFin != 'a');
                    BoardSerial.BaseStream.Flush();                    
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }

                string closeLock = "{\"id\":" + j / 4 + ",\"c\":\"off\"}\n"; // off is lock and on is unlock
                if(mode == "Serial") LockingSerial.Write(closeLock);
            }
            if (!nextPlay) isPlaying = false;
            reset();
        }
        firstPlay = true;
    }

    public void firstMatrix(JObject jobj)
    {
        totalBoardData = new Board_Data(mapWidth, mapLength, new List<Pin_Data>());
        totalBoardData.JObjectTOTotalData(jobj);
    }

    public void updateNextMatrix(JObject jobj)
    {
        nextBoardData = new Board_Data(mapWidth, mapLength, new List<Pin_Data>());
        nextBoardData.JObjectTOTotalData(jobj);
        nextPlay = true;
    }
}


class Pin_Data
{
    public int col;
    public int row;
    public int step_val;

    public Pin_Data(int col, int row, int step_val)
    {
        this.col = col;
        this.row = row;
        this.step_val = step_val;
    }
}

class Board_Data
{
    public int board_width;
    public int board_height;
    public List<Pin_Data> board_data_list = new List<Pin_Data>();
    
    public void JObjectTOTotalData(JObject jobject)
    {
        JArray data_list = (JArray)jobject["board_data_list"];
        this.board_data_list = new List<Pin_Data>();

        for(int i = 0; i < data_list.Count; i++)
        {
            var singleData = data_list[i];
            JToken col = singleData["col"];
            JToken row = singleData["row"];
            JToken step_val = singleData["step_val"];
            Pin_Data pd = new Pin_Data((int)col, (int)row, (int)step_val);
            this.board_data_list.Add(pd);
        }
    }

    public Board_Data(int board_width, int board_height, List<Pin_Data> board_data_list)
    {
        this.board_width = board_width;
        this.board_height = board_height;
        this.board_data_list = board_data_list;
    }
}