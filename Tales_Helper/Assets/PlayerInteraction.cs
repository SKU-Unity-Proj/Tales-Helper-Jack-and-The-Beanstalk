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
    private int currentColorIndex = 0; // ���� ���� �ε���
    public float interactionDistance = 1f; // ��ȣ�ۿ� ������ �ִ� �Ÿ�
    public KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű
    public Material[] outlineMaterials; // �ƿ����� ȿ���� ����� ��Ƽ���� �迭
    private int currentIndex = 0; // ���� �ƿ������� �� ������Ʈ�� �ε���
    //private IMover _mover = null;

    private void Awake()
    {
        //_mover = GetComponent<IMover>();

        //�÷��̾� ������ ���߱�
        //_mover.StopMovement();
    }

    private void Update()
    {
        // ��ȣ�ۿ� Ű�� �������� Ȯ��
        if (Input.GetKeyDown(interactionKey))
        {
            CheckInteraction();

        }

        //���� ����ǰ� �ִ� �ִϸ��̼� �̸��� Press button �϶� ���� �Լ� ����
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
                Invoke("ToggleNextOutline", 1.2f);

            }
        }
    }
    // ���� ������Ʈ�� �ƿ������� ����ϴ� �Լ�
    private void ToggleNextOutline()
    {
        // ���� �ε����� ������Ʈ �ƿ������� ���ϴ�.
        if (currentIndex < outlineMaterials.Length)
        {
            DisableOutline(currentIndex);
        }

        // �ε����� ������ŵ�ϴ�.
        currentIndex++;

        // ��� ������Ʈ�� �ƿ������� �״ٸ� �ε����� �ʱ�ȭ�մϴ�.
        if (currentIndex >= outlineMaterials.Length)
        {
            currentIndex = 0;
        }

        // ���� ������Ʈ�� �ƿ������� �մϴ�.
        EnableOutline(currentIndex);
    }

    // Ư�� �ε����� ������Ʈ �ƿ������� �Ѵ� �Լ�
    private void EnableOutline(int index)
    {
        if (index >= 0 && index < outlineMaterials.Length)
        {
            Material outlineMaterial = outlineMaterials[index];
            outlineMaterial.SetFloat("_OutlineWidth", 0.02f); // �ƿ����� �β� ����
        }
    }

    // Ư�� �ε����� ������Ʈ �ƿ������� ���� �Լ�
    private void DisableOutline(int index)
    {
        if (index >= 0 && index < outlineMaterials.Length)
        {
            Material outlineMaterial = outlineMaterials[index];
            outlineMaterial.SetFloat("_OutlineWidth", 0f); // �ƿ����� �β��� 0���� �����Ͽ� ��Ȱ��ȭ
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
