using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WriteMatrix : MonoBehaviour
{

    [Header("device specifications")]
    private int pinRowNum = 60;
    private int pinColNum = 20;
    public int pinSteps = 10;
    [Header("pin measurements")]
    [HideInInspector]
    public float pinW_mm = 30; //in mm
    private float pinW_M = 0;
    [HideInInspector]
    public float pinH_mm = 150; // in mm
    private float pinH_M = 0;
    [Header("remap specifications")]
    [HideInInspector]
    public float lowestPosition = 0f;
    [HideInInspector]
    public float highestPosition = 2f;
    public bool adaptiveRange = false;
    public bool clipHeight = false;
    public Transform relativeTo;
    [Header("Pins")]
    public GameObject pinPrefab;
    public GameObject pinParent;
    public List<Vector3> pinArrayPositions;
    private Vector3[] rayHitPoints;
    [HideInInspector]
    public List<GameObject> pinSimulations = new List<GameObject>();


    [Header("Interactive settings")]
    public List<Transform> maskTargets;
    public bool maskTargetRows = true;
    public float maskPadding_mm = 600;
    private float maskPadding_M = 0;
    public bool dynamicEnvironment = false;
    public int framesPerMin = 4;
    private float seconsPerFrame = 0;
    public Play HWserial;
    ShaderControl ShaderCheck;


    [Header("Debug settings")]
    public bool debugLog = false;
    [Header("Send JSON")]
    public bool connectHW = false;


    [Header("JSON path")]
    public string JSONPath = "/Resources/MatrixData/currentMatrix.json";
    JsonSerializerSettings serializerSetting = new JsonSerializerSettings();

    Board_Data totalBoardData; // full floor info
    List<Pin_Data> board_data_list = new List<Pin_Data>(); // each pin info in a list

    private int layermask = 1 << 8;



    public Transform origin;
    public Transform stones;



    //public int minHeight = 1;
    //public int maxHeight = 10;
    //public int defaultPinHeight = 2;
    //private Vector3 savedOrigin;
    //public int offsetX;
    //public int offsetY;
    //public int offsetZ;

    // Start is called before the first frame update
    void Start()
    {
        serializerSetting.Formatting = Formatting.Indented;
        serializerSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        ShaderCheck = stones.GetComponent<ShaderControl>();
        layermask = 1 << LayerMask.NameToLayer("Stone");

        InstantiateRays();
        CastFloor(); // Cast includes the code to determine the height of each pin.
        InstantiatePins();

        if(connectHW) HWserial.play();


        if (dynamicEnvironment) StartCoroutine("UpdateFloor");


    }


    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator UpdateFloor()
    {
        while(true)
        {
            board_data_list.Clear();
            InstantiateRays();
            CastFloor();
            UpdatePins();
            if (connectHW) HWserial.play();

            if (debugLog) Debug.Log("Recast complete.");
            yield return new WaitForSeconds(seconsPerFrame);
        }
    }

    void InstantiateRays()
    {
        pinArrayPositions = new List<Vector3>();
        ConvertUnits();
        for (int i= 0; i< pinRowNum; i++)
        {
            for( int j = 0; j< pinColNum;j++)
            {
                Vector3 position = transform.position + pinW_M * (j * transform.localScale.z * transform.forward + i * transform.localScale.x * transform.right); // times scale and direction
                pinArrayPositions.Add(position);
            }
        }

        int numPins = pinRowNum * pinColNum;
        rayHitPoints = new Vector3[numPins];
    }

    void ConvertUnits()
    {
        pinW_M = pinW_mm / 1000.0f;
        pinH_M = pinH_mm / 1000.0f;
        maskPadding_M = maskPadding_mm / 1000.0f;
    }

    // for testing and debuggin
    public void generateRandomHeight(int seedNum, int[,] height)
    {
        Random.seed = seedNum;
        //for (int i = 0; i < pinColNum; i++) for (int j = 0; j < pinRowNum; j++) height[i, j] = Random.Range(minHeight, maxHeight + 1);
    }

    private float rayRange_low = Mathf.Infinity;
    private float rayRange_high = 0.0f;

    void CastFloor()
    {
        int numPins = pinRowNum * pinColNum;

        for(int i = 0; i < numPins; i++)
        {
            RaycastHit hit;

            // skip pins/rows that are masked
            // if (maskTargetRows) & (Mask(pinArrayPositions[i]) continue;

            if(Physics.Raycast(pinArrayPositions[i],-1f*transform.up, out hit, Mathf.Infinity, layermask))
            {
                if (debugLog) Debug.Log("a hit has been made. hit distance: " + hit.distance);

                Debug.DrawRay(pinArrayPositions[i], -1f * transform.up * hit.distance, Color.yellow, layermask);
                rayHitPoints[i] = hit.point;
                rayRange_high = Mathf.Max(rayRange_high, rayHitPoints[i].y);
                rayRange_low = Mathf.Min(rayRange_low, rayHitPoints[i].y);
            }
        }

        board_data_list.Clear();

        for(int j = 0; j<pinRowNum;j++)

        {
            for(int i = 0; i<pinColNum;i++)
            {
                float worldHeight = rayHitPoints[j * pinColNum + i].y;
                float relativeHeight = worldHeight - relativeTo.position.y;
                int boardHeight = 0;

                if(adaptiveRange)
                {
                    boardHeight = (int)Mathf.Lerp(1, pinSteps, Mathf.InverseLerp(rayRange_low, rayRange_high, relativeHeight));
                }
                else
                {
                    if (clipHeight) relativeHeight = Mathf.Clamp(relativeHeight, lowestPosition, highestPosition);
                    boardHeight = (int)Mathf.Lerp(1, pinSteps, Mathf.InverseLerp(lowestPosition, highestPosition, relativeHeight));
                }

                boardHeight = (int)Mathf.Clamp(boardHeight, 0, pinSteps); // TODO check the lowest and highest pin height value in HW
                board_data_list.Add(new Pin_Data(pinColNum-(i+1), j, boardHeight));

            }
        }
        totalBoardData = new Board_Data(pinRowNum, pinColNum, board_data_list);
        string JsonBoardData = JsonConvert.SerializeObject(totalBoardData, serializerSetting);
        File.WriteAllText(Application.dataPath + JSONPath , JsonBoardData);

    }



    void InstantiatePins()
    {
        int numPins = pinRowNum * pinColNum;

        for (int i = 0; i < pinColNum; i++)
        {
            for (int j = 0; j < pinRowNum; j++)
            {
                GameObject pin_Curr = Instantiate(pinPrefab);
                int p = i + j * pinColNum;
                pin_Curr.transform.position = rayHitPoints[p];

                pin_Curr.GetComponent<PinObject>().UpdateRawPosition(rayHitPoints[p], rayRange_low, rayRange_high, pinSteps);
                pin_Curr.GetComponent<PinObject>().UpdateColRow(i, j);
                pin_Curr.transform.parent = pinParent.transform;

                pinSimulations.Add(pin_Curr);
            }
        }
    }

    void UpdatePins()
    {
        int numPins = pinRowNum * pinColNum;

        for(int i = 0; i < numPins; i++)
        {
            pinSimulations[i].transform.position = rayHitPoints[i];
            pinSimulations[i].GetComponent<PinObject>().UpdateRawPosition(rayHitPoints[i], rayRange_low, rayRange_high, pinSteps);
            pinSimulations[i].GetComponent<PinObject>().UpdateColRow(i, i%pinColNum);


        }

    }




}

