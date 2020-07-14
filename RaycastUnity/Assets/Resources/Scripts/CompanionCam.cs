using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionCam : MonoBehaviour
{
    public Transform target;
    public float smoothingValue = 0.01f;
    private Vector3 moveVel;
    private Vector3 rotVel;

    // Start is called before the first frame update
    void Awake()
    {
        moveVel = Vector3.zero;
        rotVel = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.fixedDeltaTime;

        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref moveVel, smoothingValue, Mathf.Infinity, deltaTime);

        Vector3 eulerAngles = transform.rotation.eulerAngles;
        Vector3 targetEulerAngles = target.eulerAngles;
        eulerAngles.x = Mathf.SmoothDampAngle(eulerAngles.x, targetEulerAngles.x, ref rotVel.x, smoothingValue, Mathf.Infinity, deltaTime);
        eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, targetEulerAngles.y, ref rotVel.y, smoothingValue, Mathf.Infinity, deltaTime);
        eulerAngles.z = Mathf.SmoothDampAngle(eulerAngles.z, targetEulerAngles.z, ref rotVel.z, smoothingValue, Mathf.Infinity, deltaTime);
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
