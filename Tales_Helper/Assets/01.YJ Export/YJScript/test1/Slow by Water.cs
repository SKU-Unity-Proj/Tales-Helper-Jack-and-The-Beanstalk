using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowbyWater : MonoBehaviour
{
    public float animSpeed = 0.5f;
    public Animator anim;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Water")
        {
            anim.SetFloat("WaterInside", animSpeed);
        }
    }
}
