using UnityEngine;

public class WoodenPlankScript : MonoBehaviour
{
    public Transform targetPosition; // �̵��� ��ǥ ��ġ
    public float moveSpeed = 2f;   // �̵� �ӵ�

    private bool shouldMove = false;

    private void OnEnable()
    {
        HingeScript.OnHingeDestroyed += StartMoving; // ��ø �̺�Ʈ ����
    }

    private void OnDisable()
    {
        HingeScript.OnHingeDestroyed -= StartMoving; // ��ø �̺�Ʈ ����
    }

    private void StartMoving()
    {
        shouldMove = true;
    }

    private void Update()
    {
        if (shouldMove)
        {
            // �������ڸ� õõ�� �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

            // ��ǥ ��ġ�� �����ϸ� ������ ����
            if (transform.position == targetPosition.position)
            {
                shouldMove = false;
            }
        }
    }
}
