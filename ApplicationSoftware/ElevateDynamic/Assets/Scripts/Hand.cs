using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    //public SteamVR_Action_Boolean m_GrabAction = null;
    //private SteamVR_Behaviour_Pose m_Pose = null;

    private FixedJoint m_joint = null;
    private Interactable m_CurrentInteractable = null;

    private List<Interactable> m_ContactInteractables = new List<Interactable>();
    // Start is called before the first frame update
    void Awake()
    {
        //m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_joint = GetComponent<FixedJoint>();

    }

    // Update is called once per frame
    void Update()
    {

        /*
        //down
        if(m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "trigger down");
            PickUp();
        }

        //up
        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "trigger up");
            Drop();
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable")) return;
        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable")) return;
        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }

    public void PickUp()
    {
    }

    public void Drop()
    {

    }

    public Interactable GetNearestInteractable()
    {
        return null;
    }
}
