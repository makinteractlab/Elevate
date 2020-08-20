using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public Hand m_ActiveHand = null;

    public Material greenLight;
    public Material defaultMat;
    public float speed;
    public float targetHeight=21;
    private Transform defaultT;

    private bool liftUp = false;
    private Vector3 touchPoint;
    public WriteMatrix pinManager;
    private void Start()
    {
        //save rotational value;
        defaultT = transform;
        transform.GetComponent<Renderer>().material = defaultMat;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "collisionPad")
        {
            //Debug.Log("point of collision: " + collision.contacts[0].point);
            touchPoint = collision.contacts[0].point;
            ShadowStone(touchPoint);
            liftUp = true;


        }
    }

    private void Update()
    {
        if(liftUp)
        {
            Destroy(transform.GetComponent<Rigidbody>());
            //transform.GetComponent<Renderer>().enabled=false;
            transform.rotation = defaultT.rotation;

            LiftStone(targetHeight, touchPoint);
        }
    }
    private void LiftStone(float targetH, Vector3 collisionPoint)
    {

        if (transform.position.y <= targetH)
        {
            transform.position = new Vector3(collisionPoint.x, transform.position.y + (Time.deltaTime * speed), collisionPoint.z);
            
        }
        else
        {
            transform.GetComponent<Renderer>().material = greenLight;
            liftUp = false;
        }
        
    }

    private void ShadowStone(Vector3 collisionPoint) //TODO for play matrix to catch the expected pins so that we can push the play before it rises up
    {

        GameObject shadowStone = Instantiate(gameObject, new Vector3(collisionPoint.x,targetHeight, collisionPoint.z),Quaternion.identity);
        shadowStone.GetComponent<Interactable>().enabled = false;
        Destroy(shadowStone.GetComponent<Interactable>());

        Destroy(shadowStone.GetComponent<Renderer>());
        Destroy(shadowStone.GetComponent<Rigidbody>());

        //TODO call play script;
        pinManager.pushPlayScript = true;


    }
}
