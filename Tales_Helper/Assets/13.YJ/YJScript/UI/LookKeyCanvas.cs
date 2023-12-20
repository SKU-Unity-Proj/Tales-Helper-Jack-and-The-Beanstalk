using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookKeyCanvas : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}
