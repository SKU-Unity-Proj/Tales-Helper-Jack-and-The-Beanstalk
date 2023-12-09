using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CameraTriggerManager : MonoBehaviour
{
    [System.Serializable]
    public class CameraZone
    {
        public CinemachineVirtualCamera camera;
        public Vector3 position;
        public Vector3 boxsize;
        public GameObject zoneObject; // 각 Zone의 GameObject 참조
    }

    [SerializeField] private CameraZone[] cameraZones;

    public CinemachineStateDrivenCamera stateDrivenCamera;

    private void Awake()
    {
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
    }

    private void Start()
    {
        foreach (var zone in cameraZones)
        {
            GameObject zoneObj = new GameObject(zone.camera.name + "_Zone");
            zoneObj.transform.position = zone.position;
            zoneObj.transform.parent = this.transform;

            BoxCollider collider = zoneObj.AddComponent<BoxCollider>();
            collider.size = zone.boxsize;
            collider.isTrigger = true;

            zone.zoneObject = zoneObj;

            CameraSwitcher.Register(zone.camera);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var zone in cameraZones)
            {
                if (zone.zoneObject.GetComponent<Collider>().bounds.Contains(other.transform.position))
                {
                    if (stateDrivenCamera != null)
                        stateDrivenCamera.enabled = false;
                    CameraSwitcher.SwitchCamera(zone.camera);
                    break;
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
                if (other.gameObject.CompareTag("Player") == zone.zoneObject) // 해당 Zone의 GameObject와 일치하는지 확인
                {
                    if (stateDrivenCamera != null)
                        stateDrivenCamera.enabled = true;

                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var zone in cameraZones)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(zone.position, zone.boxsize);
        }
    }
}
