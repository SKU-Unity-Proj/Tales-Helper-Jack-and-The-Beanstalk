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

    public enum GiantState { Idle, Wander, Chase, Search, Attack, Sitting, Knocking }
    public GiantState currentState;

    [Header("Wander and Rest Settings")]
    [SerializeField] private float Wander_Range = 10f;
    [SerializeField] private Transform chairTransform; // 의자 Transform
    [SerializeField] private Transform doorPos;
    [SerializeField] private Transform doorCol;

    public float sightRadius = 10f; // 시야 거리
    public float attackRadius = 2f; // 공격 범위
    public float chaseRadius = 15f; // 추적 거리

    private int wanderCount;
    private int maxWanderCount = 50;
    private int minWanderCount = 45;

    private Transform player; // 플레이어의 위치

    public bool ischase = false;
    public bool forceChase = false;

    private bool hasInteracted = false;
    private bool isStandingUp = false; // 일어나는 중인지 여부
    private bool isWandering = false; // 순찰 중인지 여부
    private bool isSearching = false; // 서치 상태 여부 플래그

    private float chaseTimer; // 추적 시간 타이머
    private float maxChaseDuration = 6f; // 추적 실패 시 6초 후 순찰 상태로 전환

    private bool isPaused = false; // 퍼즈 상태 플래그

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


        //if (isPaused) return; // 퍼즈 상태일 때 업데이트 중단

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
            case GiantState.Knocking: // 새롭게 추가된 Knocking 상태
                KnockingState();
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
            //Debug.Log("Moving to destination...");
        }

        // 휴식 조건 확인
        restCondition.UpdateTimer(Time.deltaTime);
        if (restCondition.CheckCondition())
        {
            currentState = GiantState.Sitting; // 휴식 조건이 만족되면 앉는 상태로 전환
            //Debug.Log("Rest condition met. Switching to Sitting state.");
        }


        bool specialconditionMet = DroppedObject.Instance.CheckSpecialObjectCondition();
        //Debug.Log(specialconditionMet);
        // 특별 조건이 발동되었는지 확인
        if (specialconditionMet == true)
        {
            // 일으켜 세우는 동작 없이 바로 추적 상태로 전환
            currentState = GiantState.Knocking; // 즉시 추적 상태로 전환

            //Debug.Log("Special condition met while sitting. Standing up and chasing player.");
            return;
        }
    }

    void ChaseState()
    {
        if (player == null) return; // 플레이어가 없으면 반환

        ischase = true;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // 플레이어 위치에 대한 경로 설정 및 추적
        if (distanceToPlayer <= attackRadius)
        {
            currentState = GiantState.Attack; // 공격 상태로 전환
            //chaseTimer = 0f; // 공격 상태로 전환되면 타이머 리셋
        }
        else if (distanceToPlayer <= chaseRadius)
        {
            // 추적 애니메이션 실행
            anim.SetBool("Run", true);

            // NavMeshAgent를 통해 플레이어 추적
            giantAgent.MoveToRun(player.position);

            // NavMeshAgent의 경로가 유효하지 않거나 플레이어가 보이지 않는 경우, 경로를 재계산
            if (navAgent.pathStatus == NavMeshPathStatus.PathInvalid || navAgent.remainingDistance > chaseRadius)
            {
                // 경로가 유효하지 않으면 경로를 다시 설정
                navAgent.ResetPath();
                giantAgent.MoveToRun(player.position);
            }

            //Debug.Log($"Chasing player..."); // 추적 중인 상태 로그 출력
        }
        else
        {
            // 추적 범위를 벗어난 경우, 추적 실패 시간을 증가시킴
            chaseTimer += Time.deltaTime;
            //Debug.Log($"Player out of range. Chase Timer: {chaseTimer} / {maxChaseDuration}"); // 타이머 로그 출력

            // 추적 실패 시간이 초과되면 순찰로 전환
            if (chaseTimer >= maxChaseDuration)
            {
                ischase = false;
                anim.SetBool("Run", false);
                currentState = GiantState.Wander;
                chaseTimer = 0f; // 타이머 리셋
                //Debug.Log("Player lost, returning to Wander state.");
            }
        }

        bool specialconditionMet = DroppedObject.Instance.CheckSpecialObjectCondition();
        //Debug.Log(specialconditionMet);
        // 특별 조건이 발동되었는지 확인
        if (specialconditionMet == true)
        {
            // 일으켜 세우는 동작 없이 바로 추적 상태로 전환
            currentState = GiantState.Knocking; // 즉시 추적 상태로 전환

            Debug.Log("Special condition met while sitting. Standing up and chasing player.");
            return;
        }
    }

    void SearchState()
    {
        // 서치 상태 시작
        isSearching = true;

        // 이미 상호작용이 완료되었는지 확인
        if (hasInteracted)
        {
            isSearching = false; // 서치 상태 종료
            currentState = GiantState.Wander; // 순찰 상태로 전환
            return;
        }

        // 거인이 앉아 있는 상태라면 먼저 일어나서 탐색 상태로 전환
        if (anim.GetBool("Sitting"))
        {
            giantAgent.StandUpSearch(); // 서 있는 상태로 변경
            restCondition.ResetCondition(); // 휴식 상태 초기화
        }

        // 현재 탐색 중인 물체가 있는지 확인하고, 없으면 탐색 시작
        if (!giantAgent.IsSearching() && DroppedObject.Instance.GetDroppedObjectsCount() > 0)
        {
            //Debug.Log("Starting to search for dropped objects.");
            giantAgent.SearchingObject(); // 탐색 시작
            return;
        }

        // 탐색이 완료되었는지 확인
        if (giantAgent.IsSearching())
        {
            hasInteracted = true; // 상호작용 완료
            //Debug.Log("Interaction with dropped object complete.");
            isSearching = false; // 서치 상태 종료
            currentState = GiantState.Wander; // 순찰 상태로 전환
            return;
        }

        bool specialconditionMet = DroppedObject.Instance.CheckSpecialObjectCondition();
        //Debug.Log(specialconditionMet);
        if (specialconditionMet)
        {
            anim.SetBool("Run", true);
            anim.SetBool("SearchObj", false);
            isSearching = false; // 서치 상태 종료
            currentState = GiantState.Knocking; // Knocking 상태로 전환
            //Debug.Log("Special condition met. Switching to Knocking state.");
            return;
        }

        // 탐색 진행 중
        //Debug.Log("Searching in progress...");
    }



    void AttackState()
    {
        if (player == null) return;

        // 공격 범위 내에 있으면 공격
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRadius)
        {
            giantAgent.AttackPlayerInRange(player.gameObject);
        }
        else
        {
            giantAgent.MissingPlayer(player.transform.position);
            currentState = GiantState.Chase; // 공격 후 플레이어가 범위 밖에 있으면 다시 추적 상태로 전환
        }

        bool specialconditionMet = DroppedObject.Instance.CheckSpecialObjectCondition();
        // 특별 조건이 발동되었는지 확인
        if (specialconditionMet == true)
        {
            currentState = GiantState.Knocking; // 즉시 추적 상태로 전환
            return;
        }
    }

    void SittingState()
    {
        bool specialconditionMet = DroppedObject.Instance.CheckSpecialObjectCondition();

        // 1. 거인이 앉으러 가고 있는 도중 플레이어를 발견한 경우
        if (player != null && !isStandingUp && !anim.GetBool("Sitting")) // 아직 앉지 않았을 때
        {
            // 일으켜 세우는 동작 없이 바로 추적 상태로 전환
            currentState = GiantState.Chase; // 즉시 추적 상태로 전환
            //Debug.Log("Player detected while moving to sit. Switching to Chase state.");

            return;
        }

        // 2. 거인이 이미 앉아 있는 상태에서 플레이어를 발견한 경우
        if (player != null && anim.GetBool("Sitting")) // 이미 앉은 상태에서 플레이어를 발견
        {
            isStandingUp = true; // 거인이 일어나는 중임을 표시

            // 일반적인 추적 상태 전환
            giantAgent.StandUpChase(); // 일어서서 추적 동작 실행
            restCondition.ResetCondition(); // 휴식 상태 초기화

            anim.SetBool("Sitting", false); // 앉는 애니메이션 중단
            anim.SetBool("Run", true); // 추적 애니메이션 시작

            StartCoroutine(TransitionToChase()); // 추적 상태로 전환
            //Debug.Log("Player detected while sitting. Standing up and switching to Chase state.");
            return;
        }
        else
        {
            //Debug.Log(specialconditionMet);
            // 특별 조건이 발동되었는지 확인
            if (specialconditionMet == true)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, doorPos.rotation, Time.deltaTime * 5f);
                // 일으켜 세우는 동작 없이 바로 추적 상태로 전환
                currentState = GiantState.Knocking; // 즉시 추적 상태로 전환

                //Debug.Log("Special condition met while sitting. Standing up and chasing player.");
                return;
            }
        }

        // 3. 목적지(의자)에 도착하지 않았다면 계속 이동
        if (!HasReachedDestination())
        {
            anim.SetBool("Run", false); // 앉을 때는 뛰는 애니메이션 끄기
            giantAgent.SitAtPosition(chairTransform.position, chairTransform.rotation); // 의자까지 이동
        }
        else
        {
            // 도착한 경우에만 앉기 애니메이션 실행
            anim.SetBool("Sitting", true);
        }

        bool conditionMet = DroppedObject.Instance.CheckCondition();
        if (conditionMet == true)
        {
            currentState = GiantState.Search; // 즉시 추적 상태로 전환
        }

    }

    // Knocking 상태 처리
    void KnockingState()
    {

        // 앉기 상태를 강제로 중단하고 목표로 문 위치를 설정
        anim.SetBool("Sitting", false);
        anim.SetBool("Run", true);

        if (anim.GetBool("Sitting"))
        {
            giantAgent.StandUpChase(); // 일어서서 추적 동작 실행
            giantAgent.MoveToRun(doorPos.position); // 이동 시작
            //transform.rotation = Quaternion.Lerp(transform.rotation, doorPos.rotation, Time.deltaTime * 5f);
            StartCoroutine(DelayedWarp(2.5f, doorPos.position)); // 1.5초 후 워프 실행

        }

        if (!anim.GetBool("Sitting"))
        {
           // transform.rotation = Quaternion.Lerp(transform.rotation, doorPos.rotation, Time.deltaTime * 5f);
            giantAgent.MoveToRun(doorPos.position); // 이동 시작
            StartCoroutine(DelayedWarp(2.5f, doorPos.position)); // 1.5초 후 워프 실행

        }

        if (navAgent.CalculatePath(doorPos.position, new NavMeshPath()))
        {
            Debug.Log("Moving to door...");
            navAgent.isStopped = false;
            giantAgent.MoveToRun(doorPos.position);
            doorCol.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            Debug.LogError("No valid path to door.");
        }

        if (giantAgent.IsKnockingDoor)
        {
            giantAgent.StartKnockingDoor(doorPos.position, doorPos.rotation);
        }
    }
    private IEnumerator DelayedWarp(float delay, Vector3 targetPosition)
    {
        yield return new WaitForSeconds(delay); // 1.5초 딜레이
        navAgent.Warp(targetPosition); // 강제로 위치 이동
        navAgent.stoppingDistance = 0f; // 스톱핑 거리 설정
        Debug.Log("Warped to target position.");
    }

    // 플레이어 감지 로직
    void CheckTargetVisibility()
    {
        if (currentState == GiantState.Knocking)
            return;

        // 만약 서치 상태라면 플레이어 감지 중단
        if (isSearching) return;

        foreach (var target in DetectableTargetManager.Instance.AllTargets)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            // 먼저 공격 범위 내에 있는지 확인
            if (distanceToTarget <= attackRadius)
            {
                player = target.transform; // 타겟을 플레이어로 설정
                currentState = GiantState.Attack; // 공격 상태로 전환
                Debug.Log("Player detected in attack range. Switching to Attack state.");
                return; // 공격 상태로 전환되었으므로 루프 종료
            }

            // 공격 범위에 있지 않으면 추적 범위 내에서 추적
            else if (distanceToTarget <= chaseRadius)
            {
                player = target.transform; // 타겟을 플레이어로 설정
                if (currentState == GiantState.Sitting && !isStandingUp)
                {
                    // 일어서서 추적 상태로 전환
                    isStandingUp = true;
                    //giantAgent.StandUpChase();
                    restCondition.ResetCondition();
                    StartCoroutine(TransitionToChase());
                }
                else
                {
                    currentState = GiantState.Chase; // 추적 상태로 전환
                    chaseTimer = 0f; // 추적 상태로 전환할 때 타이머 초기화
                    Debug.Log("Player detected in chase range. Switching to Chase state.");
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
    private IEnumerator DisableAndEnableGiant(float delay)
    {
        // 거인 비활성화
        gameObject.SetActive(false);
        Debug.Log("Giant disabled.");

        // 딜레이 후 재활성화
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(true);
        Debug.Log("Giant enabled.");
    }

    private void SetAnimationState(string stateName, float transitionDuration = 0.1f, int StateLayer = 0)
    {
        if (anim.HasState(StateLayer, Animator.StringToHash(stateName)))
        {
            anim.CrossFadeInFixedTime(stateName, transitionDuration, StateLayer);

            if (StateLayer == 1)
                SetLayerPriority(1, 1);
        }

    }

    private void SetLayerPriority(int StateLayer = 1, int Priority = 1) // 애니메이터의 레이어 우선순위 값(무게) 설정
    {
        anim.SetLayerWeight(StateLayer, Priority);
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



