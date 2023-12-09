using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))] // BoxCollider ������Ʈ�� �ʿ����� ���
[RequireComponent(typeof(Rigidbody))] // Rigidbody ������Ʈ�� �ʿ����� ���
public class CameraTriggerManager : MonoBehaviour
{
    // ī�޶� ���� �����ϴ� Ŭ����
    [System.Serializable]
    public class CameraZone
    {
        public CinemachineVirtualCamera camera; // ī�޶� ���� ����� CinemachineVirtualCamera
        public Vector3 position; // ī�޶� ���� ��ġ
        public Vector3 boxsize; // ī�޶� ���� ũ��
        public GameObject zoneObject; // ī�޶� ���� ��Ÿ���� GameObject
    }

    [SerializeField] private CameraZone[] cameraZones; // ���� ī�޶� ���� �����ϴ� �迭

    public CinemachineStateDrivenCamera stateDrivenCamera; // ���� ��� Cinemachine ī�޶�

    private void Awake()
    {
        // ���� ��� Cinemachine ī�޶� ã��
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
    }

    private void Start()
    {
        // ī�޶� �� �ʱ�ȭ
        foreach (var zone in cameraZones)
        {
            // ���ο� GameObject ���� �� ����
            GameObject zoneObj = new GameObject(zone.camera.name + "_Zone");
            zoneObj.transform.position = zone.position;
            zoneObj.transform.parent = this.transform;

            // BoxCollider �߰� �� ����
            BoxCollider collider = zoneObj.AddComponent<BoxCollider>();
            collider.size = zone.boxsize;
            collider.isTrigger = true; // Trigger�� ����

            zone.zoneObject = zoneObj; // zoneObject�� ������ GameObject �Ҵ�

            CameraSwitcher.Register(zone.camera); // ī�޶� ���
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ī�޶� ���� ���� �� ó��
        if (other.CompareTag("Player"))
        {
            foreach (var zone in cameraZones)
            {
                // �÷��̾ �ش� �� ���ο� �ִ��� Ȯ��
                if (zone.zoneObject.GetComponent<Collider>().bounds.Contains(other.transform.position))
                {
                    if (stateDrivenCamera != null)
                        stateDrivenCamera.enabled = false; // ���� ��� ī�޶� ��Ȱ��ȭ

                    CameraSwitcher.SwitchCamera(zone.camera); // ī�޶� ��ȯ
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ ī�޶� ���� ����� �� ó��
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (var zone in cameraZones)
            {
                //������� Ȯ��
                if (other.gameObject.CompareTag("Player") == zone.zoneObject)
                {
                    if (stateDrivenCamera != null)
                        stateDrivenCamera.enabled = true; // ���� ��� ī�޶� Ȱ��ȭ

                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // �����Ϳ��� ī�޶� ���� ��踦 �ð�ȭ
        foreach (var zone in cameraZones)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(zone.position, zone.boxsize);
        }
    }
}
