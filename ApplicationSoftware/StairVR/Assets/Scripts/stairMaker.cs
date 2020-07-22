using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stairMaker : MonoBehaviour
{
    public static void changeStair(float posX, float posZ, int x, int y, int z, int num, int i)
    {
        GameObject changedStair = GameObject.Find("stair container/stair" + (i + 1));

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