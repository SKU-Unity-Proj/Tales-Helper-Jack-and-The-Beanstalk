using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class kitchenGiant : MonoBehaviour
{
    public enum State { Cleaning, Chasing, Attacking, Searching }
    public State currentState;

    public int index; // 이 거인의 인덱스

    public kitchenGiantManager manager; // 메니저 참조
    private NavMeshAgent agent;
    private Animator _animator;
    public Transform player;

    public Collider handCollider; // 손에 있는 스피어 콜라이더 참조

    public float sightRadius = 10f;
    public float soundRadius = 5f;
    public float sightAngle = 120f; // 시야각
    public float loseSightRadius = 15f; // 플레이어를 잃어버리고 청소 위치로 돌아가는 범위
    public float rotationSpeed = 90.0f; // 초당 90도 회전

    public float attackRadius = 2.0f; // 공격 범위
    public float attackCooldown = 1.5f; // 공격 쿨다운

    private Transform cleaningPosition;
    private bool isWaiting = false;
    private Coroutine currentCoroutine;
    private bool canAttack = true; // 공격 가능 여부 플래그

    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        agent.autoTraverseOffMeshLink = false;

        currentState = State.Cleaning;

        DisableCollider(); // 시작할 때 콜라이더를 비활성화
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

        Vector3 controlPoint1 = startPos + (Vector3.up * 0.5f); // 높이를 줄여 덜 드라마틱하게 조정
        Vector3 controlPoint2 = endPos + (Vector3.up * 0.5f);

        
        _animator.SetBool("islanded", true);  // 점프 애니메이션 시작

        //agent.updatePosition = false; // 에이전트의 위치 업데이트를 스크립트에서 제어

        float duration = Vector3.Distance(startPos, endPos) / agent.speed;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float t = normalizedTime;
            Vector3 position = BezierCurve(startPos, controlPoint1, controlPoint2, endPos, t);
            agent.transform.position = position;

            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        _animator.SetBool("islanded", false);  // 착지 애니메이션 시작
        yield return new WaitForSeconds(0.1f); // 착지 애니메이션을 보장하기 위한 대기 시간

        agent.speed = 0f;  // 착지할 때 속도를 임시로 0으로 설정
        //agent.updatePosition = true; // 에이전트의 위치 업데이트를 다시 활성화
        
        agent.CompleteOffMeshLink();
        agent.speed = 3.5f; // 속도를 원래대로 복구


    }

    Vector3 BezierCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        return Mathf.Pow(1 - t, 3) * start +
               3 * Mathf.Pow(1 - t, 2) * t * control1 +
               3 * (1 - t) * Mathf.Pow(t, 2) * control2 +
               Mathf.Pow(t, 3) * end;
    }

    void Clean()
    {
        if (cleaningPosition != null && !isWaiting)
        {
            agent.SetDestination(cleaningPosition.position);
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                if (!isWaiting)
                {
                    if (currentCoroutine != null)
                    {
                        StopCoroutine(currentCoroutine);
                    }
                    currentCoroutine = StartCoroutine(WaitAndClean());

                    // 청소 위치에 도착하면, 메니저를 통해 방향을 가져와서 설정
                    Vector3 targetDirection = manager.GetCleaningDirection(index, this.index);
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    _animator.SetBool("isCleaning", true);
                }
            }
        }
    }
        void Chase()
    {
        _animator.SetBool("isCleaning", false);
        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRadius)
        {
            // 플레이어가 공격 범위에 들어오면 공격 상태로 전환
            currentState = State.Attacking;
        }
        else if (distanceToPlayer > loseSightRadius)
        {
            Debug.Log($"{gameObject.name}이 플레이어를 잃어버리고 청소 위치로 돌아갑니다.");
            currentState = State.Cleaning;
            agent.SetDestination(cleaningPosition.position);
        }

    }

    void Attack()
    {
        // 공격 가능한 상태인지 확인
        if (!canAttack) return;

        // 하체는 계속 움직이면서 상체로만 공격 애니메이션 재생
        //_animator.SetLayerWeight(1, 1); // Upper Body Layer의 우선순위 설정
        _animator.SetTrigger("Attack"); // 상체 공격 애니메이션 트리거

        EnableCollider();

        canAttack = false; // 공격이 진행 중이므로 다시 공격하지 못하게 설정
        StartCoroutine(AttackCooldown()); // 쿨다운 시작

        // 플레이어를 계속 추적 (하체 애니메이션으로 이동)
        agent.SetDestination(player.position);

        // 공격이 끝나면 다시 추적 상태로 돌아갑니다.
        if (Vector3.Distance(transform.position, player.position) > attackRadius)
        {
            currentState = State.Chasing;
            DisableCollider();
        }
    }

    IEnumerator AttackCooldown()
    {
        // 플레이어에게 데미지 적용
        //player.GetComponent<PlayerHealth>().TakeDamage(10); // 임의로 10의 데미지를 줌, 플레이어에 맞는 TakeDamage 함수 구현 필요

        yield return new WaitForSeconds(attackCooldown); // 공격 쿨다운 대기
        canAttack = true; // 다시 공격 가능하게 설정
    }


    // 공격 애니메이션의 시작 부분에서 이 함수를 호출
    public void EnableCollider()
    {
        handCollider.enabled = true; // 공격 시작 시 콜라이더 활성화
        Debug.Log("Collider Enabled");
    }

    // 공격 애니메이션의 끝 부분에서 이 함수를 호출
    public void DisableCollider()
    {
        handCollider.enabled = false; // 공격이 끝난 후 콜라이더 비활성화
        Debug.Log("Collider Disabled");
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

                // 플레이어와의 거리를 계산
                float playerDistance = Vector3.Distance(transform.position, player.position);

                // 공격 범위 내에 있는지 확인
                if (playerDistance <= attackRadius)
                {
                    Debug.Log($"{gameObject.name}이 공격 범위 내에 플레이어를 감지했습니다.");
                    currentState = State.Attacking; // 공격 상태로 전환
                }
                else
                {
                    // 공격 범위 밖에 있으면 추적 상태로 전환
                    Debug.Log($"{gameObject.name}이 추적 상태로 전환합니다.");
                    currentState = State.Chasing; // 추적 상태로 전환
                }

                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                isWaiting = false;
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
