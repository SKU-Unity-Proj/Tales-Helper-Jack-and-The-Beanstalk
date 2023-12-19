using UnityEngine;
using Cinemachine;

public class CameraTriggerManager : MonoBehaviour
{
    [System.Serializable]
    public class CameraZone
    {
        public CinemachineVirtualCamera camera;
        public Vector3 position;
        public Vector3 boxsize;
        public GameObject zoneObject;
    }

    [SerializeField] private CameraZone[] cameraZones;
    public CinemachineStateDrivenCamera stateDrivenCamera;
    private CinemachineVirtualCamera activeCamera = null;

    private void Awake()
    {
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
    }

    private void Start()
    {
        int zoneIndex = 0;
        foreach (var zone in cameraZones)
        {
            string zoneObjectName = zone.camera.name + "_Zone_" + zoneIndex;
            GameObject zoneObj = new GameObject(zoneObjectName);
            zoneObj.transform.position = zone.position;
            zoneObj.transform.parent = this.transform;

            BoxCollider collider = zoneObj.AddComponent<BoxCollider>();
            collider.size = zone.boxsize;
            collider.isTrigger = true;

            ZoneIndex zIdx = zoneObj.AddComponent<ZoneIndex>();
            zIdx.index = zoneIndex;

            zone.zoneObject = zoneObj;
            Debug.Log(zoneObj + "," + zoneIndex);

            activeCamera = zone.camera;
            activeCamera.Priority = 0;
            zoneIndex++;
        }
       

    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 해당 존의 콜라이더 내에 있는지 확인
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

    /*
    private void SwitchCamera(CinemachineVirtualCamera newCamera)
    {
        activeCamera = newCamera;

        if (activeCamera == null)
        {
            activeCamera.Priority = 0;
        }

       
        if (activeCamera != null)
        {

            activeCamera.Priority = 11;
        }
    }
    */


    private void OnDrawGizmos()
    {
        foreach (var zone in cameraZones)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(zone.position, zone.boxsize);
        }
    }

    private int GetZoneIndex(GameObject zoneObject)
    {
        ZoneIndex zIdx = zoneObject.GetComponent<ZoneIndex>();
        if (zIdx != null)
        {
            return zIdx.index;
        }
        return -1;
    }
}



