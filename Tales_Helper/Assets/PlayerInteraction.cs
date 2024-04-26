using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Animator playerAnimator; // �÷��̾� �ִϸ�����
    public Animator buttonAnimator; // ��ư �ִϸ�����
    private bool isAnimating = false; // �ִϸ��̼��� �۵� ������ ���θ� �����ϴ� ����

    public Transform targetPos; // ��ư�� ������ �� ��ġ

    public Light pointLight; // Point Light�� ����
    public Color[] lightColorsOnInteraction; // ��ȣ�ۿ� �� ����� ���� ���� �迭
    private int currentColorIndex = 0; // ���� ���� �ε���

    public float interactionDistance = 1f; // ��ȣ�ۿ� ������ �ִ� �Ÿ�
    public KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű

    public GameObject[] books; // Ȱ��ȭ�� å ������Ʈ �迭
    private int currentBookIndex = 0; // ���� Ȱ��ȭ�� å�� �ε���

    private void Update()
    {
        if (!isAnimating)
        {
            // ��ȣ�ۿ� Ű�� �������� Ȯ��
            if (Input.GetKeyDown(interactionKey))
            {
                CheckInteraction();
            }
        }
        else
        {
            // �ִϸ��̼��� �۵� ���� ������ SetPositionAndRotation() �Լ� ȣ��
            SetPositionAndRotation();
        }
    }

    private void CheckInteraction()
    {
        // �÷��̾� ��ġ���� ���� ����� ��ư ã�� (��ư ������Ʈ�� "Button" �±׷� �����Ǿ� �־�� ��)
        GameObject nearestButton = FindNearestWithTag("Button");

        if (nearestButton != null)
        {
            // �÷��̾�� ��ư ���� �Ÿ� ���
            float distance = Vector3.Distance(transform.position, nearestButton.transform.position);
            // �Ÿ��� interactionDistance �̳����� Ȯ��
            if (distance <= interactionDistance)
            {
                // �ִϸ��̼� �۵� ������ ǥ��
                isAnimating = true;

                // �÷��̾�� ��ư�� �ִϸ��̼� ���
                playerAnimator.SetTrigger("Press Button");
                buttonAnimator.SetTrigger("Press");

                // ���� �ð��� ���� �Ŀ� �ִϸ��̼� �۵��� ����Ǿ����� ǥ��
                Invoke("EndAnimation", 3.5f);

                // ���� ���� ����� å Ȱ��ȭ ó��
                ChangeLightColor();
            }
        }
    }

    private void ChangeLightColor()
    {
        if (pointLight != null && lightColorsOnInteraction.Length > 0)
        {
            // �迭 ���� ���� �������� ������ ������ ����
            pointLight.color = lightColorsOnInteraction[currentColorIndex];
            // ���� ��ȣ�ۿ��� ���� ���� �ε����� ������Ʈ
            currentColorIndex = (currentColorIndex + 1) % lightColorsOnInteraction.Length;

            // ���� å�� Ȱ��ȭ�ϰ� �ε����� ������Ŵ
            books[currentBookIndex].SetActive(false);
            currentBookIndex = (currentBookIndex + 1) % books.Length;
            books[currentBookIndex].SetActive(true);
        }
    }

    // �ִϸ��̼� �۵��� ����Ǿ����� ǥ���ϴ� �Լ�
    private void EndAnimation()
    {
        isAnimating = false;
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

    // ������ �±׸� ���� ���� ����� ������Ʈ�� ã�� �Լ�
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
