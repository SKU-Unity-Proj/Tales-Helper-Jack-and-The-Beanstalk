using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBooktag : MonoBehaviour
{
    public Animator playerAnimator; // �÷��̾� �ִϸ�����
    public Animator bookAnimator; // å �ִϸ�����
    public Animator playerAnimator2; // �� ��° �÷��̾� �ִϸ�����
    public Animator bookAnimator2; // �� ��° å �ִϸ�����
    public Transform targetPos; // FŰ�� ������ �� �̵��� ��ġ
    public Transform targetPos2; // �� ��° ��ġ
    public float interactionDistance = 1f; // ��ȣ�ۿ� ������ �ִ� �Ÿ�
    public KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű
    public GameObject paperObject; // ù ��° ���� ������Ʈ
    public GameObject paperObject2; // �� ��° ���� ������Ʈ

    // �� �����Ӹ��� ȣ��
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
                nearestBook.tag = "Untagged"; // �±׸� Untagged�� ����
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
            Debug.LogError("Paper ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
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
                nearestBook.tag = "Untagged"; // �±׸� Untagged�� ����
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
            Debug.LogError("Paper ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
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
