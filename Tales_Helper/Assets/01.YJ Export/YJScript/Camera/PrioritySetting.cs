using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PrioritySetting : MonoBehaviour
{
    //public Camera firstPersonCamera;
    public CinemachineVirtualCamera vCam;
    public CinemachineMixingCamera vCam2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //firstPersonCamera.enabled = false;
            vCam.MoveToTopOfPrioritySubqueue();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            //firstPersonCamera.enabled = true;
            vCam2.MoveToTopOfPrioritySubqueue();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        vCam2.MoveToTopOfPrioritySubqueue();
    }
    void OnTriggerExit(Collider col)
    {
        vCam.MoveToTopOfPrioritySubqueue();
    }
}
