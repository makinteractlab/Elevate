using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinObject_J : MonoBehaviour
{


    public int col;
    public int row;
    public int step_val;

    public Pin_Data information;
    public float step_height;
    public Vector3 startPosition;

    [HideInInspector]
    Transform PinParent;
    //public void Start()
    //{
    //    PinParent = GameObject.Find("pinParents").transform;
    //    transform.parent = PinParent;
    //}

    public void UpdateColRow(int c, int r)
    {
        col = c;
        row = r;

    }

    public void UpdateRawPosition(Vector3 position, float low, float high, int pinSteps)
    {
        float yVal = position.y;
        float t = Mathf.InverseLerp(low, high, yVal);
        float rawRemap_y = Mathf.Lerp(0.0f, (float)pinSteps, t);
        //Debug.Log("Hmmm " + rawRemap_y.ToString());
        int stepVal = (int)rawRemap_y;
        step_val = stepVal;
        transform.position = new Vector3(position.x, stepVal * 0.15f / (float)pinSteps, position.z);

    }

    public void UpdatePosition()
    {
        transform.position = new Vector3(startPosition.x, startPosition.y + information.step_val * step_height, startPosition.z);
       
    }

    public void UpdateStep(int step)
    {
        information.step_val = step;
        UpdatePosition();
    }

    public void Instantiate(Pin_Data currPin, Vector3 start, float stepHeight)
    {
        information = currPin;
        startPosition = start;
        step_height = stepHeight;
        UpdatePosition();
    }

}
