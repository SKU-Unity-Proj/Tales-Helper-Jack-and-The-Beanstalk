using Cinemachine;
using UnityEngine;

public class DynamicCameraController : MonoBehaviour
{
    public CinemachineStateDrivenCamera stateDrivenCamera;
    public string defaultCameraName = "Default Follow"; // 'default' ������ Virtual Camera �̸�

    private CinemachineVirtualCamera defaultVirtualCamera;
    private Transform defaultCameraTransform;

    public float waveFrequency = 1.0f; // �ﷷ�Ÿ��� ���ļ�
    public float waveAmplitude = 0.5f; // �ﷷ�Ÿ��� ũ��

    private float originalYPosition;
    private float timer;

    void Start()
    {
        // 'default' ������ Virtual Camera�� ã�� �ʱ� Y ��ġ�� �����մϴ�.
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
            // Ÿ�̸Ӹ� ������Ʈ�մϴ�.
            timer += Time.deltaTime;

            // ���� �Լ��� ����Ͽ� Y �࿡ ���� �ﷷ�Ÿ��� ����մϴ�.
            float newYPosition = originalYPosition + Mathf.Sin(timer * waveFrequency) * waveAmplitude;

            // ī�޶��� ���� ��ġ�� ������Ʈ�մϴ�.
            Vector3 newLocalPosition = defaultCameraTransform.localPosition;
            newLocalPosition.y = newYPosition;
            defaultCameraTransform.localPosition = newLocalPosition;
        }
        else
        {
            // �ٸ� ī�޶� Ȱ��ȭ �Ǹ� Ÿ�̸Ӹ� �����մϴ�.
            timer = 0;
        }
    }
}
