using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stairBuilder : MonoBehaviour
{
    int[,] heightPerUnit = new int[20, 60];
    public int  length, height, width, stepNum;
    float positionX , positionZ;
    stairToJson stairToJson = new stairToJson();
    public Play play;

    public void changinglength(float l)
    {
        length = (int)l;
    }

    public void changingHeight(float h)
    {
        height = (int)h;
    }

    public void changingwidth(float w)
    {
        width = (int)w;
    }

    public void changingStepNumber(float n)
    {
        stepNum = (int)n;
    }
    
    public void playStart()
    {
        stairToJson.stairDraw(length, height, width, stepNum);
        play.play();
    }

    void Start()
    {
        positionX = 0;
        positionZ = 0;
        // for (int i = 0; i < 10; i++)
        //     stairMaker.MakeStair(length, height, width, stepNum, i);
    }

    void Update()
    {
        for (int i = 0; i < 10; i++)
            stairMaker.changeStair(positionX, positionZ, length, height, width, stepNum, i);

        backgroundBuilder.changeHighEnvironment(positionX, positionZ, height);
        backgroundBuilder.changeLowEnvironment(positionX, positionZ);
    }

    public void stairPositionSetting()
    {
            GameObject stairContainer = GameObject.Find("stair container");

            Vector3 controllerloc = GameObject.Find("/[CameraRig]/Controller (left)").transform.position;

            positionX = controllerloc.x + (float)0.1;
            positionZ = controllerloc.z;
    }
}