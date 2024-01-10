using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public Transform giantTransform; // 거인의 Transform
    public LineRenderer lineRenderer; // LineRenderer 컴포넌트
    public float raycastDistance = 50f; // 레이캐스트 거리

    public List<Transform> DroppedObjects = new List<Transform>(); // 떨어진 오브젝트 리스트

    private bool isRaycasting = false;
    private bool isConditionMet = false;

    void Start()
    {
        Animator anim = GetComponent<Animator>();

        // LineRenderer 초기화
        lineRenderer.positionCount = 2; // 선의 시작점과 끝점 설정
        lineRenderer.enabled = false; // 처음에는 선을 비활성화
    }

    void Update()
    {
        if (isRaycasting)
        {
            UpdateRaycastingLine();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Giant"))
        {
            RaycastHit hit;
            Vector3 directionToGiant = (giantTransform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, directionToGiant, out hit, raycastDistance))
            {
                // 레이캐스트가 거인에게 도달했는지 확인
                if (hit.transform == giantTransform)
                {
                    Debug.Log("Raycast hit the Giant.");
                    isRaycasting = true;
                    lineRenderer.enabled = true;

                    // 레이캐스트가 거인에게 도달했다면, 오브젝트를 감지 목록에 추가
                    AddDroppedObject(transform);
                    isConditionMet = true;
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

    }

    public void AddDroppedObject(Transform obj)
    {
        if (!DroppedObjects.Contains(obj))
        {
            DroppedObjects.Add(obj);
        }
    }

    void UpdateRaycastingLine()
    {
        // 선의 시작점을 오브젝트의 위치로, 끝점을 거인의 위치로 설정
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, giantTransform.position);
    }
    public bool CheckCondition()
    {
        return isConditionMet;
    }
   
}