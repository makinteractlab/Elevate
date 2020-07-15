using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class blocks2DController : MonoBehaviour
{
    Total_Board_Data totalBoardData;
    string JsonBoardData;
    public int[,] blockMatrix = new int[20, 60];

    public Color blockColor(int h)
    {
        return new Color((255 - h * 21) / 255f, (255 - h * 21) / 255f, (255 - h * 5) / 255f);
    }

    void Start()
    {
        JsonBoardData = File.ReadAllText(Application.dataPath + "/Resources/MatrixData/currentMatrix.json");
        totalBoardData = JsonConvert.DeserializeObject<Total_Board_Data>(JsonBoardData);

        for (int j = 0; j < 60; j++)
            for (int i = 0; i < 20; i++)
                blocks2DMaker.MakeBlock(new Vector2(i, j + 6), blockColor(0)); 
    }

    public void setBlockMatrix(int i, int j, int val)
    {
        blockMatrix[i, j] = val;
    }

    public int getBlockMatrix(int i, int j)
    {
        return blockMatrix[i, j];
    }

    void Update()
    {
        JsonBoardData = File.ReadAllText(Application.dataPath + "/Resources/MatrixData/currentMatrix.json");
        totalBoardData = JsonConvert.DeserializeObject<Total_Board_Data>(JsonBoardData);
        int count = 0;
        for (int j = 0; j < 60; j++)
            for (int i = 0; i < 20; i++)
            {
                count++;
                int step_val = 0;
                if (totalBoardData.board_data_list.Exists(item => item.col == i && item.row == j))
                    step_val = totalBoardData.board_data_list.Find(item => item.col == i && item.row == j).step_val;
                GameObject.Find("block container/block" + count).GetComponent<Renderer>().material.color = blockColor(step_val);
            }
    }
}