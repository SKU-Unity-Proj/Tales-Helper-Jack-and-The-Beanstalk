using UnityEngine;
using Cinemachine;
using DiasGames.Controller;

public class CameraDrivenManager : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    public CinemachineStateDrivenCamera stateDrivenCamera; // State Driven Camera 참조

    private void Start()
    {
        // CinemachineStateDrivenCamera 컴포넌트를 찾아서 참조합니다.
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 'entrance' 태그를 가진 콜라이더에 들어간 오브젝트가 플레이어인지 확인
        if (other.gameObject.name == "CS Character Controller")
        {
            Debug.Log("Player entered entrance");

            // CinemachineStateDrivenCamera를 비활성화합니다.
            if (stateDrivenCamera != null)
                stateDrivenCamera.enabled = false;

            // 버츄얼 카메라 전환 로직
            SwitchToCamera(0); // 예를 들어 첫 번째 카메라로 전환
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 'entrance' 태그를 가진 콜라이더를 떠난 오브젝트가 플레이어인지 확인
        if (other.gameObject.name == "CS Character Controller")
        {
            // CinemachineStateDrivenCamera를 다시 활성화합니다.
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
