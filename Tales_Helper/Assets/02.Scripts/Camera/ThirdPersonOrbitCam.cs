
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 카메라 속성중 중요 속성 하나는 카메라로부터 위치 오프셋 벡터, 피봇 오프셋 벡터
// 위치 오프셋 벡터는 충돌 처리용으로 사용하고 피봇 오프셋 벡터는 시선이동에 사용하도록
// 충돌체크 : 이중 충돌 체크 가능 ( 캐릭터로부터 카메라, 카메라로부터 캐릭터사이 )
// 사격 반동을 위한 기능
// FOV - 시야각 변경 기능

[RequireComponent(typeof(Camera))] // 카메라 컴포넌트가 누락되는 것을 방지하기 위한 코드. -> 스크립트를 추가하면 자동으로 Camera 컴포넌트 반영.
public class ThirdPersonOrbitCam : MonoBehaviour
{
    public Transform player;
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 camOffset = new Vector3(0.4f, 0.5f, 2.0f); // Z 값을 양수로 변경.

    public float followSpeed = 3f; // 카메라가 플레이어를 따라가는 속도
    public float smoothTime = 0.3F; // 카메라의 부드러운 움직임을 제어

    private Vector3 velocity = Vector3.zero; // 부드러운 움직임을 위한 벨로시티

    public float smooth = 10f;  // 카메라 반응속도

    private Transform cameraTransform; // 트랜스폼 캐싱.
    private Camera myCamera;
    private Vector3 relCameraPos; // 플레이어로부터 카메라까지의 벡터.
    private float relCameraPosMag; // 플레이어로부터 카메라사이 거리.
    private Vector3 smoothPivotOffset; // 카메라 피봇 보간용 벡터. 
    private Vector3 smoothCamOffset; // 카메라 위치 보간용 벡터. 
    private Vector3 targetPivotOffset; // 카메라 피봇 보간용 벡터. 
    private Vector3 targetCamOffset; // 카메라 위치 보간용 벡터.

    private float defaultFOV; // 기본 시야값.
    private float targetFOV; // 타겟 시야값.

    public float followTime = 0.3f; // 카메라가 플레이어를 따라가는 데 걸리는 시간 (초 단위)

    private void Awake()
    {
        //캐싱
        cameraTransform = transform;
        myCamera = cameraTransform.GetComponent<Camera>();


        // 카메라 기본 포지션 세팅
        cameraTransform.position = player.position + camOffset;
        //cameraTransform.rotation = Quaternion.identity;

        // 카메라와 플레이어간의 상대 벡터. -> 충돌체크에 사용함
        relCameraPos = cameraTransform.position - player.position;
        relCameraPosMag = relCameraPos.magnitude - 0.5f; // 플레이어를 뺀 거리.

        // 기본세팅
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = myCamera.fieldOfView;
 

        ResetTargetOffsets();
        ResetFOV();

    }
    //초기화 함수
    public void ResetTargetOffsets()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
    }
    public void ResetFOV()
    {
        this.targetFOV = defaultFOV;
    }

    // 기본 세팅 조절 함수
    public void SetTargetOffset(Vector3 newPivotOffset, Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
    }
    public void SetFOV(float customFOV)
    {
        this.targetFOV = customFOV;
    }


    // 충돌체크 함수 추후에 줌 기능이나 뭐 쏘는 기능 구현하면 쓸거
    /*
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight) // playerPos는 발밑이므로 플레이어의 높이를 체크해서 그 앞으로 충돌체크를 이어나감.
    {
        Vector3 target = player.position + (Vector3.up * deltaPlayerHeight);
        // Raycast : 충돌하는 collider에 대한 거리, 위치등 자세한 정보를 RaycastHit로 반환.
        // 또한, LayerMask 필터링을 해서 원하는 layer에 충돌을 하면 true 값 반환.
        // 추가로, OverlapSphere : 중점과 반지름으로 가상의 원을 만들어 추출하려는 반경 이내에 들어와 있는 collider를 반환하는 함수.
        // 함수의 반환 값은 collider 컴포넌트 배열로 넘어온다.
        if (Physics.SphereCast(checkPos, 0.2f, target - checkPos, out RaycastHit hit, relCameraPosMag))
        {
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger) // isTrigger을 false로 두는 이유는 이벤트나 연출을 위한 Collider는 제외 시켜야 되기때문.
            {
                return false;
            }
        }
        return true;
    }
    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float maxDistance)
    {
        Vector3 origin = player.position + (Vector3.up * deltaPlayerHeight);
        if (Physics.SphereCast(origin, 0.2f, checkPos - origin, out RaycastHit hit, maxDistance))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
    {
        float playerFocusHeight = player.GetComponent<CapsuleCollider>().height * 0.75f;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight, offset);
    }
    */

    private void FixedUpdate()
    {

        //set FOV
        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFOV, Time.deltaTime);

        /* 마찬가지로 줌 기능때 사용하는거
        Vector3 baseTempPos = player.position + camYRotation * targetPivotOffset; // 기본 포지션 값.
        Vector3 noCollisionOffset = targetCamOffset; // 조준할 때 카메라의 오프셋 값, 조준할 때와 하지 않을 때 값과 다르다.

        for (float zOffset = targetCamOffset.z; zOffset <= 0f; zOffset += 0.5f)
        {
            noCollisionOffset.z = zOffset;
            if (DoubleViewingPosCheck(baseTempPos + aimRotation * noCollisionOffset, Mathf.Abs(zOffset)) || zOffset == 0f)
            {
                break;
            }
        }
        */

        float xOffset = -5.0f;  // -x 방향으로의 오프셋
        float yOffset = 3.0f;   // +y 방향으로의 오프셋

        Vector3 customOffset = new Vector3(xOffset, yOffset, 0); // 원하는 오프셋 값 설정

        // 회전 쿼터니언을 정의
        Quaternion rotate90Degrees = Quaternion.Euler(0, -90, 0);

        // 현재 오프셋을 회전하여 새로운 오프셋을 계산
        Vector3 rotatedCamOffset = rotate90Degrees * smoothCamOffset;

        Vector3 targetPosition = player.position + smoothPivotOffset + rotatedCamOffset + customOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, followSpeed);

        cameraTransform.position = smoothedPosition;

        // 플레이어의 회전을 가져와서 카메라에 적용
        Quaternion targetRotation = Quaternion.Euler(0, 90, 0); // 카메라 회전 축 설정
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, Time.deltaTime);

    }

}
