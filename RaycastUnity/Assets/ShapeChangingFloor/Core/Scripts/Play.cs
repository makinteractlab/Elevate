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

    public string comPort = "COM20";
    public int baudRate = 115200;
    public static SerialPort mySerial;
    public bool testMode;
    PlayMatrix playMatrix;

    void Awake()
    {
        mySerial = new SerialPort(comPort, baudRate);
        playMatrix = new PlayMatrix(mySerial);
        mySerial.Open();
        mySerial.ReadTimeout = 1;
    }

    public void play()
    {
        string JsonBoardData = File.ReadAllText(Application.dataPath + "/Resources/MatrixData/raycastMatrix.json");
        JObject jobj = JObject.Parse(JsonBoardData);
        sendToQueue(jobj);
    }

    public void sendToQueue(JObject jobj)
    {
        if (playMatrix == null)
        {
            UnityEngine.Debug.Log("Play matrix does not exist.");
        }
        if (playMatrix.isPlaying)
        {
            UnityEngine.Debug.Log(jobj);
            playMatrix.updateNextMatrix(jobj);
        } else
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
        //JsonSerializerSettings setting = new JsonSerializerSettings();
        //setting.Formatting = Formatting.Indented;
        //setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //Total_Board_Data resetBoardData;
        //List<Board_Data> reset_board_data_list = new List<Board_Data>();
        //for (int j = 0; j < 60; j++)
        //    for (int i = 0; i < 20; i++)
        //        reset_board_data_list.Add(new Board_Data(i, j, 1));
        //resetBoardData = new Total_Board_Data(20, 60, reset_board_data_list);
        //string JsonBoardData = JsonConvert.SerializeObject(resetBoardData, setting);
        //JObject jobj = JObject.Parse(JsonBoardData);
        //sendToQueue(jobj);
    }
}

class PlayMatrix
{
    Total_Board_Data totalBoardData;
    Total_Board_Data nextBoardData;

    public int[,] heightPerMatrix = new int[20, 60];
    public int[,] previousHeight = new int[20, 60];
    public bool checkEmpty = true;
    public int count = 0;
    public int[] blocksToGo = new int[60];
    public int currentRow = 0;
    public int finalRow = 0;

    private int delayTime1 = 950;
    private int delayTime2 = 300;
    SerialPort mySerial;
    int maxValueInRow = 0;
    int prevMaxValue = 0;
    public bool isPlaying;
    public bool firstPlay;
    public bool nextPlay;

    public float totalTime = 0;
    public Stopwatch sw = new Stopwatch();

    public PlayMatrix(SerialPort mySerial)
    {
        for (int j = 0; j < 60; j++)
        {
            for(int i = 0; i < 20; i++)
                heightPerMatrix[i, j] = 0;
            blocksToGo[j] = 0;
        }
        this.mySerial = mySerial;
        isPlaying = false;
        firstPlay = true;
        nextPlay = false;
    }

    public void makeMatrix()
    {
        for (int j = 0; j < 60; j++)
        {
            for (int i = 0; i < 20; i++)
            {
                int newHeight;
                if (totalBoardData.board_data_list.Exists(item => item.col == i && item.row == j))
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
            }
        }

        for (int j = 0; j <= finalRow; j++)
        {
            if (j != 0)
                totalTime += delayTime1 * blocksToGo[j] / 1000f;
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
                if (prevMaxValue < previous1Height) { prevMaxValue = previous1Height; }
                if (prevMaxValue < previous2Height) { prevMaxValue = previous2Height; }
            }
            totalTime += delayTime2 * (maxValueInRow - 1) / 1000f;
            totalTime += delayTime2 * prevMaxValue / 1000f;
        }
        totalTime += delayTime1 * finalRow / 1000f;
    }

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
    }

    public void stepperMove(int j)
    {
        string s1 = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[0," + blocksToGo[j] + "]}\n";
        mySerial.Write(s1);
    }

    public void motorMove(int j)
    {
        maxValueInRow = 0;
        for (int i = 1; i <= 10; i++)
        {
            int motor1Height = heightPerMatrix[(2 * i) - 2, j] + 1;
            int motor2Height = heightPerMatrix[(2 * i) - 1, j] + 1;
            int previous1Height = previousHeight[(2 * i) - 2, j] + 1;
            int previous2Height = previousHeight[(2 * i) - 1, j] + 1;
            if (maxValueInRow < motor1Height) { maxValueInRow = motor1Height; }
            if (maxValueInRow < motor2Height) { maxValueInRow = motor2Height; }
            if (prevMaxValue < previous1Height) { prevMaxValue = previous1Height; }
            if (prevMaxValue < previous2Height) { prevMaxValue = previous2Height; }
            string s2 = "{\"i\":" + i + ",\"c\":\"motor\",\"d\":[" + motor1Height + "," + motor2Height +  "," + previous1Height + "," + previous2Height + "]}\n";
            int num1 = (2 * i) - 1 + 20 * j;
            int num2 = (2 * i) + 20 * j;
            float cubeX = GameObject.Find("ModelControl").transform.position.x;
            float cubeZ = GameObject.Find("ModelControl").transform.position.z;
            GameObject.Find("cube container/cube" + num1).transform.position = new Vector3(-((2 * i) - 2) * 50 - 25 + cubeX, 50 * motor1Height / 2, -(j * 50 + 25) + cubeZ);
            GameObject.Find("cube container/cube" + num2).transform.position = new Vector3(-((2 * i) - 1) * 50 - 25 + cubeX, 50 * motor2Height / 2, -(j * 50 + 25) + cubeZ);
            GameObject.Find("cube container/cube" + num1).transform.localScale = new Vector3(50, 50 * motor1Height, 50);
            GameObject.Find("cube container/cube" + num2).transform.localScale = new Vector3(50, 50 * motor2Height, 50);
            mySerial.Write(s2);
        }
    }

    public void goHome()
    {
        string s = "{\"i\":" + 11 + ",\"c\":\"stepper\",\"d\":[1,0]}\n";
        mySerial.Write(s);
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
                for (int i = 0; i < nextBoardData.board_data_list.Count; i++)
                nextPlay = false;
            }
            makeMatrix();
            mySerial.Write("\n");
            sw.Start();
            for (int j = 0; j <= finalRow; j++)
            {
                if (j != 0)
                {
                    stepperMove(j);
                    yield return new WaitForSeconds(delayTime1 * blocksToGo[j] / 1000f);
                }
                motorMove(j);
                yield return new WaitForSeconds(delayTime2 * (maxValueInRow + prevMaxValue - 1) / 1000f);
            }
            if (!nextPlay)
                isPlaying = false;

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

class Total_Board_Data
{
    public int board_width = 20;
    public int board_height = 60;
    public List<Board_Data> board_data_list = new List<Board_Data>();
    
    public void JObjectTOTotalData(JObject jobject)
    {
        JArray data_list = (JArray)jobject["board_data_list"];
        this.board_data_list = new List<Board_Data>();

        for(int i = 0; i < data_list.Count; i++)
        {
            var singleData = data_list[i];
            JToken col = singleData["col"];
            JToken row = singleData["row"];
            JToken step_val = singleData["step_val"];
            Board_Data bd = new Board_Data((int)col, (int)row, (int)step_val);
            this.board_data_list.Add(bd);
        }
    }

    public Total_Board_Data(int board_width, int board_height, List<Board_Data> board_data_list)
    {
        this.board_width = board_width;
        this.board_height = board_height;
        this.board_data_list = board_data_list;
    }
}