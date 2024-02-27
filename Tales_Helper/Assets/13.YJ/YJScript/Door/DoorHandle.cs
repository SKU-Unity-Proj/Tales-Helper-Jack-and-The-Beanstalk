using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{   
    public Animator doorAnim;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            doorAnim.SetTrigger("Grab");

            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
