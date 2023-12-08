using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Raser : MonoBehaviour
{
    public Transform StartPos;
    public float speed = 2;

    public float leftMax = 1.0f;
    public float rightMax = 11.0f;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == ("CS Character Controller"))
        {
            col.transform.position = StartPos.position;
            //col.transform.Translate(new Vector3(0, 0, 0), Space.World);
            //col.transform.position = new Vector3(-224f, 0f, 0.5f);
            Debug.Log("Player");
        }
    }

    void Update()
    {
        if (transform.position.z <= leftMax)
        {
            speed *= -1;
        }
        else if (transform.position.z >= rightMax)
        {
            speed *= -1;
        }
        transform.Translate(Vector3.forward * 1f * Time.deltaTime * speed);
    }
}
