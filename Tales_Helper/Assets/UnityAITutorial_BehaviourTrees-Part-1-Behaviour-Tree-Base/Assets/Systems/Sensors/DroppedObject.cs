using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public static DroppedObject Instance { get; private set; }
    public List<GameObject> DroppedObjects = new List<GameObject>(); // ������ ������Ʈ ����Ʈ
    public GameObject GrabPoint; // �ʱ�ȭ�� ��ü�� �����մϴ�.

    private Transform giantTransform; // ���� private�� ����
    public float raycastDistance = 50f; // ����ĳ��Ʈ �Ÿ�

    private bool isConditionMet = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Giant Transform�� �����ϴ� �ۺ� �޼ҵ�
    public void SetGiantTransform(Transform newGiantTransform)
    {
        giantTransform = newGiantTransform;
    }

    public int GetDroppedObjectsCount()
    {
        return DroppedObjects.Count;
    }

    #region ������ ������Ʈ ��� �� ���� �Լ�
    public void AddDroppedObject(GameObject obj)
    {
        if (!DroppedObjects.Contains(obj))
        {
            DroppedObjects.Add(obj);

        }
        isConditionMet = true;
    }

    public void RemoveDroppedObject(GameObject obj)
    {
        if (DroppedObjects.Contains(obj))
        {
            DroppedObjects.Remove(obj);
            // �߰� ����
        }
    }
    #endregion


    #region ������ üũ �Լ�
    public bool CheckCondition()
    {
        // ���� ������ ��ȯ�ϰ�, ������ ������ �� ����Ʈ�� ����ִٸ� ������ ����
        bool currentCondition = isConditionMet;
        if (isConditionMet && DroppedObjects.Count == 0)
        {
            isConditionMet = false; // ��� ������Ʈ ó�� �� ���� ����
        }
        return currentCondition;
    }

    public bool IsSearchCompleted()
    {
        // ��� ��ü�� ���� Ž���� �Ϸ�Ǿ����� Ȯ��
        return DroppedObjects.Count == 0;
    }

    // Ư���� ������Ʈ('checkgrab')�� �����Ǿ����� Ȯ���ϴ� �޼ҵ�
    public bool CheckSpecialObjectCondition()
    {
        foreach (var droppedObject in DroppedObjects)
        {
            if (droppedObject.name == "CheckGrab")
            {
                // �Ÿ� ���� ���� ������ true ��ȯ
                return true;
            }
        }
        return false;
    }
    #endregion
}