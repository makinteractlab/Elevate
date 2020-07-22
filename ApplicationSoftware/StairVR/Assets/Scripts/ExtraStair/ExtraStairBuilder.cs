using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraStairBuilder : MonoBehaviour
{
    public int mode, controlValue;
    float positionX , positionZ;

    void Start()
    {
        positionX = 0;
        positionZ = 0;
    }

    void Update()
    {
        for (int i = 0; i < 10; i++)
            ExtraStairMaker.makeStair(positionX, positionZ, mode, i, controlValue);
    }

    public void stairPositionSetting()
    {
            GameObject stairContainer = GameObject.Find("stair container");

            Vector3 controllerloc = GameObject.Find("/[CameraRig]/Controller (left)").transform.position;

            positionX = controllerloc.x + (float)0.1;
            positionZ = controllerloc.z;
    }
}
