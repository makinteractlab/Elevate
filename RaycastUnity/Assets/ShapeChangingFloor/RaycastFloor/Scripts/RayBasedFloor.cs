using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class RayBasedFloor : MonoBehaviour
{
    [Header("Floor specifications")]
    public int pinsWidth = 20;
    public int pinsLength = 60;
    public int pinsSteps = 11;
    public string floorLayer = "Floor";
    private int layerMask = 1 << 8; // Limit collisions to specific mask.

    [Header("Pin measurements")]
    public float pinW_mm = 30; // in mm
    private float pinW_m = 0;
    public float pinH_mm = 150; // in mm
    private float pinH_m = 0;

    [Header("Remapping specifications")]
    public float lowestPosition = 0f;
    public float highestPosition = 2f;
    public bool adaptiveRange = false;
    public bool clipHeight = false;
    public Transform relativeTo;

    [Header("Pins")]
    public GameObject pinPrefab;
    public List<Vector3> pinArrayPositions = new List<Vector3>();
    private Vector3[] rayHitPoints;
    public List<GameObject> pinSimulations = new List<GameObject>();
    private Board_Data totalBoardData; // Full floor info.
    private List<Pin_Data> boardDataList = new List<Pin_Data>(); // each pin info in a list.
    private JsonSerializerSettings setting = new JsonSerializerSettings();

    [Header("Interactive settings")]
    public List<Transform> maskTargets;
    public bool maskTargetRows = true;
    public float maskPadding_mm = 600; // in mm
    private float maskPadding_m = 0;
    public bool dynamicEnvironment = false;
    public int framesPerMin = 4;
    private float secondsPerFrame = 0;
    public Play play;
    public Text time;

    [Header("Debug settings")]
    public bool debugLog = false;

    [Header("JSON path")]
    public string JSONPath = "/Resources/MatrixData/currentMatrix.json";

    // Start is called before the first frame update
    void Start()
    {
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        // Instantiate rays, cast, and pins.
        layerMask = 1 << LayerMask.NameToLayer(floorLayer);
        InstantiateRays();
        CastFloor(); // Cast includes the code to determine the height of each pin.
        InstantiatePins();

        play.play(); // Trigger play of the floor.

        secondsPerFrame = 60f / (float)framesPerMin;

        if (dynamicEnvironment)
            StartCoroutine("UpdateFloor");
    }

    // Update is called once per frame
    void Update()
    {
        // time.text = "time: " + play.getTimeleft() + "sec"; // What is this for?
    }

    IEnumerator UpdateFloor()
    {
        while (true)
        {
            CastFloor();
            UpdatePins();

            if(debugLog)
                Debug.Log("Recast complete.");
            yield return new WaitForSeconds(secondsPerFrame);
        }
    }

    void InstantiateRays()
    {
        pinArrayPositions = new List<Vector3>();
        ConvertUnits();

        for(int j = 0; j < pinsLength; j++)
        {
            for(int i = 0; i < pinsWidth; i++)
            {
                Vector3 position = transform.position + pinW_m * (i * transform.localScale.z * transform.forward + j * transform.localScale.x * transform.right);
                pinArrayPositions.Add(position);
            }
        }

        int numPins = pinsWidth * pinsLength;
        rayHitPoints = new Vector3[numPins];
    }

    void ConvertUnits()
    {
        pinW_m = pinW_mm / 1000.0f;
        pinH_m = pinH_mm / 1000.0f;
        maskPadding_m = maskPadding_mm / 1000.0f;
    }

    bool Mask(Vector3 position)
    {
        foreach (Transform maskTarget in maskTargets)
        {
            Vector3 currMaskPos = Vector3.Project(maskTarget.position, transform.right);
            Vector3 currTestPos = Vector3.Project(position, transform.right);

            if (Vector3.Distance(currMaskPos,currTestPos) <= maskPadding_m)
            {
                return true;
            }
        }
        return false;
    }

    private float rayRange_low = Mathf.Infinity;
    private float rayRange_high = 0.0f;
    void CastFloor()
    {
        int numPins = pinsWidth * pinsLength;

        for (int p = 0; p < numPins; p++)
        {
            RaycastHit hit;

            // Skip pins/rows that are meant to be masked
            if (maskTargetRows)
            {
                if (Mask(pinArrayPositions[p]))
                {
                    continue;
                }
            }

            if (Physics.Raycast(pinArrayPositions[p], -1.0f * transform.up, out hit, Mathf.Infinity, layerMask))
            {
                if (debugLog)
                    Debug.Log("A hit has been made.");
                Debug.DrawRay(pinArrayPositions[p], -1.0f * transform.up * hit.distance, Color.yellow, layerMask);
                rayHitPoints[p] = hit.point;
                rayRange_high = Mathf.Max(rayRange_high, rayHitPoints[p].y);
                rayRange_low = Mathf.Min(rayRange_low, rayHitPoints[p].y);
            }
        }

        boardDataList.Clear(); // Clear the old data. NOTE: This is where masking should be checked instead.

        for (int j = 0; j < pinsLength; j++)
        {
            for (int i = 0; i < pinsWidth; i++)
            {
                float worldHeight = rayHitPoints[j * pinsWidth + i].y;
                float relativeHeight = worldHeight - relativeTo.position.y;
                int boardHeight = 0;
                if (adaptiveRange)
                {
                    boardHeight = (int)Mathf.Lerp(1, pinsSteps, Mathf.InverseLerp(rayRange_low, rayRange_high, relativeHeight));
                }
                else
                {
                    if (clipHeight)
                        relativeHeight = Mathf.Clamp(relativeHeight, lowestPosition, highestPosition);
                    boardHeight = (int)Mathf.Lerp(1, pinsSteps, Mathf.InverseLerp(lowestPosition, highestPosition, relativeHeight));
                }
                boardHeight = (int)Mathf.Clamp(boardHeight, 0, pinsSteps);
                boardDataList.Add(new Pin_Data(pinsWidth-(i+1), j, boardHeight));
            }
        }
        totalBoardData = new Board_Data(pinsWidth, pinsLength, boardDataList);

        string JsonBoardData = JsonConvert.SerializeObject(totalBoardData, setting);
        File.WriteAllText(Application.dataPath + JSONPath, JsonBoardData);
    }

    void InstantiatePins()
    {
        int numPins = pinsWidth * pinsLength;

        for(int i = 0; i < pinsWidth; i++)
        {
            for (int j = 0; j < pinsLength; j++)
            {
                GameObject pin_Curr = Instantiate(pinPrefab);
                //pin_Curr.transform.position = rayHitPoints[p];
                int p = i + j * pinsWidth;
                pin_Curr.GetComponent<PinObject>().UpdateRawPosition(rayHitPoints[p], rayRange_low, rayRange_high, pinsSteps);
                pin_Curr.GetComponent<PinObject>().UpdateColRow(i, j);
                pin_Curr.transform.parent = transform;

                pinSimulations.Add(pin_Curr);
            }
        }
    }

    void UpdatePins()
    {

    }
    
}
