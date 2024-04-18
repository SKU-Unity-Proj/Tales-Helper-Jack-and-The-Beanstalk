using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBooktag : MonoBehaviour
{
    public Animator playerAnimator; // �÷��̾� �ִϸ�����
    public Animator bookAnimator; // å �ִϸ�����
    public Transform targetPos; // F�� ������ �� ��ġ
    public float interactionDistance = 1f; // ��ȣ�ۿ� ������ �ִ� �Ÿ�
    public KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű
    public GameObject paperObject;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            CheckBook();
        }
    }

    private void CheckBook()
    {
        // �÷��̾� ��ġ���� ���� ����� ��ư ã�� (��ư ������Ʈ�� "Book" �±׷� �����Ǿ� �־�� ��)
        GameObject nearestBook = FindNearestWithTag("Book");

        if (nearestBook != null)
        {
            // �÷��̾�� ��ư ���� �Ÿ� ���
            float distance = Vector3.Distance(transform.position, nearestBook.transform.position);
            // �Ÿ��� interactionDistance �̳����� Ȯ��
            if (distance <= interactionDistance)
            {
                SetPositionAndRotation();
                // �÷��̾�� ��ư�� �ִϸ��̼� ���
                playerAnimator.SetTrigger("PullBook");
                bookAnimator.SetTrigger("PullBook");
                Invoke("ActivatePaper", 4.4f);
            }
        }
    }

    public void ActivatePaper()
    {
        // Paper ������Ʈ�� �����ϸ� Ȱ��ȭ
        if (paperObject != null)
        {
            paperObject.SetActive(true);
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
            transform.position = targetPos.position; // ��ġ ����
            transform.rotation = targetPos.rotation; // ȸ�� ����
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
