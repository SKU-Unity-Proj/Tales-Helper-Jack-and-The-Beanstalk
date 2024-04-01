using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Animator playerAnimator; // �÷��̾� �ִϸ�����
    public Animator buttonAnimator; // ��ư �ִϸ�����
    public Transform targetPos; // ��ư�� ������ �� ��ġ
    public Light pointLight; // Point Light�� ����
    public Color[] lightColorsOnInteraction; // ��ȣ�ۿ� �� ����� ���� ���� �迭
    public Material[] outlineMaterials; // ���͸����� �ƿ����� ������ ������ ���͸��� �迭
    private int currentColorIndex = 0; // ���� ���� �ε���
    public float interactionDistance = 1f; // ��ȣ�ۿ� ������ �ִ� �Ÿ�
    public KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű

    private void Update()
    {
        //�ִϸ��̼� ����� ������ ���� ��Ȱ��ȭ
        playerAnimator.applyRootMotion = false;
        // ��ȣ�ۿ� Ű�� �������� Ȯ��
        if (Input.GetKeyDown(interactionKey))
        {
            CheckInteraction();
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
                SetPositionAndRotation();
                // �÷��̾�� ��ư�� �ִϸ��̼� ���
                playerAnimator.SetTrigger("Press Button");
                buttonAnimator.SetTrigger("Press");
                Invoke("ChangeLightColor", 1.2f);
                Invoke("ChangeOutlineColor", 1.2f);
            }
        }
    }

    private void ChangeOutlineColor()
    {
        if (outlineMaterials != null && outlineMaterials.Length > 0)
        {
            // ���� ���� �ε����� �ش��ϴ� ���͸����� �����ͼ� �ƿ����� ������ ����
            Material outlineMaterial = outlineMaterials[currentColorIndex];
            outlineMaterial.SetColor("_OutlineColor", lightColorsOnInteraction[currentColorIndex]);
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
        }
    }

    public void SetPositionAndRotation()
    {
        if (targetPos != null)
        {
            transform.position = targetPos.position;// ��ġ ����
            transform.rotation = targetPos.rotation;// ȸ�� ����
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
