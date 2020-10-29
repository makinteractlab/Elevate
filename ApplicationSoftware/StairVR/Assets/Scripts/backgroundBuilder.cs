using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundBuilder : MonoBehaviour
{
    static float defaultHighPosX = (float)0.9;
    static float defaultHighPosY = (float)(-0.85);
    static float defaultHighPosZ = (float)0;

    static float defaultLowPosX = (float)0;
    static float defaultLowPosY = (float)0;
    static float defaultLowPosZ = (float)0;

    public static void changeHighEnvironment(float posX, float posZ, int y)
    {
        GameObject changedHighEnv = GameObject.Find("environment/highEnvironment");

        Vector3 posH;
        posH.x = defaultHighPosX + posX;
        posH.y = defaultHighPosY + (float)(y * 0.015);
        posH.z = defaultHighPosZ + posZ;

        changedHighEnv.transform.position = posH;
    }

    public static void changeLowEnvironment(float posX, float posZ)
    {
        GameObject changedLowEnv = GameObject.Find("environment/lowEnvironment");

        Vector3 posL;
        posL.x = defaultLowPosX + posX;
        posL.y = defaultLowPosY;
        posL.z = defaultLowPosZ + posZ;

        changedLowEnv.transform.position = posL;
    }
}
