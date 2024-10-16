using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public static DroppedObject Instance { get; private set; }
    public List<GameObject> DroppedObjects = new List<GameObject>(); // ������ ������Ʈ ����Ʈ
    public List<GameObject> SpecialObjects = new List<GameObject>(); // Ư�� ������Ʈ ����Ʈ

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
    public void AddDroppedObject(GameObject obj, bool isSpecial)
    {
        if (isSpecial)
        {
            SpecialObjects.Add(obj);
        }
        else
        {
            DroppedObjects.Add(obj);
        }
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
        return DroppedObjects.Count > 0; // �Ϲ� ������Ʈ�� �ִ� ���
    }

    public bool IsSearchCompleted()
    {
        // ��� ��ü�� ���� Ž���� �Ϸ�Ǿ����� Ȯ��
        return DroppedObjects.Count == 0;
    }

    // Ư���� ������Ʈ('checkgrab')�� �����Ǿ����� Ȯ���ϴ� �޼ҵ�
    public bool CheckSpecialObjectCondition()
    {
        return SpecialObjects.Count > 0; // Ư�� ������Ʈ�� �ִ� ���
        #endregion
    }
}