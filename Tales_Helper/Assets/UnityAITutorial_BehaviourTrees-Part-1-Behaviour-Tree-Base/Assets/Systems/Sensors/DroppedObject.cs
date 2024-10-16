using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public static DroppedObject Instance { get; private set; }
    public List<GameObject> DroppedObjects = new List<GameObject>(); // 떨어진 오브젝트 리스트
    public List<GameObject> SpecialObjects = new List<GameObject>(); // 특별 오브젝트 리스트

    private Transform giantTransform; // 이제 private로 선언
    public float raycastDistance = 50f; // 레이캐스트 거리

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


    // Giant Transform을 설정하는 퍼블릭 메소드
    public void SetGiantTransform(Transform newGiantTransform)
    {
        giantTransform = newGiantTransform;
    }

    public int GetDroppedObjectsCount()
    {
        return DroppedObjects.Count;
    }

    #region 떨어진 오브젝트 등록 및 제거 함수
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
            // 추가 로직
        }
    }
    #endregion


    #region 불형식 체크 함수
    public bool CheckCondition()
    {
        return DroppedObjects.Count > 0; // 일반 오브젝트가 있는 경우
    }

    public bool IsSearchCompleted()
    {
        // 모든 물체에 대한 탐색이 완료되었는지 확인
        return DroppedObjects.Count == 0;
    }

    // 특별한 오브젝트('checkgrab')이 감지되었는지 확인하는 메소드
    public bool CheckSpecialObjectCondition()
    {
        return SpecialObjects.Count > 0; // 특별 오브젝트가 있는 경우
        #endregion
    }
}