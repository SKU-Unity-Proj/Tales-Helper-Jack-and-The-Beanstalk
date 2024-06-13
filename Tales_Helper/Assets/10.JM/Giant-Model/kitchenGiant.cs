using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;

public class kitchenGiant : MonoBehaviour
{
    public enum State { Cleaning, Chasing, Attacking, Searching }
    public State currentState;

    private NavMeshAgent agent;
    private Animator _animator;
    public Transform player;

    public float sightRadius = 10f;
    public float soundRadius = 5f;
    public float sightAngle = 120f; // 시야각
    public float loseSightRadius = 15f; // 플레이어를 잃어버리고 청소 위치로 돌아가는 범위

    private Transform cleaningPosition;
    private bool isWaiting = false;
    private Coroutine currentCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        agent.autoTraverseOffMeshLink = false;

        currentState = State.Cleaning;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Cleaning:
                Clean();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
            case State.Searching:
                Search();
                break;
        }

        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(TraverseLinkWithBezierCurve());
        }

        CheckPlayerVisibility();
    }

    IEnumerator TraverseLinkWithBezierCurve()
    {
        OffMeshLinkData linkData = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = linkData.endPos + Vector3.up * agent.baseOffset;

        // 중간 제어점 계산
        Vector3 controlPoint1 = startPos + (Vector3.up * 2.0f); // 위로 솟구치게 하는 제어점
        Vector3 controlPoint2 = endPos + (Vector3.up * 2.0f);

        SetAnimationState("Jump up");  // 점프 애니메이션 시작

        float duration = Vector3.Distance(startPos, endPos) / agent.speed;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float t = normalizedTime;
            // 비저 곡선 공식
            Vector3 position = (1 - t) * (1 - t) * (1 - t) * startPos +
                               3 * (1 - t) * (1 - t) * t * controlPoint1 +
                               3 * (1 - t) * t * t * controlPoint2 +
                               t * t * t * endPos;
            agent.transform.position = position;

            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        SetAnimationState("Jump down");  // 착지 애니메이션 시작
        agent.CompleteOffMeshLink();
    }

    void Clean()
    {
        if (cleaningPosition != null && !isWaiting)
        {
            agent.SetDestination(cleaningPosition.position);

            if (Vector3.Distance(transform.position, cleaningPosition.position) < 1f)
            {
                if (!isWaiting)
                {
                    if (currentCoroutine != null)
                    {
                        StopCoroutine(currentCoroutine);
                    }
                    currentCoroutine = StartCoroutine(WaitAndClean());
                }
            }
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);

        // 플레이어와의 거리가 loseSightRadius보다 크면 청소 상태로 돌아갑니다.
        if (Vector3.Distance(transform.position, player.position) > loseSightRadius)
        {
            Debug.Log($"{gameObject.name}이 플레이어를 잃어버리고 청소 위치로 돌아갑니다.");
            currentState = State.Cleaning;
            agent.SetDestination(cleaningPosition.position);
        }
    }

    void Attack()
    {
        // 공격 로직 구현
    }

    void Search()
    {
        if (!isWaiting)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(WaitAndReturnToClean());
        }
    }

    void CheckPlayerVisibility()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= sightRadius)
        {
            float angleBetweenGiantAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleBetweenGiantAndPlayer <= sightAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRadius))
                {
                    if (hit.transform == player)
                    {
                        Debug.Log($"{gameObject.name}이 시야 반경 내에서 플레이어를 감지했습니다.");
                        if (currentCoroutine != null)
                        {
                            StopCoroutine(currentCoroutine);
                        }
                        isWaiting = false;
                        currentState = State.Chasing;
                        return;
                    }
                }
            }
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform == player)
            {
                Debug.Log($"{gameObject.name}이 소리 반경 내에서 플레이어를 감지했습니다.");
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                isWaiting = false;
                currentState = State.Chasing;
                return;
            }
        }
    }

    public void SetCleaningPosition(Transform position)
    {
        cleaningPosition = position;
        Debug.Log($"{gameObject.name}가 새로운 청소 위치로 이동합니다: {cleaningPosition.name}");
    }

    public Transform GetCleaningPosition()
    {
        return cleaningPosition;
    }

    private IEnumerator WaitAndClean()
    {
        isWaiting = true;
        Debug.Log($"{gameObject.name}가 청소 중입니다: {cleaningPosition.name}");
        while (true)
        {
            if (CheckPlayerVisibilityRoutine())
            {
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        isWaiting = false;
    }

    private IEnumerator WaitAndReturnToClean()
    {
        isWaiting = true;
        Debug.Log($"{gameObject.name}가 청소 위치로 복귀합니다: {cleaningPosition.name}");
        while (true)
        {
            if (CheckPlayerVisibilityRoutine())
            {
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        isWaiting = false;
        currentState = State.Cleaning;
    }

    bool CheckPlayerVisibilityRoutine()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= sightRadius)
        {
            float angleBetweenGiantAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleBetweenGiantAndPlayer <= sightAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRadius))
                {
                    if (hit.transform == player)
                    {
                        Debug.Log($"{gameObject.name}이 청소 도중 시야 반경 내에서 플레이어를 감지했습니다.");
                        currentState = State.Chasing;
                        return true;
                    }
                }
            }
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform == player)
            {
                Debug.Log($"{gameObject.name}이 청소 도중 소리 반경 내에서 플레이어를 감지했습니다.");
                currentState = State.Chasing;
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        // 소리 범위 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, soundRadius);

        // 시야각 표시
        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.Euler(0, -sightAngle / 2, 0) * transform.forward * sightRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, sightAngle / 2, 0) * transform.forward * sightRadius;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }


    private void SetAnimationState(string stateName, float transitionDuration = 0.1f, int StateLayer = 0)
    {
        if (_animator.HasState(StateLayer, Animator.StringToHash(stateName)))
        {
            _animator.CrossFadeInFixedTime(stateName, transitionDuration, StateLayer);

            if (StateLayer == 1)
                SetLayerPriority(1, 1);
        }
    }

    private void SetLayerPriority(int StateLayer = 1, int Priority = 1) // 애니메이터의 레이어 우선순위 값(무게) 설정
    {
        _animator.SetLayerWeight(StateLayer, Priority);
    }
}
