using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterUp : MonoBehaviour
{
    private Rigidbody rigid;
    public float upSpeed;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rigid.AddForce(Vector3.up * upSpeed * Time.deltaTime);
    }
}
