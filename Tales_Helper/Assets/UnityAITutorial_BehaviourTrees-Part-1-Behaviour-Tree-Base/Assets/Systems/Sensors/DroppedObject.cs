using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public Transform giantTransform; // ������ Transform
    public LineRenderer lineRenderer; // LineRenderer ������Ʈ
    public float raycastDistance = 50f; // ����ĳ��Ʈ �Ÿ�

    public List<GameObject> DroppedObjects = new List<GameObject>(); // ������ ������Ʈ ����Ʈ

    private bool isRaycasting = false;
    private bool isConditionMet = false;


    // ����ĳ��Ʈ �߻� �� ó�� ������ �� ������Ʈ�� ��ũ��Ʈ�� �̵�
    public void HandleObjectCollision(GameObject obj)
    {
        // �浹�� ������Ʈ�� ����Ʈ�� �߰�
        if (!DroppedObjects.Contains(obj))
        {
            DroppedObjects.Add(obj);
        }

        // �����ɽ�Ʈ �� �ᵵ ��. �׳� �����غ���� ������
        RaycastHit hit;
        Vector3 directionToGiant = (giantTransform.position - obj.transform.position).normalized;
        if (Physics.Raycast(obj.transform.position, directionToGiant, out hit, raycastDistance))
        {
            if (hit.transform == giantTransform)
            {
                isRaycasting = true;
                Debug.Log($"Raycast from {obj.name} hit the Giant.");
                isConditionMet = true;

                if (isRaycasting)
                {
                    Debug.Log(DroppedObjects.Count);
                }
            }
            else
            {
                Debug.Log("Raycast did not hit the Giant. It hit: " + hit.transform.name);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }


    public bool CheckCondition()
    {
        return isConditionMet;
    }
    public bool IsRaycasting()
    {
        return isRaycasting;
    }
    public bool IsSearchCompleted()
    {
        // ��� ��ü�� ���� Ž���� �Ϸ�Ǿ����� Ȯ��
        return DroppedObjects.Count == 0;
    }

}