using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour
{

    public List<GameObject> electricItems = new List<GameObject>();
    private bool lState = true;
    public bool lightState
    {
        get
        {
            return lState;
        }
        set
        {
            lState = value;
            Lights(lState);
        }
    }

    public Transform Door;
    public Animator building;
    public float timeUntilKnockDown = 80.0f; //seconds
    private bool elecCut = false;
    private float timeStart;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            elecCut = !elecCut;
        }

        if (elecCut) {
            building.SetTrigger("Flick");
            elecCut = false;
            Door.GetComponent<Valve.VR.InteractionSystem.CircularDrive>().outAngle = 80f;
            Door.GetComponent<Valve.VR.InteractionSystem.CircularDrive>().UpdateLinearMapping();
            Door.localRotation = Quaternion.Euler(0f, 80f, 0f);
        }
    }

    public void LightOff()
    {
        foreach (GameObject g in electricItems)
        {
            g.SetActive(false);
        }
    }

    public void LightOn()
    {
        foreach (GameObject g in electricItems)
        {
            g.SetActive(true);
        }
    }

    public void Lights(bool state)
    {
        if (state)
        {
            building.SetTrigger("On");
        }
        else
        {
            building.SetTrigger("Off");
        }
    }
}
