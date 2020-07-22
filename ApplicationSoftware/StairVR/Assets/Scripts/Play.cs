using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Ports;
using System.Diagnostics;

public class Play : MonoBehaviour
{
    public string BoardComPort = "COM3";
    public string LockingComPort = "COM4";
    public int baudRate = 115200;
    public static SerialPort BoardSerial;
    public static SerialPort LockingSerial;
    PlayMatrix playMatrix;

    void Awake()
    {
        BoardSerial = new SerialPort(BoardComPort, baudRate);
        LockingSerial = new SerialPort(LockingComPort, baudRate);
        playMatrix = new PlayMatrix(BoardSerial, LockingSerial);

        BoardSerial.Open();
        BoardSerial.ReadTimeout = 1;
        LockingSerial.Open();
        LockingSerial.ReadTimeout = 1;
    }

    public void play()
    {
        string JsonBoardData = File.ReadAllText(Application.dataPath + "/Resources/MatrixData/currentMatrix.json");
        JObject jobj = JObject.Parse(JsonBoardData);
        sendToQueue(jobj);
    }

    public void sendToQueue(JObject jobj)
    {
        if (playMatrix.isPlaying) { playMatrix.updateNextMatrix(jobj); } // the input data is not json file but just in format of json.
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

    public int getTimeleft()
    {
        if(playMatrix.totalTime - playMatrix.sw.ElapsedMilliseconds / 1000.0f > 0)
            return (int)(playMatrix.totalTime - playMatrix.sw.ElapsedMilliseconds / 1000.0f);
        return 0;
    }

    public void resetBoard()
    {
        JsonSerializerSettings setting = new JsonSerializerSettings();
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        Total_Board_Data resetBoardData;
        List<Board_Data> reset_board_data_list = new List<Board_Data>();
        for (int j = 0; j < 60; j++)
            for (int i = 0; i < 20; i++)
                reset_board_data_list.Add(new Board_Data(i, j, 0));
        resetBoardData = new Total_Board_Data(20, 60, reset_board_data_list);
        string JsonBoardData = JsonConvert.SerializeObject(resetBoardData, setting);
        JObject jobj = JObject.Parse(JsonBoardData);
        sendToQueue(jobj);
    }
}

class PlayMatrix
{
    Total_Board_Data totalBoardData;
    Total_Board_Data nextBoardData;

    public int[,] heightPerMatrix = new int[20, 60]; // target height
    public int[,] previousHeight = new int[20, 60]; // existing/current height
    public bool checkEmpty = true;
    public int count = 0;
    public int[] blocksToGo = new int[60];
    public bool[] lockingChecker = new bool[15];
    public int currentRow = 0;
    public int finalRow = 0;

    private int delayTime1 = 950; // row speed delay servo
    private int delayTime2 = 300; // pin up speed delay motorUp
    private int delayTime3 = 400; // pin down speed delay motorDown
    private int delayTime4 = 300; // locking
    SerialPort BoardSerial;
    SerialPort LockingSerial;
    int maxValueInRow = 0; // checking whether there is any changes in height
    int prevMaxValue = 0;
    public bool isPlaying;
    public bool firstPlay;
    public bool nextPlay;

    int motorDelayCounter = 0;

    public float totalTime = 0;
    public Stopwatch sw = new Stopwatch();

    public PlayMatrix(SerialPort BoardSerial, SerialPort LockingSerial)
    {
        for (int j = 0; j < 60; j++)
        {
            for(int i = 0; i < 20; i++)
                heightPerMatrix[i, j] = 0;
            blocksToGo[j] = 0; // number of rows to just pass by
        }
        for (int i = 0; i < 15; i++)
            lockingChecker[i] = false;

        this.BoardSerial = BoardSerial;
        this.LockingSerial = LockingSerial;
        isPlaying = false;
        firstPlay = true;
        nextPlay = false;
    }

    // preparing json file for arduino
    public void makeMatrix()
    {
        for (int j = 0; j < 60; j++)
        {
            for (int i = 0; i < 20; i++)
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
                blocksToGo[j] = count + 1;
                count = 0;
                finalRow = j;
                checkEmpty = true;
                lockingChecker[j / 4] = true; // unlocking 
            }
        }

        for (int j = 0; j <= finalRow; j++)
        {
            if (j != 0) totalTime += delayTime1 * blocksToGo[j] / 1000f;
            maxValueInRow = 0;
            prevMaxValue = 0;
            for (int i = 1; i <= 10; i++)
            {
                int motor1Height = heightPerMatrix[(2 * i) - 2, j] + 1; // default height settup. 
                int motor2Height = heightPerMatrix[(2 * i) - 1, j] + 1;
                int previous1Height = previousHeight[(2 * i) - 2, j] + 1;
                int previous2Height = previousHeight[(2 * i) - 1, j] + 1;
                // to calculate maximum amount of time to push a single height pin
                if (maxValueInRow < motor1Height) { maxValueInRow = motor1Height; }
                if (maxValueInRow < motor2Height) { maxValueInRow = motor2Height; }
                if (prevMaxValue < previous1Height && motor1Height != previous1Height) { prevMaxValue = previous1Height; }
                if (prevMaxValue < previous2Height && motor2Height != previous2Height) { prevMaxValue = previous2Height; }
            }
            totalTime += delayTime2 * (maxValueInRow - 1) / 1000f;
            totalTime += delayTime3 * prevMaxValue / 1000f;
        }
        totalTime += delayTime1 * finalRow / 1000f;
    }

    // purging the matrix
    public void reset()
    {
        for(int j = 0; j < 60; j++)
        {
            for (int i = 0; i < 20; i++)
                previousHeight[i, j] = 0;
            blocksToGo[j] = 0;
        }

        count = 0;
        finalRow = 0;
        for (int i = 0; i < 15; i++)
            lockingChecker[i] = false;
    }

    //stepper to move the row
    public void stepperMove(int j)
    {
        string s1 = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[0," + blocksToGo[j] + "]}\n";
        BoardSerial.Write(s1);
    }

    public void motorMove(int j)
    {
        maxValueInRow = 0;
        prevMaxValue = 0;
        for (int i = 1; i <= 10; i++)
        {
            int motor1Height = heightPerMatrix[(2 * i) - 2, j] + 1;
            int motor2Height = heightPerMatrix[(2 * i) - 1, j] + 1;
            int previous1Height = previousHeight[(2 * i) - 2, j] + 1;
            int previous2Height = previousHeight[(2 * i) - 1, j] + 1;
            if (maxValueInRow < motor1Height) { maxValueInRow = motor1Height; }
            if (maxValueInRow < motor2Height) { maxValueInRow = motor2Height; }
            if (prevMaxValue < previous1Height && motor1Height != previous1Height) { prevMaxValue = previous1Height; }
            if (prevMaxValue < previous2Height && motor2Height != previous2Height) { prevMaxValue = previous2Height; }
            string s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[" + motor1Height + "," + motor2Height +  "," + previous1Height + "," + previous2Height + "]}\n";
            BoardSerial.Write(s2);
        }
    }

    public void goHome()
    {
        string s = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[1,0]}\n";
        BoardSerial.Write(s);
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
                totalBoardData.board_data_list = new List<Board_Data>(nextBoardData.board_data_list);
                //for (int i = 0; i < nextBoardData.board_data_list.Count; i++) 
                nextPlay = false;
            }
            makeMatrix(); // about total board data
            BoardSerial.Write("\n");
            sw.Start();
            for (int j = 0; j <= finalRow; j++)
            {
                if (j != 0)
                {
                    stepperMove(j);
                    yield return new WaitForSeconds(delayTime1 * blocksToGo[j] / 1000f);
                }
                if(j % 4 == 0 && lockingChecker[j / 4])
                {
                    string s = "{\"id\":" + j / 4 + ",\"c\":\"on\"}\n";
                    LockingSerial.Write(s);
                    yield return new WaitForSeconds(delayTime4 / 1000f);
                }

                motorMove(j);
                if (prevMaxValue == 1) yield return new WaitForSeconds(delayTime2 * (maxValueInRow - 1) / 1000f);
                else yield return new WaitForSeconds(delayTime2 * (maxValueInRow - 1) + delayTime3 * prevMaxValue / 1000f);


                // last row in the locking group
                if ((j % 4 == 3 || j == finalRow) && lockingChecker[j / 4])
                {
                    string s = "{\"id\":" + j / 4 + ",\"c\":\"off\"}\n"; // off is lock and on is unlock
                    LockingSerial.Write(s);
                    yield return new WaitForSeconds(delayTime4 / 1000f);
                }
            }
            if (!nextPlay) isPlaying = false;
            goHome();
            yield return new WaitForSeconds(delayTime1 * finalRow / 1000f);
            totalTime = 0;
            sw.Stop();
            sw.Reset();
            reset();
        }
        firstPlay = true;
    }

    public void firstMatrix(JObject jobj)
    {
        totalBoardData = new Total_Board_Data(20, 60, new List<Board_Data>());
        totalBoardData.JObjectTOTotalData(jobj);
    }

    public void updateNextMatrix(JObject jobj)
    {
        nextBoardData = new Total_Board_Data(20, 60, new List<Board_Data>());
        nextBoardData.JObjectTOTotalData(jobj);
        nextPlay = true;
    }
}


// i guess this is pin data
class Board_Data
{
    public int col;
    public int row;
    public int step_val;

    public Board_Data(int col, int row, int step_val)
    {
        this.col = col;
        this.row = row;
        this.step_val = step_val;
    }
}


// and this could be the board data
class Total_Board_Data
{
    public int board_width = 20;
    public int board_height = 60;
    public List<Board_Data> board_data_list = new List<Board_Data>();
    
    public void JObjectTOTotalData(JObject jobject)
    {
        JArray data_list = (JArray)jobject["board_data_list"];
        this.board_data_list = new List<Board_Data>();  // not sure this is necessary

        for(int i = 0; i < data_list.Count; i++)
        {
            var singleData = data_list[i];
            JToken col = singleData["col"];
            JToken row = singleData["row"];
            JToken step_val = singleData["step_val"];
            Board_Data bd = new Board_Data((int)col, (int)row, (int)step_val);
            this.board_data_list.Add(bd); // not sure this is necessary
        }
    }

    public Total_Board_Data(int board_width, int board_height, List<Board_Data> board_data_list)
    {
        this.board_width = board_width;
        this.board_height = board_height;
        this.board_data_list = board_data_list;
    }
}