using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBooktag : MonoBehaviour
{
    public Animator playerAnimator; // 플레이어 애니메이터
    public Animator bookAnimator; // 책 애니메이터
    public Animator playerAnimator2; // 플레이어 애니메이터
    public Animator bookAnimator2; // 책 애니메이터
    public Transform targetPos; // F을 눌렀을 때 위치
    public Transform targetPos2; // F을 눌렀을 때 위치
    public float interactionDistance = 1f; // 상호작용 가능한 최대 거리
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키
    public GameObject paperObject;
    public GameObject paperObject2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            CheckBook();
            CheckBook2();
        }
    }

    private void CheckBook()
    {
        // 플레이어 위치에서 가장 가까운 버튼 찾기 (버튼 오브젝트는 "Book" 태그로 지정되어 있어야 함)
        GameObject nearestBook = FindNearestWithTag("Book");

        if (nearestBook != null)
        {
            // 플레이어와 버튼 간의 거리 계산
            float distance = Vector3.Distance(transform.position, nearestBook.transform.position);
            // 거리가 interactionDistance 이내인지 확인
            if (distance <= interactionDistance)
            {
                SetPositionAndRotation();
                // 플레이어와 버튼의 애니메이션 재생
                playerAnimator.SetTrigger("PullBook");
                bookAnimator.SetTrigger("PullBook");
                Invoke("ActivatePaper", 4.4f);
            }
        }
    }

    public void ActivatePaper()
    {
        // Paper 오브젝트가 존재하면 활성화
        if (paperObject != null)
        {
            paperObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Paper 오브젝트가 할당되지 않았습니다.");
        }
    }
    private void CheckBook2()
    {
        // 플레이어 위치에서 가장 가까운 버튼 찾기 (버튼 오브젝트는 "Book" 태그로 지정되어 있어야 함)
        GameObject nearestBook = FindNearestWithTag("Book2");

        if (nearestBook != null)
        {
            // 플레이어와 버튼 간의 거리 계산
            float distance = Vector3.Distance(transform.position, nearestBook.transform.position);
            // 거리가 interactionDistance 이내인지 확인
            if (distance <= interactionDistance)
            {
                SetPositionAndRotation2();
                // 플레이어와 버튼의 애니메이션 재생
                playerAnimator2.SetTrigger("PullBook");
                bookAnimator2.SetTrigger("PullBook");
                Invoke("ActivatePaper2", 3.2f);
            }
        }
    }

    public void ActivatePaper2()
    {
        // Paper 오브젝트가 존재하면 활성화
        if (paperObject2 != null)
        {
            paperObject2.SetActive(true);
        }
        else
        {
            Debug.LogError("Paper 오브젝트가 할당되지 않았습니다.");
        }
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
    public void SetPositionAndRotation2()
    {
        if (targetPos2 != null)
        {
            transform.position = targetPos2.position; // 위치 설정
            transform.rotation = targetPos2.rotation; // 회전 설정
        }
        else
        {
            Debug.LogError("Target position is not assigned!");
        }
    }

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
