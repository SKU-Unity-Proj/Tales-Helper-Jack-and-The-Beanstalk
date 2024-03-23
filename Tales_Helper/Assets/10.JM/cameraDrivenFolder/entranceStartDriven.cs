using Cinemachine;
using UnityEngine;

public class entranceStartDriven : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public float targetFOV = 62f; // 목표 FOV 값
    public Vector3 newOffset; // 새로운 오프셋 값

    public float camSpeed = 5f; // 오프셋 변경 속도
    public float fovSpeed = 0.5f; // FOV 조절 속도

    private CinemachineFramingTransposer framingTransposer;

    void Start()
    {
        // Virtual Camera의 Body 컴포넌트에서 Framing Transposer를 가져옵니다.
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update()
    {
        // 현재 FOV에서 목표 FOV까지 점진적으로 변경
        if (virtualCamera.m_Lens.FieldOfView < targetFOV)
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.MoveTowards(virtualCamera.m_Lens.FieldOfView, targetFOV, fovSpeed * Time.deltaTime);
        }

        if (framingTransposer != null)
        {
            // Tracked Object Offset을 부드럽게 새로운 오프셋 값으로 변경합니다.
            framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(framingTransposer.m_TrackedObjectOffset, newOffset, camSpeed * Time.deltaTime);
        }
    }
}
