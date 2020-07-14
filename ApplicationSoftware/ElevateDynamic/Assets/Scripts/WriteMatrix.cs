using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WriteMatrix : MonoBehaviour
{

    public Transform pin;
    public Transform pinParent;
    private int pinRowNum = 20;
    private int pinColNum = 60;
    private int pinSize = 1; // its 3 cm in real prototype
    public Transform origin;
    //public Transform pinParent;
    public int minHeight = 1;
    public int maxHeight = 10;
    public int defaultPinHeight =2;
    public int[,] pinHeight;
    public float elevationScale= 0.3f;
    private int layermask = 1<<9;
    private Vector3 savedOrigin;
    public int offsetX;
    public int offsetY;
    public int offsetZ;
    JsonSerializerSettings serializerSet = new JsonSerializerSettings();
    ShaderControl ShaderCheck;
    Play HWserial;
    public Transform stones;

    Total_Board_Data totalBoardData;
    List<Board_Data> board_data_list = new List<Board_Data>();


    // Start is called before the first frame update
    void Start()
    {
        serializerSet.Formatting = Formatting.Indented;
        serializerSet.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        pinHeight = new int[pinRowNum, pinColNum];
        ShaderCheck = stones.GetComponent<ShaderControl>();
        HWserial = transform.GetComponent<Play>();


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.O))
        {
            savedOrigin = new Vector3(origin.position.x+offsetX, origin.position.y+offsetY, origin.position.z+offsetZ);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            setMatrix(savedOrigin);
        }

    }


    // for testing and debuggin
    public void generateRandomHeight(int seedNum, int[,] height)
    {
        Random.seed = seedNum;
        for (int i = 0; i < pinRowNum; i++) for (int j = 0; j < pinColNum; j++) height[i, j] = Random.Range(minHeight, maxHeight + 1);
    }

    public void setMatrix(Vector3 originPt)
    {
        ShaderCheck.ready = false;
        board_data_list.Clear();
        for (int i = 0; i < pinRowNum; i++)
        {
            for (int j = 0; j < pinColNum; j++)
            {
                Vector3 pinPosition = new Vector3(originPt.x + i, originPt.y, originPt.z+j);

                RaycastHit hit;
                if (Physics.Raycast(pinPosition, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity,layermask))
                {
                    if(hit.distance * elevationScale < maxHeight && hit.distance * elevationScale > minHeight)
                    {
                        int pinH =  Mathf.RoundToInt(hit.distance * elevationScale);
                        board_data_list.Add(new Board_Data(i, j, pinH));
                        //Debug.Log("Did Hit");
                        //Instantiate(pin, new Vector3(pinPosition.x + (i), pinPosition.y + (-1 * pinH), pinPosition.z + (j)), Quaternion.identity,pinParent);


                    }
                    else if(hit.distance * elevationScale >= maxHeight)
                    {
                        board_data_list.Add(new Board_Data(i, j, maxHeight));
                        //Debug.Log("Did Hit");
                        //Instantiate(pin, new Vector3(pinPosition.x + (i), pinPosition.y + (-1 * maxHeight), pinPosition.z + (j)), Quaternion.identity,pinParent);


                    }
                    else if(hit.distance * elevationScale <= minHeight )

                    {
                        board_data_list.Add(new Board_Data(i, j, minHeight));
                        //Debug.Log("Did Hit");
                        //Instantiate(pin, new Vector3(pinPosition.x + (i), pinPosition.y + (-1 * minHeight), pinPosition.z + (j)), Quaternion.identity,pinParent);
                    }
                }
                else
                {
                    board_data_list.Add(new Board_Data(i, j, defaultPinHeight));
                    //Instantiate(pin, new Vector3(pinPosition.x + (i), pinPosition.y + (-1 * defaultPinHeight), pinPosition.z + (j)), Quaternion.identity,pinParent);
                    //Debug.Log("Did not Hit");
                }
            }
        }

        totalBoardData = new Total_Board_Data(20, 60, board_data_list);
        string JsonBoardData = JsonConvert.SerializeObject(totalBoardData, serializerSet);
        File.WriteAllText(Application.dataPath + "/Resources/currentMatrix.json", JsonBoardData);
        ShaderCheck.ready = true;
        // writing serial to HW
        //HWserial.play();

    }



}

