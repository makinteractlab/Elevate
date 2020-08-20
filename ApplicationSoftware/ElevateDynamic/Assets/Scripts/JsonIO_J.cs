using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class JsonIO_J : MonoBehaviour
{
    public string loadJsonFile;
    public GameObject pins;
    public GameObject pinObject;
    public GameObject floor;

    [Serializable]
    public class Pin
    {
        public int col;
        public int step_val;
        public int row;
    }

    [Serializable]
    public class Board
    {
        
        public Pin [] board_data = new Pin[1];
        public int board_height;
        public int board_width;
    }

    [SerializeField] Board board;

    string GetPath ()
    {
        string path = Path.Combine(Application.dataPath, loadJsonFile);
        Debug.Log(path);
        return path;
    }

    public void Save ()
    {
        string jsonString = JsonUtility.ToJson(board, true);
        StreamWriter writer = new StreamWriter (GetPath());
        writer.Write(jsonString);
        writer.Close();
    }

    public void Load ()
    {
        StreamReader reader = new StreamReader (GetPath());
        string jsonString = reader.ReadToEnd();
        reader.Close();
        JsonUtility.FromJsonOverwrite(jsonString, board);
    }

    public void Create()
    {
        floor.transform.localScale = new Vector3 (0.03f * board.board_width / 10f, floor.transform.localScale.y, 0.03f * board.board_height / 10f);
        floor.transform.localPosition = new Vector3 (0.03f * (board.board_width-1) / 2f, floor.transform.localPosition.y, 0.03f * (board.board_height-1) / 2f);
        
        foreach (Pin pinData in board.board_data) {
            GameObject pin = Instantiate(pinObject, new Vector3 (0.03f * pinData.col, 0f, 0.03f * pinData.row), Quaternion.identity);
            pin.transform.parent = pins.transform;
            pin.transform.localScale = new Vector3 (0.03f, 0.015f * pinData.step_val, 0.03f);
            pin.transform.localPosition = new Vector3 (pin.transform.localPosition.x, 0.015f * pinData.step_val / 2f, pin.transform.localPosition.z);
            pin.GetComponent<PinObject_J>().col = pinData.col;
            pin.GetComponent<PinObject_J>().row = pinData.row;
            pin.GetComponent<PinObject_J>().step_val = pinData.step_val;
        }
    }

    public void Erase()
    {
        foreach (Transform child in pins.transform) {
            DestroyImmediate(child.gameObject);
        }
    }
}
