using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))] // BoxCollider 컴포넌트가 필요함을 명시
[RequireComponent(typeof(Rigidbody))] // Rigidbody 컴포넌트가 필요함을 명시
public class CameraTriggerManager : MonoBehaviour
{
    // 카메라 존을 정의하는 클래스
    [System.Serializable]
    public class CameraZone
    {
        public CinemachineVirtualCamera camera; // 카메라 존에 연결된 CinemachineVirtualCamera
        public Vector3 position; // 카메라 존의 위치
        public Vector3 boxsize; // 카메라 존의 크기
        public GameObject zoneObject; // 카메라 존을 나타내는 GameObject
    }

    [SerializeField] private CameraZone[] cameraZones; // 여러 카메라 존을 저장하는 배열

    public CinemachineStateDrivenCamera stateDrivenCamera; // 상태 기반 Cinemachine 카메라

    private void Awake()
    {
        // 상태 기반 Cinemachine 카메라 찾기
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
    }

    private void Start()
    {
        // 카메라 존 초기화
        foreach (var zone in cameraZones)
        {
            // 새로운 GameObject 생성 및 설정
            GameObject zoneObj = new GameObject(zone.camera.name + "_Zone");
            zoneObj.transform.position = zone.position;
            zoneObj.transform.parent = this.transform;

            // BoxCollider 추가 및 설정
            BoxCollider collider = zoneObj.AddComponent<BoxCollider>();
            collider.size = zone.boxsize;
            collider.isTrigger = true; // Trigger로 설정

            zone.zoneObject = zoneObj; // zoneObject에 생성한 GameObject 할당

            CameraSwitcher.Register(zone.camera); // 카메라 등록
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 카메라 존에 들어갔을 때 처리
        if (other.CompareTag("Player"))
        {
            foreach (var zone in cameraZones)
            {
                // 플레이어가 해당 존 내부에 있는지 확인
                if (zone.zoneObject.GetComponent<Collider>().bounds.Contains(other.transform.position))
                {
                    if (stateDrivenCamera != null)
                        stateDrivenCamera.enabled = false; // 상태 기반 카메라 비활성화

                    CameraSwitcher.SwitchCamera(zone.camera); // 카메라 전환
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 카메라 존을 벗어났을 때 처리
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (var zone in cameraZones)
            {
                //벗어났는지 확인
                if (other.gameObject.CompareTag("Player") == zone.zoneObject)
                {
                    if (stateDrivenCamera != null)
                        stateDrivenCamera.enabled = true; // 상태 기반 카메라 활성화

                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 에디터에서 카메라 존의 경계를 시각화
        foreach (var zone in cameraZones)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(zone.position, zone.boxsize);
        }
    }
}
