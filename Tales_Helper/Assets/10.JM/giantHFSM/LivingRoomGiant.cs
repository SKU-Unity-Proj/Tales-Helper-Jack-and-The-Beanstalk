using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class LivingRoomGiant : MonoBehaviour
{
    private CharacterAgent giantAgent; // CharacterAgent 스크립트 참조
    private NavMeshAgent navAgent;
    private Animator anim;
    private RestCondition restCondition;

    public enum GiantState { Idle, Wander, Chase, Search, Attack, Sitting }
    public GiantState currentState;

    [Header("Wander and Rest Settings")]
    [SerializeField] private float Wander_Range = 10f;
    [SerializeField] private Transform chairTransform; // 의자 Transform

    public float sightRadius = 10f; // 시야 거리
    public float attackRadius = 2f; // 공격 범위
    public float chaseRadius = 15f; // 추적 거리
    private int wanderCount;
    private int maxWanderCount = 50;
    private int minWanderCount = 45;
    private Vector3 sittingPosition; // 거인이 앉을 위치
    private Transform player; // 플레이어의 위치

    private bool hasInteracted = false;
    private bool isStandingUp = false; // 일어나는 중인지 여부
    private bool isWandering = false; // 순찰 중인지 여부

    private float chaseTimer; // 추적 시간 타이머
    private float maxChaseDuration = 6f; // 추적 실패 시 6초 후 순찰 상태로 전환

    void Start()
    {
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        giantAgent = GetComponent<CharacterAgent>();
        restCondition = GetComponent<RestCondition>();

        currentState = GiantState.Idle;
        wanderCount = Random.Range(minWanderCount, maxWanderCount); // 순찰 횟수 설정
    }

    void Update()
    {
        switch (currentState)
        {
            case GiantState.Idle:
                IdleState();
                break;
            case GiantState.Wander:
                WanderState();
                break;
            case GiantState.Chase:
                ChaseState();
                break;
            case GiantState.Search:
                SearchState();
                break;
            case GiantState.Attack:
                AttackState();
                break;
            case GiantState.Sitting:
                SittingState();
                break;
        }

        CheckTargetVisibility(); // 플레이어 감지 여부 확인
    }

    // 상태별 메소드

    void IdleState()
    {
        if (wanderCount > 0)
        {
            currentState = GiantState.Wander;
        }
    }

    void WanderState()
    {
        // 목적지에 도착했을 경우 새로운 위치로 이동
        if (HasReachedDestination())
        {
            // 새로운 랜덤 위치로 이동
            Vector3 location = giantAgent.PickLocationInRange(Wander_Range);
            anim.SetBool("Run", false); // 순찰 중일 때 달리기 애니메이션
            giantAgent.MoveTo(location);
            player = null; // 순찰 중 플레이어가 없음을 명시
        }
        else
        {
            // 아직 목적지에 도달하지 않은 경우는 이동 중 상태 유지
            Debug.Log("Moving to destination...");
        }

        // 휴식 조건 확인
        restCondition.UpdateTimer(Time.deltaTime);
        if (restCondition.CheckCondition())
        {
            currentState = GiantState.Sitting; // 휴식 조건이 만족되면 앉는 상태로 전환
            Debug.Log("Rest condition met. Switching to Sitting state.");
        }
    }

    void ChaseState()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 추적 범위 내에서 플레이어 추적
        if (distanceToPlayer <= attackRadius)
        {
            currentState = GiantState.Attack; // 공격 상태로 전환
        }
        else if (distanceToPlayer <= chaseRadius)
        {
            anim.SetBool("Run", true); // 추적 애니메이션 실행
            giantAgent.MoveToRun(player.position); // 플레이어를 추격
            Debug.Log($"Chasing player..."); // 추적 중인 상태 로그 출력
        }
        else
        {
            // 추적 범위를 벗어난 경우, 추적 실패 시간을 증가시킴
            chaseTimer += Time.deltaTime;
            Debug.Log($"Player out of range. Chase Timer: {chaseTimer} / {maxChaseDuration}"); // 타이머 로그 출력

            // 추적 실패 시간이 초과되면 순찰로 전환
            if (chaseTimer >= maxChaseDuration)
            {
                currentState = GiantState.Wander;
                anim.SetBool("Run", false); // 추적 애니메이션 실행
                player = null; // 추적 실패 시 플레이어를 null로 설정
                chaseTimer = 0f; // 타이머 리셋
                Debug.Log("Player lost, returning to Wander state.");
            }
        }
    }


    void SearchState()
    {
        // 떨어진 물체를 탐색하는 로직
        giantAgent.SearchingObject();

        if (giantAgent.AtDestination)
        {
            currentState = GiantState.Idle; // 탐색 후 다시 Idle 상태로 돌아감
        }
    }

    void AttackState()
    {
        if (player == null) return;

        // 공격 범위 내에 있으면 공격
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRadius)
        {
            anim.SetTrigger("Attack"); // 공격 애니메이션 트리거
            giantAgent.AttackToPlayer(player.gameObject);
        }
        else
        {
            currentState = GiantState.Chase; // 공격 후 플레이어가 범위 밖에 있으면 다시 추적 상태로 전환
        }
    }

    void SittingState()
    {
        //giantAgent.SitAtPosition(chairTransform.position, chairTransform.rotation); // 앉는 상태로 전환

        if (!isStandingUp && anim.GetBool("Sitting") && player != null)
        {
            isStandingUp = true; // 거인이 일어나는 중임을 표시
            giantAgent.StandUpChase(); // 거인을 일으켜 세우는 동작 실행
            restCondition.ResetCondition(); // 휴식 상태 초기화
            StartCoroutine(TransitionToChase());
            return;
        }

        if (!HasReachedDestination())
        {
            anim.SetBool("Run", false); // 앉을 때는 뛰는 애니메이션 끄기
            player = null; // 앉는 상태에서 플레이어를 null로 설정하여 감지를 중단
            giantAgent.SitAtPosition(chairTransform.position, chairTransform.rotation); // 앉는 상태로 전환
        }
    }

    // 플레이어 감지 로직
    void CheckTargetVisibility()
    {
        foreach (var target in DetectableTargetManager.Instance.AllTargets)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            if (distanceToTarget <= attackRadius)
            {
                player = target.transform;
                if (currentState == GiantState.Sitting && !isStandingUp)
                {
                    isStandingUp = true;
                    giantAgent.StandUpChase();
                    restCondition.ResetCondition();
                    StartCoroutine(TransitionToChase());
                }
                else
                {
                    currentState = GiantState.Chase;
                    chaseTimer = 0f; // 추적 상태로 전환할 때 타이머 초기화
                }
                return;
            }
        }
    }

    // 자연스럽게 추적 상태로 전환하는 코루틴
    private IEnumerator TransitionToChase()
    {
        yield return new WaitUntil(() => anim.GetBool("Sitting") == false); // 거인이 일어날 때까지 대기
        isStandingUp = false; // 일어나는 동작 완료
        currentState = GiantState.Chase; // 추적 상태로 전환
    }

    public bool HasReachedDestination()
    {
        // NavMeshAgent의 남은 거리와 도착 여부를 기반으로 도착했는지 확인
        if (!navAgent.pathPending) // 경로가 계산 중이 아닐 때
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                // NavMeshAgent가 목적지에 거의 도착했고, 장애물에 막히지 않은 경우
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                {
                    return true; // 목적지에 도착
                }
            }
        }
        return false; // 아직 목적지에 도착하지 않음
    }
}



#if UNITY_EDITOR
[CanEditMultipleObjects]  // 다중 오브젝트 편집 지원
[CustomEditor(typeof(LivingRoomGiant))]
public class LivingRoomGiantEditor : Editor
{
    public void OnSceneGUI()
    {
        // target을 LivingRoomGiant 타입으로 캐스팅
        var giant = (LivingRoomGiant)target;

        if (giant == null) return; // 널 체크

        // 시야 범위 (Vision Cone)
        Handles.color = new Color(1f, 0f, 0f, 0.25f); // 빨간색, 투명도 0.25
        Vector3 startPoint = Mathf.Cos(-giant.sightRadius * Mathf.Deg2Rad) * giant.transform.forward +
                             Mathf.Sin(-giant.sightRadius * Mathf.Deg2Rad) * giant.transform.right;
        Handles.DrawSolidArc(giant.transform.position, Vector3.up, startPoint, giant.sightRadius * 2f, giant.sightRadius);

        // 공격 범위
        Handles.color = new Color(0f, 1f, 0f, 0.25f); // 초록색, 투명도 0.25
        Handles.DrawSolidDisc(giant.transform.position, Vector3.up, giant.attackRadius);

        // 추적 범위
        Handles.color = new Color(0f, 0f, 1f, 0.25f); // 파란색, 투명도 0.25
        Handles.DrawSolidDisc(giant.transform.position, Vector3.up, giant.chaseRadius);
    }
}
#endif // UNITY_EDITOR



