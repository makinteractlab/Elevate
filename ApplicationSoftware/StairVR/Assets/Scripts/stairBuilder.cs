using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stairBuilder : MonoBehaviour
{
    int[,] heightPerUnit = new int[20, 60];
    public int  width, height, length, stepNum;
    float positionX , positionZ;

    public void changingWidth(float w)
    {
        width = (int)w;
    }

    public void changingHeight(float h)
    {
        height = (int)h;
    }

    public void changingLength(float l)
    {
        length = (int)l;
    }

    public void changingStepNumber(float n)
    {
        stepNum = (int)n;
    }

    void Start()
    {
        positionX = 0;
        positionZ = 0;
        // for (int i = 0; i < 10; i++)
        //     stairMaker.MakeStair(width, height, length, stepNum, i);
    }

    void Update()
    {
        for (int i = 0; i < 10; i++)
            stairMaker.changeStair(positionX, positionZ, width, height, length, stepNum, i);
    }

    public void stairPositionSetting()
    {
            GameObject stairContainer = GameObject.Find("stair container");

            Vector3 controllerloc = GameObject.Find("/[CameraRig]/Controller (left)").transform.position;

            positionX = controllerloc.x + (float)0.1;
            positionZ = controllerloc.z;
    }
}