using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{   
    public GameObject grabObject;
    public Animator handleAnim;
    //public Animator doorAnim;
    private bool oneTimeTrigger = true;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            Debug.Log("PlayerGrab");
            
            Invoke("HandleDelete", 0.3f);
        }
    }
    */
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && oneTimeTrigger == true)
        {
            Debug.Log("PlayerGrab");

            handleAnim.SetTrigger("Grab");
            Invoke("HandleDelete", 1.1f);

            oneTimeTrigger = false;
        }
    }

    void HandleDelete()
    {
        grabObject.gameObject.SetActive(false);

        //doorAnim.SetTrigger("Grab");
    }
}
