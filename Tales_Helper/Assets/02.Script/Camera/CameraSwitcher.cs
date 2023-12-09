using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    static CinemachineVirtualCamera activeCamera = null;

    public static bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return camera == activeCamera;
    }

    public static void SwitchCamera(CinemachineVirtualCamera newCamera)
    {
        if (activeCamera != null)
        {
            activeCamera.Priority = 0;
        }

        activeCamera = newCamera;
        activeCamera.Priority = 10;
    }
    /*
    public static void DisSwichCamera(CinemachineVirtualCamera camera)
    {
        if (ActiveCamera == camera)
        {
            camera.Priority = 0;
            if (previousActiveCamera != null)
            {
                ActiveCamera = previousActiveCamera;
                previousActiveCamera.Priority = 10;
            }
        }
    }
    */
    public static void Register(CinemachineVirtualCamera camera)
    {
        if (!cameras.Contains(camera))
        {
            cameras.Add(camera);
            Debug.Log("Camera registered: " + camera);
        }
        
    }
    public static void UnRegister(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
        Debug.Log("Camera unregistered: " + camera);
    }
}
