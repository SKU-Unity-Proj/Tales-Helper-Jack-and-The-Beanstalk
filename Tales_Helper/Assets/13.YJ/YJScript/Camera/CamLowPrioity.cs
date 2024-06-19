using UnityEngine;
using Cinemachine;

public class CamLowPrioity : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;

    public void OnClickDownPriority()
    {
        VirtualCamera.Priority = 0;
    }
}
