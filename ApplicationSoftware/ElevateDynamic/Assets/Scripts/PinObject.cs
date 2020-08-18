using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinObject : MonoBehaviour
{
    public int col;
    public int step_val;
    public int row;

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
        int stepVal = (int)rawRemap_y;
        step_val = stepVal;

        transform.position = new Vector3(position.x, stepVal * 0.15f / (float)pinSteps, position.z);
    }
}
