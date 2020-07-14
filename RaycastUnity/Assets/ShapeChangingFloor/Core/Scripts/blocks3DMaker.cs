using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blocks3DMaker : MonoBehaviour
{
    private static GameObject cubePrefab;
    private static GameObject cubeContainer;
    private static int cubeCount = 0;
    private static List<GameObject> cubes;

    private static GameObject GetCubePrefab()
    {
        if (cubePrefab == null)
            cubePrefab = Resources.Load("Cube") as GameObject;
        return cubePrefab;
    }

    public static GameObject MakeCube(Vector3 position, Color color, int Height)
    {
        int cubeSize = 50;
        cubeCount++;
        if (cubeContainer == null)
        {
            cubeContainer = new GameObject("cube container");
            cubes = new List<GameObject>();
        }

        GameObject cube = Instantiate(GetCubePrefab()) as GameObject;
        cubes.Add(cube);
        cube.transform.position = position;
        cube.transform.parent = cubeContainer.transform;
        cube.name = "cube" + cubeCount;

        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.localScale = new Vector3(cubeSize, Height, cubeSize);

        return cube;
    }
}