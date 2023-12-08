using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpWaterCollider : MonoBehaviour
{
    public Transform startPos;

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            col.transform.position = startPos.position;
            Debug.Log("water");
        }
    }
}
