using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    // 카메라 목록을 저장하는 정적 리스트
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    // 현재 활성화된 카메라를 저장하는 정적 필드
    static CinemachineVirtualCamera activeCamera = null;

    // 현재 활성화된 카메라가 매개변수로 전달된 카메라인지 확인하는 정적 메소드
    public static bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return camera == activeCamera;
    }

    /*
    // 새 카메라로 전환하는 정적 메소드
    public static void SwitchCamera(CinemachineVirtualCamera newCamera)
    {
        if (activeCamera != null)
        {
            // 이전 카메라의 우선순위를 낮춤
            activeCamera.Priority = 0;
        }

        // 새 카메라를 활성화하고 우선순위를 높임
        activeCamera = newCamera;
        activeCamera.Priority = 11;
    }
    */

    // SwitchCamera 메소드를 CameraZone을 받도록 수정
    // 새 카메라로 전환하는 정적 메소드
  

    // 새로운 카메라를 등록하는 정적 메소드
    public static void Register(CinemachineVirtualCamera camera)
    {
        if (!cameras.Contains(camera))
        {
            cameras.Add(camera);
            Debug.Log("Camera registered: " + camera);
        }

    }

    // 등록된 카메라를 제거하는 정적 메소드
    public static void UnRegister(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
        Debug.Log("Camera unregistered: " + camera);
    }
}
