using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public Transform giantTransform; // 거인의 Transform
    public float raycastDistance = 50f; // 레이캐스트 거리

    public List<GameObject> DroppedObjects = new List<GameObject>(); // 떨어진 오브젝트 리스트

    private bool isRaycasting = false;
    private bool isConditionMet = false;


    // 레이캐스트 발사 및 처리 로직을 각 오브젝트의 스크립트로 이동
    public void HandleObjectCollision(GameObject obj)
    {
        // 충돌한 오브젝트를 리스트에 추가
        if (!DroppedObjects.Contains(obj))
        {
            DroppedObjects.Add(obj);
            isConditionMet = true;
        }

        // 레이케스트 안 써도 됨. 그냥 실험해볼라고 쓴거임
        RaycastHit hit;
        Vector3 directionToGiant = (giantTransform.position - obj.transform.position).normalized;
        if (Physics.Raycast(obj.transform.position, directionToGiant, out hit, raycastDistance))
        {
            if (hit.transform == giantTransform)
            {
                isRaycasting = true;
                Debug.Log($"Raycast from {obj.name} hit the Giant.");

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
        // 현재 조건을 반환하고, 조건이 충족된 후 리스트가 비어있다면 조건을 해제
        bool currentCondition = isConditionMet;
        if (isConditionMet && DroppedObjects.Count == 0)
        {
            isConditionMet = false; // 모든 오브젝트 처리 후 조건 해제
        }
        return currentCondition;
    }

    public bool IsRaycasting()
    {
        return isRaycasting;
    }
    public bool IsSearchCompleted()
    {
        // 모든 물체에 대한 탐색이 완료되었는지 확인
        return DroppedObjects.Count == 0;
    }

}