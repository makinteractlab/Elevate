using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraStairMaker : MonoBehaviour
{
    private static int[] stair2Height = {10, 10, 9, 8, 7, 6, 5, 4, 3, 2};
    private static int[] stair3Height = {2, 4, 6, 8, 10, 10, 8, 6, 4, 2};
    private static int[] stair4Height = {10, 8, 6, 4, 2, 2, 4, 6, 8, 10};

    public static void makeStair(float posX, float posZ, int mode, int i, int val)
    {
        GameObject changedStair = GameObject.Find("stair container/stair" + (i + 1));
        changedStair.GetComponent<MeshRenderer>().enabled = true;

        //mode 2 to 4
        if(mode >= 2 && mode <= 4)
        {
            int blockWidth = 6;
            int blockHeight = 0;
            if(mode == 2)
            {
                blockHeight = stair2Height[i];
            }
            else if(mode == 3)
            {
                blockHeight = stair3Height[i];
            }
            else
            {
                blockHeight = stair4Height[i];
            }

            Vector3 loc = new Vector3();
            loc.x = (float)(blockWidth * (i + 0.5) * 0.03) + posX;
            loc.y = (float)(blockHeight * 0.0075);
            loc.z = posZ;

            changedStair.transform.position = loc;
            changedStair.transform.localScale = new Vector3((float)(blockWidth * 0.03), (float)(blockHeight * 0.015), (float)(0.6));
        }

        //mode 5
        else if(mode == 5)
        {
            int blockWidth = 12;
            int blockHeight = (i + val) % 10 + 1;

            Vector3 loc = new Vector3();
            if(i < 5)
            {
                loc.x = (float)(blockWidth * (i + 0.5) * 0.03) + posX;
                loc.y = (float)(blockHeight * 0.0075);
                loc.z = (float)(0.15) + posZ;                
            }

            else
            {
                loc.x = (float)(blockWidth * ((9 - i) + 0.5) * 0.03) + posX;
                loc.y = (float)(blockHeight * 0.0075);
                loc.z = (float)(-0.15) + posZ;                
            }


            changedStair.transform.position = loc;
            changedStair.transform.localScale = new Vector3((float)(blockWidth * 0.03), (float)(blockHeight * 0.015), (float)(0.3));
        }

        //model 6
        else if(mode == 6)
        {
            int[] lenVal = {8, 7, 7, 8};
            int[] heightVal = {1, 4, 7, 10};
            Vector3 loc = new Vector3();
            Vector3 sca = new Vector3();
            switch (i)
            {
                case 0:
                    loc.x = (float)(((float)lenVal[0] / 2) * 0.03) + posX;
                    loc.y = (float)(heightVal[3] * 0.0075);
                    loc.z = posZ;
                    sca.x = (float)(lenVal[0] * 0.03);
                    sca.y = (float)(heightVal[3] * 0.015);
                    sca.z = (float)(0.6);
                    break;
                case 1:
                    loc.x = (float)(0.9) + posX;
                    loc.y = (float)(heightVal[3] * 0.0075);
                    loc.z = (float)(0.225) + posZ;
                    sca.x = (float)((60 - lenVal[0] * 2) * 0.03);
                    sca.y = (float)(heightVal[3] * 0.015);
                    sca.z = (float)(0.15);
                    break;
                case 2:
                    loc.x = (float)((60 - ((float)lenVal[0] / 2)) * 0.03) + posX;
                    loc.y = (float)(heightVal[3] * 0.0075);
                    loc.z = posZ;
                    sca.x = (float)(lenVal[0] * 0.03);
                    sca.y = (float)(heightVal[3] * 0.015);
                    sca.z = (float)(0.6);
                    break;
                case 3:
                    loc.x = (float)((lenVal[0] + ((float)lenVal[1] / 2)) * 0.03) + posX;
                    loc.y = (float)(heightVal[2] * 0.0075);
                    loc.z = (float)(-0.075) + posZ;
                    sca.x = (float)(lenVal[1] * 0.03);
                    sca.y = (float)(heightVal[2] * 0.015);
                    sca.z = (float)(0.45);
                    break;
                case 4:
                    loc.x = (float)(0.9) + posX;
                    loc.y = (float)(heightVal[2] * 0.0075);
                    loc.z = (float)(0.075) + posZ;
                    sca.x = (float)((60 - (lenVal[0] + lenVal[1]) * 2) * 0.03);
                    sca.y = (float)(heightVal[2] * 0.015);
                    sca.z = (float)(0.15);
                    break;
                case 5:
                    loc.x = (float)((60 - lenVal[0] - ((float)lenVal[1] / 2)) * 0.03) + posX;
                    loc.y = (float)(heightVal[2] * 0.0075);
                    loc.z = (float)(-0.075) + posZ;
                    sca.x = (float)(lenVal[1] * 0.03);
                    sca.y = (float)(heightVal[2] * 0.015);
                    sca.z = (float)(0.45);
                    break;
                case 6:
                    loc.x = (float)((lenVal[0] + lenVal[1] + ((float)lenVal[2] / 2)) * 0.03) + posX;
                    loc.y = (float)(heightVal[1] * 0.0075);
                    loc.z = (float)(-0.15) + posZ;
                    sca.x = (float)(lenVal[2] * 0.03);
                    sca.y = (float)(heightVal[1] * 0.015);
                    sca.z = (float)(0.3);
                    break;
                case 7:
                    loc.x = (float)(0.9) + posX;
                    loc.y = (float)(heightVal[1] * 0.0075);
                    loc.z = (float)(-0.075) + posZ;
                    sca.x = (float)((60 - (lenVal[0] + lenVal[1] + lenVal[2]) * 2) * 0.03);
                    sca.y = (float)(heightVal[1] * 0.015);
                    sca.z = (float)(0.15);
                    break;
                case 8:
                    loc.x = (float)((60 - lenVal[0] - lenVal[1] - ((float)lenVal[2] / 2)) * 0.03) + posX;
                    loc.y = (float)(heightVal[1] * 0.0075);
                    loc.z = (float)(-0.075) + posZ;
                    sca.x = (float)(lenVal[2] * 0.03);
                    sca.y = (float)(heightVal[1] * 0.015);
                    sca.z = (float)(0.45);
                    break;
                case 9:
                    loc.x = (float)(0.9) + posX;
                    loc.y = (float)(heightVal[0] * 0.0075);
                    loc.z = (float)(-0.225) + posZ;
                    sca.x = (float)(((lenVal[3]) * 2) * 0.03);
                    sca.y = (float)(heightVal[0] * 0.015);
                    sca.z = (float)(0.15);
                    break;                
            }

            changedStair.transform.position = loc;
            changedStair.transform.localScale = sca;
        }
    }
}
