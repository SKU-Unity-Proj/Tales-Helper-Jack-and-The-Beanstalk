using UnityEngine;
using Cinemachine;
using DiasGames.Controller;

public class CameraDrivenManager : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    public CinemachineStateDrivenCamera stateDrivenCamera;

    public string[] availableTags; // 사용 가능한 태그들의 배열
    public int selectedTagIndex = 0; // 인스펙터에서 선택된 태그의 인덱스. 기본값은 0

    private string selectedTag; // 실제 사용될 선택된 태그

    private void Start()
    {
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();

        // 인덱스가 유효하면 선택된 태그를 설정
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
        // 선택된 태그를 가진 콜라이더에 들어간 오브젝트 확인
        if (other.gameObject.name == "CS Character Controller")
        {
            Debug.Log("Player entered: " + selectedTag);

            if (stateDrivenCamera != null)
                stateDrivenCamera.enabled = false;

            Debug.Log("ggod");
            // 예시: 첫 번째 카메라로 전환
            SwitchToCamera(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 설정된 태그를 가진 콜라이더를 떠난 오브젝트가 플레이어인지 확인
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
        // 현재 활성화된 카메라의 이름 또는 인덱스를 로그로 출력
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
