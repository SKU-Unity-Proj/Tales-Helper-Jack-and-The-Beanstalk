using UnityEngine;
using Cinemachine;
using DiasGames.Controller;

public class CameraDrivenManager : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    public CinemachineStateDrivenCamera stateDrivenCamera; // State Driven Camera ����

    private void Start()
    {
        // CinemachineStateDrivenCamera ������Ʈ�� ã�Ƽ� �����մϴ�.
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 'entrance' �±׸� ���� �ݶ��̴��� �� ������Ʈ�� �÷��̾����� Ȯ��
        if (other.gameObject.name == "CS Character Controller")
        {
            Debug.Log("Player entered entrance");

            // CinemachineStateDrivenCamera�� ��Ȱ��ȭ�մϴ�.
            if (stateDrivenCamera != null)
                stateDrivenCamera.enabled = false;

            // ����� ī�޶� ��ȯ ����
            SwitchToCamera(0); // ���� ��� ù ��° ī�޶�� ��ȯ
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 'entrance' �±׸� ���� �ݶ��̴��� ���� ������Ʈ�� �÷��̾����� Ȯ��
        if (other.gameObject.name == "CS Character Controller")
        {
            // CinemachineStateDrivenCamera�� �ٽ� Ȱ��ȭ�մϴ�.
            if (stateDrivenCamera != null)
                stateDrivenCamera.enabled = true;
        }
    }

    void SwitchToCamera(int cameraIndex)
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].gameObject.SetActive(i == cameraIndex);
        }
        // ���� Ȱ��ȭ�� ī�޶��� �̸� �Ǵ� �ε����� �α׷� ���
        if (cameraIndex >= 0 && cameraIndex < virtualCameras.Length)
        {
            Debug.Log("Switched to camera: " + virtualCameras[cameraIndex].name);
        }
        else
        {
            Debug.Log("Invalid camera index: " + cameraIndex);
        }
    }
}
