using UnityEngine;
using Cinemachine;

public class FrameJumpSwapCamera : MonoBehaviour
{
    public CinemachineVirtualCamera swapCam; // FrameJumpCam

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Entered: " + other.name);
        swapCam.Priority = 13;
    }

    private void OnTriggerExit(Collider other)
    {
        swapCam.Priority = 1;
    }
}
