using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QE : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            anim.SetTrigger("Looking Left");

        if (Input.GetKeyDown(KeyCode.E))
            anim.SetTrigger("Looking Right");
    }
}
