using UnityEngine;

public class GuideEffect : MonoBehaviour
{
    public float moveSpeed = 3.0f; // ����Ʈ �̵� �ӵ�
    public float disappearDistance = 2.0f; // ����Ʈ�� ����� �÷��̾���� �Ÿ�
    public float destroyDelay = 5.0f; // �÷��̾� ���� �� ����Ʈ�� ������������ �ð�
    private Transform target;
    private GameObject player;
    private bool playerReachedTarget = false;

    // ��ǥ ��ġ ����
    public void SetTarget(Transform targetPosition)
    {
        target = targetPosition;
    }

    void Start()
    {
        // �÷��̾� ã��
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (target != null && !playerReachedTarget)
        {
            // ��ǥ ��ġ�� �̵�
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // �÷��̾ ��ǥ ��ġ�� �����ߴ��� Ȯ��
            if (player != null && Vector3.Distance(player.transform.position, target.position) < disappearDistance)
            {
                playerReachedTarget = true;
                // ��ǥ ��ġ�� ������ �� 5�� �Ŀ� ����Ʈ ����
                Invoke("DestroyEffect", destroyDelay);
            }
        }
    }

    // ����Ʈ ���� �Լ�
    void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
