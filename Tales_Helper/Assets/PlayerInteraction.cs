using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Animator playerAnimator; // 플레이어 애니메이터
    public Animator buttonAnimator; // 버튼 애니메이터
    public Transform targetPos; // 버튼을 눌렀을 때 위치
    public Light pointLight; // Point Light의 참조
    public Color[] lightColorsOnInteraction; // 상호작용 시 변경될 빛의 색상 배열
    private int currentColorIndex = 0; // 현재 색상 인덱스
    public float interactionDistance = 1f; // 상호작용 가능한 최대 거리
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키
    public Material[] outlineMaterials; // 아웃라인 효과가 적용된 머티리얼 배열
    private int currentIndex = 0; // 현재 아웃라인을 켤 오브젝트의 인덱스
    //private IMover _mover = null;

    private void Awake()
    {
        //_mover = GetComponent<IMover>();

        //플레이어 움직임 멈추기
        //_mover.StopMovement();
    }

    private void Update()
    {
        // 상호작용 키를 눌렀는지 확인
        if (Input.GetKeyDown(interactionKey))
        {
            CheckInteraction();

        }

        //현재 실행되고 있는 애니메이션 이름이 Press button 일때 위의 함수 쓰기
    }

    private void CheckInteraction()
    {
        // 플레이어 위치에서 가장 가까운 버튼 찾기 (버튼 오브젝트는 "Button" 태그로 지정되어 있어야 함)
        GameObject nearestButton = FindNearestWithTag("Button");

        if (nearestButton != null)
        {
            // 플레이어와 버튼 간의 거리 계산
            float distance = Vector3.Distance(transform.position, nearestButton.transform.position);
            // 거리가 interactionDistance 이내인지 확인
            if (distance <= interactionDistance)
            {
                SetPositionAndRotation();
                // 플레이어와 버튼의 애니메이션 재생
                playerAnimator.SetTrigger("Press Button");
                buttonAnimator.SetTrigger("Press");
                Invoke("ChangeLightColor", 1.2f);
                Invoke("ToggleNextOutline", 1.2f);

            }
        }
    }
    // 다음 오브젝트의 아웃라인을 토글하는 함수
    private void ToggleNextOutline()
    {
        // 현재 인덱스의 오브젝트 아웃라인을 끕니다.
        if (currentIndex < outlineMaterials.Length)
        {
            DisableOutline(currentIndex);
        }

        // 인덱스를 증가시킵니다.
        currentIndex++;

        // 모든 오브젝트의 아웃라인을 켰다면 인덱스를 초기화합니다.
        if (currentIndex >= outlineMaterials.Length)
        {
            currentIndex = 0;
        }

        // 다음 오브젝트의 아웃라인을 켭니다.
        EnableOutline(currentIndex);
    }

    // 특정 인덱스의 오브젝트 아웃라인을 켜는 함수
    private void EnableOutline(int index)
    {
        if (index >= 0 && index < outlineMaterials.Length)
        {
            Material outlineMaterial = outlineMaterials[index];
            outlineMaterial.SetFloat("_OutlineWidth", 0.02f); // 아웃라인 두께 설정
        }
    }

    // 특정 인덱스의 오브젝트 아웃라인을 끄는 함수
    private void DisableOutline(int index)
    {
        if (index >= 0 && index < outlineMaterials.Length)
        {
            Material outlineMaterial = outlineMaterials[index];
            outlineMaterial.SetFloat("_OutlineWidth", 0f); // 아웃라인 두께를 0으로 설정하여 비활성화
        }
    }



    private void ChangeLightColor()
    {
        if (pointLight != null && lightColorsOnInteraction.Length > 0)
        {
            // 배열 내의 다음 색상으로 조명의 색상을 변경
            pointLight.color = lightColorsOnInteraction[currentColorIndex];
            // 다음 상호작용을 위해 색상 인덱스를 업데이트
            currentColorIndex = (currentColorIndex + 1) % lightColorsOnInteraction.Length;
        }
    }

    public void SetPositionAndRotation()
    {
        if (targetPos != null)
        {
            transform.position = targetPos.position;// 위치 설정
            transform.rotation = targetPos.rotation;// 회전 설정
        }
        else
        {
            Debug.LogError("Target position is not assigned!");
        }
    }
    // 지정된 태그를 가진 가장 가까운 오브젝트를 찾는 함수
    GameObject FindNearestWithTag(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject obj in taggedObjects)
        {
            float distance = Vector3.Distance(obj.transform.position, currentPosition);
            if (distance < minDistance)
            {
                nearest = obj;
                minDistance = distance;
            }
        }

        return nearest;
    }
}
