using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonInteraction : MonoBehaviour
{
    public Animator playerAnimator; // 플레이어 애니메이터
    public Animator buttonAnimator; // 버튼 애니메이터
    public float interactionDistance = 1f; // 상호작용 가능한 최대 거리
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키

    private void ButtonUpdate()
    {
        // 상호작용 키를 눌렀는지 확인
        if (Input.GetKeyDown(interactionKey))
        {
            ButtonCheckInteraction();
        }
    }

    private void ButtonCheckInteraction()
    {
        // 플레이어 위치에서 가장 가까운 버튼 찾기 (버튼 오브젝트는 "Button" 태그로 지정되어 있어야 함)
        GameObject nearestButton = FindNearestButtonWithTag("Button");
        if (nearestButton != null)
        {
            // 플레이어와 버튼 간의 거리 계산
            float distance = Vector3.Distance(transform.position, nearestButton.transform.position);

            // 거리가 interactionDistance 이내인지 확인
            if (distance <= interactionDistance)
            {
                // 플레이어와 버튼의 애니메이션 재생
                playerAnimator.SetTrigger("Press Button");
                buttonAnimator.SetTrigger("push");
            }
        }
    }

    // 지정된 태그를 가진 가장 가까운 오브젝트를 찾는 함수
    GameObject FindNearestButtonWithTag(string tag)
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
