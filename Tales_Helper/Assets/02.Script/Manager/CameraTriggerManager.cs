using UnityEngine;
using Cinemachine;
using System.Linq; // LINQ ���

/// <summary>
/// CameraTriggerManager�� ���� ī�޶� ���� �����ϰ�,
/// �� ���� �����ϰų� ���� �� ī�޶� ��ȯ�� ó���ϴ� Ŭ�����̴�.
/// LINQ�� ����Ͽ� Ư�� Collider�� ����� CameraZone�� ȿ�������� ã�� �� ����.
/// LINQ�� ����ϸ� �迭, ����Ʈ, XML, �����ͺ��̽�, �� �� �پ��� ������ �ҽ��� ���� ���ϵ� ������� ������ �ۼ��� �� �ִ�.
/// </summary>
public class CameraTriggerManager : MonoBehaviour
{
    // CameraZone Ŭ������ ����. �� Ŭ������ �� ī�޶� ���� ������ ����.
    [System.Serializable]
    public class CameraZone
    {
        public CinemachineVirtualCamera camera; // ī�޶� ���� ���� Cinemachine ���� ī�޶�
        public Vector3 position; // ī�޶� ���� ��ġ
        public Vector3 boxsize; // ī�޶� ���� ũ��
        public GameObject zoneObject; // ī�޶� ���� ��Ÿ���� ���� ������Ʈ
    }

    [SerializeField] private CameraZone[] cameraZones; // �����Ϳ��� ������ �� �ִ� ī�޶� �� �迭
    public CinemachineStateDrivenCamera stateDrivenCamera; // ���� ��� Cinemachine ī�޶�
    private CinemachineVirtualCamera activeCamera = null; // ���� Ȱ��ȭ�� ���� ī�޶�

    private void Awake()
    {
        // ���� ���� ��, CinemachineStateDrivenCamera ������Ʈ�� ã�Ƽ� �Ҵ�
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
    }

    private void Start()
    {
        int zoneIndex = 0;
        foreach (var zone in cameraZones)
        {
            // �� ī�޶� ���� ���� ���� ������Ʈ�� �����ϰ� ������
            string zoneObjectName = zone.camera.name + "_Zone_" + zoneIndex;
            GameObject zoneObj = new GameObject(zoneObjectName);
            zoneObj.transform.position = zone.position;
            zoneObj.transform.parent = this.transform;

            // BoxCollider ������Ʈ�� �߰��ϰ� ũ�� �� Ʈ���� ������
            BoxCollider collider = zoneObj.AddComponent<BoxCollider>();
            collider.size = zone.boxsize;
            collider.isTrigger = true;

            // ZoneIndex ������Ʈ�� �߰��Ͽ� �� ���� �ĺ��� �� �ִ� �ε����� ������
            ZoneIndex zIdx = zoneObj.AddComponent<ZoneIndex>();
            zIdx.index = zoneIndex;

            // ������ ���� ������Ʈ�� CameraZone�� zoneObject�� ������
            zone.zoneObject = zoneObj;
            Debug.Log(zoneObj + "," + zoneIndex);

            // �� ī�޶��� �켱 ������ �ʱ�ȭ
            activeCamera = zone.camera;
            activeCamera.Priority = 0;
            zoneIndex++;
        }
    }

    #region Ʈ���� ����(���߿� ������ �� ������)
    /*
    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ �ش� ���� �ݶ��̴� ���� �ִ��� Ȯ��
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (var zone in cameraZones)
            {
                Collider zoneCollider = zone.zoneObject.GetComponent<Collider>();
                Debug.Log(cameraZones.Length);
                Debug.Log(zone.camera.name);

                ZoneIndex zIdx = zone.zoneObject.GetComponent<ZoneIndex>();
                int zoneIndex = zIdx.index;
                Debug.Log($" in {zoneIndex}");

                if (zoneCollider)
                {
                    
                    if (zoneIndex != -1)
                    {
                        Debug.Log("Entered Zone Index: " + zoneIndex);

                        activeCamera = cameraZones[zoneIndex].camera;
                        activeCamera.Priority = 13;
                    }
               
                }          
            }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (var zone in cameraZones)
            {
                Collider zoneCollider = zone.zoneObject.GetComponent<Collider>();

                if (other.gameObject.CompareTag("Player") == zoneCollider)
                {
                    ZoneIndex zIdx = zone.zoneObject.GetComponent<ZoneIndex>();
                    int zoneIndex = zIdx.index;
                    Debug.Log($" out {zoneIndex}");
                    if (zoneIndex != -1)
                    {
                        Debug.Log("Exit Zone Index: " + zoneIndex);

                        activeCamera = cameraZones[zoneIndex].camera;
                        activeCamera.Priority = 0;
                    }

                }

            }
        }
    }
    */
    #endregion


    /// <summary>
    /// �־��� Collider�� ����� CameraZone�� ã�´�.
    /// LINQ�� FirstOrDefault�� ����Ͽ� ȿ�������� �ش� ������ �����ϴ� ù ��° ��Ҹ� ��ȯ��.
    /// </summary>
    /// <param name="collider">ã���� �ϴ� Collider</param>
    /// <returns>Collider�� ����� CameraZone, ������ null ��ȯ</returns>
    public CameraZone GetCameraZoneFromCollider(Collider collider)
    {
        // LINQ�� ����Ͽ� collider�� ����� CameraZone�� ã�� ��ȯ
        return cameraZones.FirstOrDefault(zone => zone.zoneObject.GetComponent<Collider>() == collider);
    }

    // ����Ƽ �������� Gizmo�� ����Ͽ� ī�޶� ���� ��ġ�� ũ�⸦ �ð�ȭ
    private void OnDrawGizmos()
    {
        foreach (var zone in cameraZones)
        {
            Gizmos.color = Color.green; // Gizmo�� ������ ������� ����
            Gizmos.DrawWireCube(zone.position, zone.boxsize); // �� ī�޶� ���� ���̾� ������ ť�긦 �׸�
        }
    }
}



