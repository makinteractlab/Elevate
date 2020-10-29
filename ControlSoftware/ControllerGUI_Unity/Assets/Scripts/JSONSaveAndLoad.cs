using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class JSONSaveAndLoad : MonoBehaviour
{
    public void JSON_save()
    {
        string JsonBoardData = File.ReadAllText(Application.dataPath + "/Resources/MatrixData/currentMatrix.json");
        File.WriteAllText(Application.dataPath + "/Resources/MatrixData/newMatrix.json", JsonBoardData);
    }

    public void JSON_load()
    {
        string JsonBoardData = File.ReadAllText(Application.dataPath + "/Resources/MatrixData/newMatrix.json");
        File.WriteAllText(Application.dataPath + "/Resources/MatrixData/currentMatrix.json", JsonBoardData);
    }

    public void JSON_stairLoad()
    {
        string JsonBoardData = File.ReadAllText(Application.dataPath + "/Resources/MatrixData/test2Matrix.json");
        File.WriteAllText(Application.dataPath + "/Resources/MatrixData/currentMatrix.json", JsonBoardData);
    }
}