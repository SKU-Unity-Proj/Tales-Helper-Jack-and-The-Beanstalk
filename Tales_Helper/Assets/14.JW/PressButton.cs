using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonInteraction : MonoBehaviour
{
    public Animator playerAnimator; // �÷��̾� �ִϸ�����
    public Animator buttonAnimator; // ��ư �ִϸ�����
    public float interactionDistance = 1f; // ��ȣ�ۿ� ������ �ִ� �Ÿ�
    public KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű

    private void ButtonUpdate()
    {
        // ��ȣ�ۿ� Ű�� �������� Ȯ��
        if (Input.GetKeyDown(interactionKey))
        {
            ButtonCheckInteraction();
        }
    }

    private void ButtonCheckInteraction()
    {
        // �÷��̾� ��ġ���� ���� ����� ��ư ã�� (��ư ������Ʈ�� "Button" �±׷� �����Ǿ� �־�� ��)
        GameObject nearestButton = FindNearestButtonWithTag("Button");
        if (nearestButton != null)
        {
            // �÷��̾�� ��ư ���� �Ÿ� ���
            float distance = Vector3.Distance(transform.position, nearestButton.transform.position);

            // �Ÿ��� interactionDistance �̳����� Ȯ��
            if (distance <= interactionDistance)
            {
                // �÷��̾�� ��ư�� �ִϸ��̼� ���
                playerAnimator.SetTrigger("Press Button");
                buttonAnimator.SetTrigger("push");
            }
        }
    }

    // ������ �±׸� ���� ���� ����� ������Ʈ�� ã�� �Լ�
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
