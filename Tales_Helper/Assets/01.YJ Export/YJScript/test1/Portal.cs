using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private bool goHome;

    void OnTriggerEnter(Collider col)
    {
        if (goHome == false)
        {
            col.transform.position = new Vector3(224f, 0f, 0f);
            goHome = true;
        }
        else
        {
            col.transform.position = new Vector3(-224f, 0f, 0.5f);
            goHome = false;
        }
    }
}
