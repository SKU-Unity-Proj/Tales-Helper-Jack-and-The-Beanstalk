using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LfitController : MonoBehaviour
{
    public float maxHeight = 5f; // 최대 높이
    public float minHeight = 1f; // 가장 낮은 높이
    public float midHeight; // 중간 높이

    public float movementSpeed = 1f; // 이동 속도

    public int targetHeightIndex = 0; // 목표 높이 인덱스
    private int touchingObjects = 0; // 현재 닿고 있는 오브젝트의 수

    private void Update()
    {
        // 목표 높이에 따라서 움직임 처리
        float targetHeight;
        switch (targetHeightIndex)
        {
            case 0:
                targetHeight = maxHeight;
                break;
            case 1:
                targetHeight = midHeight;
                break;
            case 2:
                targetHeight = minHeight;
                break;
            default:
                targetHeight = maxHeight;
                break;
        }

        // 목표 높이까지 천천히 이동
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetHeight, transform.position.z), step);
    }

    private void OnCollisionEnter(Collision collision)
    {
        touchingObjects++;
        UpdateTargetHeightIndex();
    }

    private void OnCollisionExit(Collision collision)
    {
        touchingObjects--;
        UpdateTargetHeightIndex();
    }

    private void UpdateTargetHeightIndex()
    {
        if (touchingObjects == 1)
            targetHeightIndex = 1;

        else if(touchingObjects == 2)
            targetHeightIndex = 2;

        else
            targetHeightIndex = 0;
    }
}
