using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class simpleDraw : MonoBehaviour
{
    Total_Board_Data totalBoardData;
    List<Board_Data> board_data_list = new List<Board_Data>();
    JsonSerializerSettings setting = new JsonSerializerSettings();
    //JObject init = new JObject();
    public Text time;
    public Play play;
    
    void Start()
    {
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //init["board_width"] = 20;
        //init["board_height"] = 60;

        for (int j = 0; j < 60; j++)
            for (int i = 0; i < 20; i++)
                board_data_list.Add(new Board_Data(i, j, (int)UnityEngine.Random.Range(1, 11)));
        totalBoardData = new Total_Board_Data(20, 60, board_data_list);
    }

    void Update()
    {
        time.text = "time: " + play.getTimeleft() + "sec";
    }

    public void stairDraw()
    {
        board_data_list.Clear();
        for (int j = 0; j < 60; j++)
            for (int i = 0; i < 20; i++)
                board_data_list.Add(new Board_Data(i, j, (int)UnityEngine.Random.Range(1, 11)));
        totalBoardData = new Total_Board_Data(20, 60, board_data_list);

        string JsonBoardData = JsonConvert.SerializeObject(totalBoardData, setting);
        File.WriteAllText(Application.dataPath + "/Resources/MatrixData/currentMatrix.json", JsonBoardData);
    }

    // size of i th block of num
    static int sizeOfStep(int tot, int num, int i)
    {
        int min = (int)Math.Truncate((double)(tot / num));
        int extraStepLeft = tot - num * min;
        if(i < extraStepLeft) return min + 1;
        return min;
    }

    // position of i th block of num
    static int positionOfStep(int tot, int num, int i)
    {
        int pos = 0;
        for(int j = 0; j < i; j ++)
        {
            pos += sizeOfStep(tot, num, j);
        }
        return pos;
    }

    static int sumOfStep(int tot, int num, int i)
    {
        return sizeOfStep(tot, num, i) + positionOfStep(tot, num, i);
    }
}
