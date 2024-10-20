using UnityEngine;
using Cinemachine;

public class CamLowPriority : MonoBehaviour
{
    public CinemachineBrain cinemachineBrain;

    public void OnClickDownPriority()
    {
        if (cinemachineBrain != null && cinemachineBrain.ActiveVirtualCamera != null)
        {
            CinemachineVirtualCamera activeCamera = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (activeCamera != null)
            {
                activeCamera.Priority = 0;
            }
        }
    }
}
