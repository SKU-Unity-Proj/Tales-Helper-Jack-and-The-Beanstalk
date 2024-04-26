using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Animator playerAnimator; // 플레이어 애니메이터
    public Animator buttonAnimator; // 버튼 애니메이터
    private bool isAnimating = false; // 애니메이션이 작동 중인지 여부를 추적하는 변수

    public Transform targetPos; // 버튼을 눌렀을 때 위치

    public Light pointLight; // Point Light의 참조
    public Color[] lightColorsOnInteraction; // 상호작용 시 변경될 빛의 색상 배열
    private int currentColorIndex = 0; // 현재 색상 인덱스

    public float interactionDistance = 1f; // 상호작용 가능한 최대 거리
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키

    public GameObject[] books; // 활성화할 책 오브젝트 배열
    private int currentBookIndex = 0; // 현재 활성화된 책의 인덱스

    private void Update()
    {
        if (!isAnimating)
        {
            // 상호작용 키를 눌렀는지 확인
            if (Input.GetKeyDown(interactionKey))
            {
                CheckInteraction();
            }
        }
        else
        {
            // 애니메이션이 작동 중일 때에도 SetPositionAndRotation() 함수 호출
            SetPositionAndRotation();
        }
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
                // 애니메이션 작동 중임을 표시
                isAnimating = true;

                // 플레이어와 버튼의 애니메이션 재생
                playerAnimator.SetTrigger("Press Button");
                buttonAnimator.SetTrigger("Press");

                // 일정 시간이 지난 후에 애니메이션 작동이 종료되었음을 표시
                Invoke("EndAnimation", 3.5f);

                // 조명 색상 변경과 책 활성화 처리
                ChangeLightColor();
            }
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

            // 다음 책을 활성화하고 인덱스를 증가시킴
            books[currentBookIndex].SetActive(false);
            currentBookIndex = (currentBookIndex + 1) % books.Length;
            books[currentBookIndex].SetActive(true);
        }
    }

    // 애니메이션 작동이 종료되었음을 표시하는 함수
    private void EndAnimation()
    {
        isAnimating = false;
    }

    public void SetPositionAndRotation()
    {
        if (targetPos != null)
        {
            transform.position = targetPos.position; // 위치 설정
            transform.rotation = targetPos.rotation; // 회전 설정
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
