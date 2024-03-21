using Cinemachine;
using UnityEngine;

public class DynamicCameraController : MonoBehaviour
{
    public CinemachineStateDrivenCamera stateDrivenCamera;
    public string defaultCameraName = "Default Follow"; // 'default' ������ Virtual Camera �̸�

    private CinemachineVirtualCamera currentVirtualCamera;

    public float waveFrequency = 1.0f; // �ﷷ�Ÿ��� ���ļ�
    public float waveAmplitude = 0.5f; // �ﷷ�Ÿ��� ũ��

    private float originalYPosition;
    private float timer;

    void Start()
    {
        // �ʱ� Y ��ġ�� �����մϴ�.
        originalYPosition = stateDrivenCamera.transform.position.y;
    }
    void Update()
    {
        // ���� Ȱ��ȭ�� Virtual Camera ���
        currentVirtualCamera = stateDrivenCamera.LiveChild as CinemachineVirtualCamera;

        // 'default' ������ Virtual Camera���� Ȯ��
        if (currentVirtualCamera != null && currentVirtualCamera.Name == defaultCameraName)
        {
            // Ÿ�̸Ӹ� ������Ʈ�մϴ�.
            timer += Time.deltaTime;

            // ���� �Լ��� ����Ͽ� Y �࿡ ���� �ﷷ�Ÿ��� ����մϴ�.
            float newYPosition = originalYPosition + Mathf.Sin(timer * waveFrequency) * waveAmplitude;

            // ī�޶��� ��ġ�� ������Ʈ�մϴ�.
            Vector3 newPosition = stateDrivenCamera.transform.position;
            newPosition.y = newYPosition;
            stateDrivenCamera.transform.position = newPosition;
        }
        else
        {
            // �ٸ� ���¿����� Ÿ�̸Ӹ� �����մϴ�.
            timer = 0;
        }
    }
}