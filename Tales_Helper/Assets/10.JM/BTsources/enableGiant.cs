using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableGiant : MonoBehaviour
{
    public GameObject cellerGiant;

    public void OnClickDownPriority()
    {
        cellerGiant.SetActive(true);
    }

}
