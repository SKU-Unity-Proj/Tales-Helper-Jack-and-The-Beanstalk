using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public Transform giantTransform; // 거인의 Transform
    public float raycastDistance = 50f; // 레이캐스트 거리
    public Color raycastDebugColor = Color.red; // 레이캐스트 디버그 선의 색상

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Giant"))
        {
            Vector3 directionToGiant = (giantTransform.position - transform.position).normalized;
            RaycastHit hit;

            // 레이캐스트 디버그 선 그리기
            Debug.DrawLine(transform.position, transform.position + directionToGiant * raycastDistance, raycastDebugColor, 2f);

            if (Physics.Raycast(transform.position, directionToGiant, out hit, raycastDistance))
            {
                if (hit.transform == giantTransform)
                {
                    // 거인에게 도달했을 경우의 로직
                    NotifyGiant(hit.transform.gameObject);
                }
            }
        }
    }

    void NotifyGiant(GameObject giant)
    {
       // giant.GetComponent<CharacterAgent>().MoveToAndInteract(transform.position);
    }
}

