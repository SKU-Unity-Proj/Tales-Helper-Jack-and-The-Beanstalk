using Cinemachine;
using UnityEngine;

public class entranceStartDriven : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public float targetFOV = 62f; // ��ǥ FOV ��
    public Vector3 newOffset; // ���ο� ������ ��

    public float camSpeed = 5f; // ������ ���� �ӵ�
    public float fovSpeed = 0.5f; // FOV ���� �ӵ�

    private CinemachineFramingTransposer framingTransposer;

    void Start()
    {
        // Virtual Camera�� Body ������Ʈ���� Framing Transposer�� �����ɴϴ�.
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update()
    {
        // ���� FOV���� ��ǥ FOV���� ���������� ����
        if (virtualCamera.m_Lens.FieldOfView < targetFOV)
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.MoveTowards(virtualCamera.m_Lens.FieldOfView, targetFOV, fovSpeed * Time.deltaTime);
        }

        if (framingTransposer != null)
        {
            // Tracked Object Offset�� �ε巴�� ���ο� ������ ������ �����մϴ�.
            framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(framingTransposer.m_TrackedObjectOffset, newOffset, camSpeed * Time.deltaTime);
        }
    }
}
