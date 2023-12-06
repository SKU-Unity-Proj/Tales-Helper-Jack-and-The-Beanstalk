using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBox : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Collider>().CompareTag("BlockBox"))
        {
            Debug.Log("Box");
            anim.SetTrigger("Tripping");
        }
    }
}
