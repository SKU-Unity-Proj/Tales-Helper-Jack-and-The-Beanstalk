using UnityEngine;

public class WoodenPlankScript : MonoBehaviour
{
    public Transform targetPosition; // 이동할 목표 위치
    public float moveSpeed = 2f;   // 이동 속도

    private bool shouldMove = false;

    private void OnEnable()
    {
        HingeScript.OnHingeDestroyed += StartMoving; // 경첩 이벤트 구독
    }

    private void OnDisable()
    {
        HingeScript.OnHingeDestroyed -= StartMoving; // 경첩 이벤트 해제
    }

    private void StartMoving()
    {
        shouldMove = true;
    }

    private void Update()
    {
        if (shouldMove)
        {
            // 나무판자를 천천히 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

            // 목표 위치에 도달하면 움직임 멈춤
            if (transform.position == targetPosition.position)
            {
                shouldMove = false;
            }
        }
    }
}
