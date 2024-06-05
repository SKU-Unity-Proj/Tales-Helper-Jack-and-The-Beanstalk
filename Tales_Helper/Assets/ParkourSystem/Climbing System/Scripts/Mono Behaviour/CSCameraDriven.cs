using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSCameraDriven : MonoBehaviour
{
    private CameraTriggerManager cameraTriggerManager;

    void Start()
    {
        // CameraTriggerManager 인스턴스를 찾아서 cameraTriggerManager 변수에 할당
        // 이를 통해, 이 스크립트는 카메라 존 정보에 접근할 수 있음
        cameraTriggerManager = FindObjectOfType<CameraTriggerManager>();
    }

    // 오브젝트가 트리거 콜라이더에 진입할 때 자동으로 호출
    private void OnTriggerEnter(Collider other)
    {
        //카메라존 태그가 없으면 실행X
        if (other.CompareTag("CameraZone"))
        {
            // 현재 충돌한 콜라이더에 해당하는 카메라 존을 가져옴
            var cameraZone = cameraTriggerManager.GetCameraZoneFromCollider(other);

            // 이는 해당 카메라 존에 진입했을 때, 그 카메라를 활성화하는 데 사용
            if (cameraZone != null)
            {
                cameraZone.camera.Priority = 11;
            }
        }
        else
            return;
    }

    // 오브젝트가 트리거 콜라이더에서 벗어날 때 자동으로 호출
    private void OnTriggerExit(Collider other)
    {
        //카메라존 태그가 없으면 실행X
        if (other.CompareTag("CameraZone"))
        {
            // 현재 충돌한 콜라이더에 해당하는 카메라 존을 가져옴
            var cameraZone = cameraTriggerManager.GetCameraZoneFromCollider(other);

            // 이는 해당 카메라 존에 진입했을 때, 그 카메라를 활성화하는 데 사용
            if (cameraZone != null)
            {
                cameraZone.camera.Priority = 0;
            }
        }
        else
            return;
    }
}