using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    // ī�޶� ����� �����ϴ� ���� ����Ʈ
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    // ���� Ȱ��ȭ�� ī�޶� �����ϴ� ���� �ʵ�
    static CinemachineVirtualCamera activeCamera = null;

    // ���� Ȱ��ȭ�� ī�޶� �Ű������� ���޵� ī�޶����� Ȯ���ϴ� ���� �޼ҵ�
    public static bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return camera == activeCamera;
    }

    /*
    // �� ī�޶�� ��ȯ�ϴ� ���� �޼ҵ�
    public static void SwitchCamera(CinemachineVirtualCamera newCamera)
    {
        if (activeCamera != null)
        {
            // ���� ī�޶��� �켱������ ����
            activeCamera.Priority = 0;
        }

        // �� ī�޶� Ȱ��ȭ�ϰ� �켱������ ����
        activeCamera = newCamera;
        activeCamera.Priority = 11;
    }
    */

    // SwitchCamera �޼ҵ带 CameraZone�� �޵��� ����
    // �� ī�޶�� ��ȯ�ϴ� ���� �޼ҵ�
  

    // ���ο� ī�޶� ����ϴ� ���� �޼ҵ�
    public static void Register(CinemachineVirtualCamera camera)
    {
        if (!cameras.Contains(camera))
        {
            cameras.Add(camera);
            Debug.Log("Camera registered: " + camera);
        }

    }

    // ��ϵ� ī�޶� �����ϴ� ���� �޼ҵ�
    public static void UnRegister(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
        Debug.Log("Camera unregistered: " + camera);
    }
}
