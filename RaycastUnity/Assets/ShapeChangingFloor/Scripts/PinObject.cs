using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinObject : MonoBehaviour
{
    public Pin_Data information;
    public float step_height;

    public Vector3 startPosition;

    void Start()
    {
        //transform.localScale = new Vector3(0.03f, 0.15f, 0.03f);
        //transform.localPosition = new Vector3(transform.localPosition.x, 0.015f * step_val / 2f, transform.localPosition.z);
    }

    void Update()
    {
    }

    public void Instantiate(Pin_Data currPin, Vector3 start, float stepHeight)
    {
        information = currPin;
        startPosition = start;
        step_height = stepHeight;
        UpdatePosition();
    }

    public void UpdateStep(int step)
    {
        information.step_val = step;
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        transform.position = new Vector3(startPosition.x, startPosition.y + information.step_val * step_height, startPosition.z);
    }
    
}
