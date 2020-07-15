using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blocks3DController : MonoBehaviour
{
    int[,] heightPerUnit = new int[20, 60];
    int cubeSize = 50;
    Color basicColor = new Color(204 / 255f, 175 / 255f, 129 / 255f);

    void Start()
    {
        float cubeX = GameObject.Find("ModelControl").transform.position.x;
        float cubeZ = GameObject.Find("ModelControl").transform.position.z;
        for (int j = 0; j < 60; j++)
            for (int i = 0; i < 20; i++)
                blocks3DMaker.MakeCube(new Vector3(-(i * cubeSize + cubeSize / 2) + cubeX, cubeSize / 2, -(j * cubeSize + cubeSize / 2) + cubeZ), basicColor, cubeSize);
    }

    public void reset()
    {
        for (int j = 0; j < 60; j++)
            for (int i = 0; i < 20; i++)
            {
                int num = i + j * 20 + 1;
                GameObject.Find("cube container/cube" + num).transform.position = new Vector3(- i * 50 - 25, cubeSize / 2, -(j * 50 + 25));
                GameObject.Find("cube container/cube" + num).transform.localScale = new Vector3(50, cubeSize, 50);
            }
    }
}
