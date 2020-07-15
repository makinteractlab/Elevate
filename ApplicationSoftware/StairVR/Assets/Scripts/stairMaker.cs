using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stairMaker : MonoBehaviour
{
    private static GameObject stairPrefab;
    private static GameObject stairContainer;
    private static int stairCount = 0;
    private static List<GameObject> stairs;

    private static GameObject GetStairPrefab()
    {
        if (stairPrefab == null)
            stairPrefab = Resources.Load("stairUnit") as GameObject;
        return stairPrefab;
    }

    // public static GameObject MakeStair(float x, float y, float z, int num, int i)
    // {
    //     stairCount++;
    //     if (stairContainer == null)
    //     {
    //         stairContainer = new GameObject("stair container");
    //         stairs = new List<GameObject>();
    //     }

    //     GameObject stair = Instantiate(GetStairPrefab()) as GameObject;
    //     stairs.Add(stair);
    //     Vector3 loc = stair.transform.position;
    //     loc.x += (float)((x / num) * i);
    //     loc.y += (float)((y / num) * (i + 0.5));
    //     stair.transform.position = loc;
    //     stair.transform.localScale = new Vector3((float)x, (float)y, (float)z);

    //     stair.transform.parent = stairContainer.transform;
    //     stair.name = "stair" + stairCount;
    //     stair.GetComponent<BoxCollider>().enabled = false;
    //     if ((x /num) * i > 1.8 || i >= num) stair.GetComponent<MeshRenderer>().enabled = false;
    //     else stair.GetComponent<MeshRenderer>().enabled = true;

    //     Rigidbody gameObjectsRigidBody = stair.AddComponent<Rigidbody>();
    //     stair.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    //     return stair;
    // }

    public static void changeStair(float posX, float posZ, int x, int y, int z, int num, int i)
    {
        GameObject changedStair = GameObject.Find("stair container/stair" + (i + 1));
        // GameObject hiddenPlane = GameObject.Find("hiddenPlane");

        int blockWidth = 0, blockHeight = 0;
        changedStair.GetComponent<MeshRenderer>().enabled = true;

        if(num >= (i + 1))
        {
            blockWidth = sizeOfStep(x, num, i);
            blockHeight = sumOfStep(y, num, i);
        }
        else
            changedStair.GetComponent<MeshRenderer>().enabled = false;

        Vector3 loc = changedStair.transform.position;
        loc.x = (float)((positionOfStep(x, num, i) + (float)sizeOfStep(x, num, i) / 2) * 0.03) + posX;
        loc.y = (float)(blockHeight * 0.015 / 2);
        loc.z = posZ;

        changedStair.transform.position = loc;
        changedStair.transform.localScale = new Vector3((float)(blockWidth * 0.03), (float)(blockHeight * 0.015), (float)(z * 0.06));


        if(i == 9 && x < 60)
        {
            changedStair.GetComponent<MeshRenderer>().enabled = true;
            Vector3 locLast = changedStair.transform.position;
            locLast.x = (float)((1.8 + (x * 0.03)) / 2);
            locLast.y = (float)(y * 0.015 / 2);
            locLast.z = posZ;
            changedStair.transform.position = locLast;
            changedStair.transform.localScale = new Vector3((float)(1.8 - (x * 0.03)), (float)(y * 0.015), (float)(z * 0.06));
        }
        

        // loc = hiddenPlane.transform.position;
        // loc.x = posX;
        // loc.y = (float)y;
        // loc.z = posZ;
        // hiddenPlane.transform.position = loc;

        // Vector3 sca = hiddenPlane.transform.localScale;
        // sca.z = (float)(z * 0.1);
        // hiddenPlane.transform.localScale = sca;

        // Vector3 rot = hiddenPlane.transform.eulerAngles;
        // rot.z = (float)(Math.Atan((float)(y / x)) * 180 / Math.PI);
        // hiddenPlane.transform.eulerAngles = rot;
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