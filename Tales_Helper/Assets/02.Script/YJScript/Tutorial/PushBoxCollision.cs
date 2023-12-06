using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBoxCollision : MonoBehaviour
{
    private Rigidbody rigid;
    public float pushPower;
    public float speed;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(7.2f, 0f, -3.48f), Time.deltaTime * speed);
    }

    public void AddForceBox()
    {
        //rigid.AddForce(Vector3.left * pushPower);
    }
}
