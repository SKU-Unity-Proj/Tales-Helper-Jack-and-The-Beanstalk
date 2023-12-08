using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_AxeRotation : MonoBehaviour
{
    public Transform StartPos;
    public float rotationSpeed;
    /*
    public float angleRotate = 1f;
    public float leftMax = -82.0f;
    public float rightMax = 82.0f;

    void Update()
    {
        if (transform.rotation.z < leftMax)
        {
            angleRotate *= -1;
        }
        else if (transform.rotation.z > rightMax)
        {
            angleRotate *= -1;
        }
        //transform.Rotate(Vector3.up * Time.deltaTime * angleRotate);
        transform.Rotate(0,0,angleRotate);
    }
    */
    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
    }

    void OnTriggerEnter(Collider col)
    {
            col.transform.position = StartPos.position;
            Debug.Log("Player");
    }

}
