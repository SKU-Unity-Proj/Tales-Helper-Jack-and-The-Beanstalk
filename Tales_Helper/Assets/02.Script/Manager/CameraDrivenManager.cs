using UnityEngine;
using Cinemachine;
using DiasGames.Controller;

public class CameraDrivenManager : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    public CinemachineStateDrivenCamera stateDrivenCamera;

    public string[] availableTags; // ��� ������ �±׵��� �迭
    public int selectedTagIndex = 0; // �ν����Ϳ��� ���õ� �±��� �ε���. �⺻���� 0

    private string selectedTag; // ���� ���� ���õ� �±�

    private void Start()
    {
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();

        // �ε����� ��ȿ�ϸ� ���õ� �±׸� ����
        if (selectedTagIndex >= 0 && selectedTagIndex < availableTags.Length)
        {
            selectedTag = availableTags[selectedTagIndex];
        }
        else
        {
            Debug.LogError("Selected tag index is out of range. Please check the availableTags array.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���õ� �±׸� ���� �ݶ��̴��� �� ������Ʈ Ȯ��
        if (other.gameObject.name == "CS Character Controller")
        {
            Debug.Log("Player entered: " + selectedTag);

            if (stateDrivenCamera != null)
                stateDrivenCamera.enabled = false;

            Debug.Log("ggod");
            // ����: ù ��° ī�޶�� ��ȯ
            SwitchToCamera(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ������ �±׸� ���� �ݶ��̴��� ���� ������Ʈ�� �÷��̾����� Ȯ��
        if (other.tag == selectedTag  && other.gameObject.name == "CS Character Controller")
        {
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
