using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public static DroppedObject Instance { get; private set; }
    public List<GameObject> DroppedObjects = new List<GameObject>(); // 떨어진 오브젝트 리스트
    public GameObject GrabPoint; // 초기화할 객체를 선언합니다.

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
            // 추가 로직
        }
    }
    #endregion


    #region 불형식 체크 함수
    public bool CheckCondition()
    {
        // 현재 조건을 반환하고, 조건이 충족된 후 리스트가 비어있다면 조건을 해제
        bool currentCondition = isConditionMet;
        if (isConditionMet && DroppedObjects.Count == 0)
        {
            isConditionMet = false; // 모든 오브젝트 처리 후 조건 해제
        }
        return currentCondition;
    }

    public bool IsSearchCompleted()
    {
        // 모든 물체에 대한 탐색이 완료되었는지 확인
        return DroppedObjects.Count == 0;
    }

    // 특별한 오브젝트('checkgrab')이 감지되었는지 확인하는 메소드
    public bool CheckSpecialObjectCondition()
    {
        foreach (var droppedObject in DroppedObjects)
        {
            if (droppedObject.name == "CheckGrab")
            {
                // 거리 제한 없이 무조건 true 반환
                return true;
            }
        }
        return false;
    }
    #endregion
}