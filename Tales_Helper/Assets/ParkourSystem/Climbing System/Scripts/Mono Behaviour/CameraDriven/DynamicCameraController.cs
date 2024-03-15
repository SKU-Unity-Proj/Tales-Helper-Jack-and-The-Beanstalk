using Cinemachine;
using UnityEngine;

public class DynamicCameraController : MonoBehaviour
{
    public CinemachineStateDrivenCamera stateDrivenCamera;
    public string defaultCameraName = "Default Follow"; // 'default' 상태의 Virtual Camera 이름

    private CinemachineVirtualCamera defaultVirtualCamera;
    private Transform defaultCameraTransform;

    public float waveFrequency = 1.0f; // 울렁거림의 주파수
    public float waveAmplitude = 0.5f; // 울렁거림의 크기

    private float originalYPosition;
    private float timer;

    void Start()
    {
        // 'default' 상태의 Virtual Camera를 찾고 초기 Y 위치를 저장합니다.
        foreach (var vcam in stateDrivenCamera.ChildCameras)
        {
            if (vcam.Name == defaultCameraName)
            {
                defaultVirtualCamera = vcam as CinemachineVirtualCamera;
                if (defaultVirtualCamera != null)
                {
                    defaultCameraTransform = defaultVirtualCamera.transform;
                    originalYPosition = defaultCameraTransform.localPosition.y;
                }
                break;
            }
        }
    }

    void Update()
    {
        if (defaultVirtualCamera != null && defaultVirtualCamera.isActiveAndEnabled)
        {
            // 타이머를 업데이트합니다.
            timer += Time.deltaTime;

            // 사인 함수를 사용하여 Y 축에 대한 울렁거림을 계산합니다.
            float newYPosition = originalYPosition + Mathf.Sin(timer * waveFrequency) * waveAmplitude;

            // 카메라의 로컬 위치를 업데이트합니다.
            Vector3 newLocalPosition = defaultCameraTransform.localPosition;
            newLocalPosition.y = newYPosition;
            defaultCameraTransform.localPosition = newLocalPosition;
        }
        else
        {
            // 다른 카메라가 활성화 되면 타이머를 리셋합니다.
            timer = 0;
        }
    }
}
