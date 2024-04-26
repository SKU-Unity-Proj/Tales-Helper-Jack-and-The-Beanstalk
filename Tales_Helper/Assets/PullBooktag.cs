using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBooktag : MonoBehaviour
{
    public Animator playerAnimator; // 플레이어 애니메이터
    public Animator bookAnimator; // 책 애니메이터
    public Animator playerAnimator2; // 두 번째 플레이어 애니메이터
    public Animator bookAnimator2; // 두 번째 책 애니메이터
    public Transform targetPos; // F키를 눌렀을 때 이동할 위치
    public Transform targetPos2; // 두 번째 위치
    public float interactionDistance = 1f; // 상호작용 가능한 최대 거리
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키
    public GameObject paperObject; // 첫 번째 종이 오브젝트
    public GameObject paperObject2; // 두 번째 종이 오브젝트

    // 매 프레임마다 호출
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
        GameObject nearestBook = FindNearestWithTag("Book");

        if (nearestBook != null)
        {
            float distance = Vector3.Distance(transform.position, nearestBook.transform.position);
            if (distance <= interactionDistance)
            {
                SetPositionAndRotation();
                playerAnimator.SetTrigger("PullBook");
                bookAnimator.SetTrigger("PullBook");
                Invoke("ActivatePaper", 4.4f);
                nearestBook.tag = "Untagged"; // 태그를 Untagged로 변경
            }
        }
    }

    public void ActivatePaper()
    {
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
        GameObject nearestBook = FindNearestWithTag("Book2");

        if (nearestBook != null)
        {
            float distance = Vector3.Distance(transform.position, nearestBook.transform.position);
            if (distance <= interactionDistance)
            {
                SetPositionAndRotation2();
                playerAnimator2.SetTrigger("PullBook");
                bookAnimator2.SetTrigger("PullBook");
                Invoke("ActivatePaper2", 3.2f);
                nearestBook.tag = "Untagged"; // 태그를 Untagged로 변경
            }
        }
    }

    public void ActivatePaper2()
    {
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
            transform.position = targetPos.position;
            transform.rotation = targetPos.rotation;
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
            transform.position = targetPos2.position;
            transform.rotation = targetPos2.rotation;
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
