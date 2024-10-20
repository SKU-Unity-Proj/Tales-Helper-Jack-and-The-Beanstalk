using UnityEngine;

public class GuideEffect : MonoBehaviour
{
    public float moveSpeed = 3.0f; // 이펙트 이동 속도
    public float disappearDistance = 2.0f; // 이펙트가 사라질 플레이어와의 거리
    public float destroyDelay = 5.0f; // 플레이어 도착 후 이펙트가 사라지기까지의 시간
    private Transform target;
    private GameObject player;
    private bool playerReachedTarget = false;

    // 목표 위치 설정
    public void SetTarget(Transform targetPosition)
    {
        target = targetPosition;
    }

    void Start()
    {
        // 플레이어 찾기
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (target != null && !playerReachedTarget)
        {
            // 목표 위치로 이동
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 플레이어가 목표 위치에 도착했는지 확인
            if (player != null && Vector3.Distance(player.transform.position, target.position) < disappearDistance)
            {
                playerReachedTarget = true;
                // 목표 위치에 도착한 후 5초 후에 이펙트 제거
                Invoke("DestroyEffect", destroyDelay);
            }
        }
    }

    // 이펙트 제거 함수
    void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
