using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBoxAction : MonoBehaviour
{
    public GameObject call;

    void Start()
    {
        //GameObject call = GameObject.Find("Push Box");
    }

    void OnTriggerStay()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Push");
            //call.GetComponent<PushBoxCollision>().AddForceBox();
            call.GetComponent<PushBoxCollision>().enabled = true;
        }
    }
}
