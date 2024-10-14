using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EOffmeshLinkStatus
{
    NotStarted,
    InProgress
}

// 캐릭터(거인)의 이동 및 행동 관리를 위한 스크립트
[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAgent : CharacterBase
{
    [SerializeField] public float NearestPointSearchRange = 5f; // 최근접 탐색 범위
    [SerializeField] Transform attackCol;

    private NavMeshAgent Agent; // NavMeshAgent 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트
    private LivingRoomGiant isDestination; // 애니메이터 컴포넌트

    private float walkSpeed = 2.5f; // 걷기 속도
    private float runSpeed = 6.5f; // 뛰기 속도

    bool DestinationSet = false; // 목적지 설정 여부
    bool ReachedDestination = false; // 목적지 도달 여부

    EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted; // OffmeshLink 상태

    public bool IsMoving => Agent.velocity.magnitude > float.Epsilon; // 이동 중 여부
    public bool AtDestination => ReachedDestination; // 목적지 도달 여부 반환

    public bool IsKnockingDoor { get; private set; } = false;

    // Start is called before the first frame update
    protected void Start()
    {
        Agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 초기화
        anim = GetComponent<Animator>(); // Animator 컴포넌트 초기화
        isDestination = GetComponent<LivingRoomGiant>();

        Agent.autoRepath = true; // 경로가 유효하지 않을 때 자동으로 경로를 재계산
        Agent.stoppingDistance = 1.5f; // 플레이어와의 적절한 거리를 설정

        // 거인 트렌스폼이 필요할 시 이거 쓰면 됨.
        /* 
        // 거인의 Transform을 가져옴
        Transform giantTransform = this.transform;
        // DroppedObjectManager의 인스턴스를 찾아서 SetGiantTransform 호출
        DroppedObject.Instance.SetGiantTransform(giantTransform);
        */

    }

    // Update is called once per frame
    protected void Update()
    {
        // 목적지 설정되었고, 경로 계산 완료 및 OffMeshLink가 아니고, 목적지에 근접했는지 확인
        if (!Agent.pathPending && !Agent.isOnOffMeshLink && DestinationSet && (Agent.remainingDistance <= Agent.stoppingDistance))
        {
            DestinationSet = false;
            ReachedDestination = true;
        }

        // OffMeshLink 상태 확인 및 처리
        if (Agent.isOnOffMeshLink)
        {
            if (OffMeshLinkStatus == EOffmeshLinkStatus.NotStarted)
                StartCoroutine(FollowOffmeshLink());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("doorCol"))
        {
            IsKnockingDoor = true;
            Debug.Log("1");
        }
    }

    // OffmeshLink를 따라 이동하는 코루틴
    IEnumerator FollowOffmeshLink()
    {
        OffMeshLinkStatus = EOffmeshLinkStatus.InProgress;
        Agent.updatePosition = false; // NavMeshAgent의 위치 및 회전 제어 해제
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        Vector3 newPosition = transform.position;
        while (!Mathf.Approximately(Vector3.Distance(newPosition, Agent.currentOffMeshLinkData.endPos), 0f))
        {
            newPosition = Vector3.MoveTowards(transform.position, Agent.currentOffMeshLinkData.endPos, Agent.speed * Time.deltaTime);
            transform.position = newPosition;

            yield return new WaitForEndOfFrame();
        }

        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
        Agent.CompleteOffMeshLink(); // OffMeshLink 완료 처리

        Agent.updatePosition = true; // NavMeshAgent 제어 복원
        Agent.updateRotation = true;
        Agent.updateUpAxis = true;

        Debug.Log("OffMeshLink 완료");
    }

    // 주어진 범위 내에서 임의의 위치 선택
    public Vector3 PickLocationInRange(float range)
    {
        Vector3 searchLocation = transform.position;
        searchLocation += Random.Range(-range, range) * Vector3.forward;
        searchLocation += Random.Range(-range, range) * Vector3.right;

        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(searchLocation, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
            return hitResult.position;

        return transform.position;
    }

    // 현재 명령 취소
    protected virtual void CancelCurrentCommand()
    {
        // clear the current path
        Agent.ResetPath();

        DestinationSet = false;
        ReachedDestination = false;
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    }

    // 지정된 목적지로 이동
    public virtual void MoveTo(Vector3 destination)
    {
        CancelCurrentCommand();

        this.Agent.speed = walkSpeed;
        anim.SetBool("Move", true);

        SetDestination(destination);
    }

    public virtual void MoveToRun(Vector3 destination)
    {
        CancelCurrentCommand();

        this.Agent.isStopped = true;
        this.Agent.velocity = Vector3.zero;

        this.Agent.speed = runSpeed;

        //anim.SetBool("Suprise", true);
        anim.SetBool("Run", true);

        this.Agent.isStopped = false;

        SetDestination(destination);
    }

    #region 의자 앉기
    public void SitAtPosition(Vector3 interactionPos, Quaternion chairRotation)
    {
        StartCoroutine(SitProcess(interactionPos, chairRotation));
    }

    private IEnumerator SitProcess(Vector3 interactionPos, Quaternion chairRotation)
    {
        // 거인이 interactionPos로 이동
        MoveTo(interactionPos);
        yield return new WaitUntil(() => isDestination.HasReachedDestination());

        //Debug.Log(ReachedDestination);
        //Debug.Log(interactionPos);

        // 목표 회전에 도달할 때까지 회전을 진행
        while (Quaternion.Angle(transform.rotation, chairRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, chairRotation, Time.deltaTime * 5f);
            yield return null;
        }


        // 앉는 애니메이션 실행
        anim.SetBool("Sitting", true);

    }

    public void StandUpChase()
    {
        this.Agent.isStopped = true;
        this.Agent.velocity = Vector3.zero;

        SetAnimationState("Stand");

        anim.SetBool("Suprise", true);

        anim.SetBool("Run", true);
        anim.SetBool("Sitting", false);

        this.Agent.isStopped = false;
    }

    public void StandUpSearch()
    {
        this.Agent.isStopped = true;
        this.Agent.velocity = Vector3.zero;

        SetAnimationState("Stand");
        anim.SetBool("Sitting", false);

        this.Agent.isStopped = false;
    }
    #endregion

    #region 떨어진 물체 탐색

    public void SearchingObject()
    {
        if(DroppedObject.Instance.GetDroppedObjectsCount() > 0)
        {
            StartCoroutine(SearchingProcess());
        }

    }

    /// <summary>
    /// do-while 문을 사용하는 이유는 떨어진 객체의 배열(DroppedObjects)에 대한 처리를 최소 한 번은 보장하기 위해
    /// 즉, 함수가 호출될 때 배열에 최소한 하나의 요소가 있음을 가정하고, 
    /// 이 요소에 대한 처리를 시작합니다. 그 후에 배열에 추가 요소가 남아있다면 계속해서 처리함
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private IEnumerator SearchingProcess()
    {
        // 처리할 오브젝트의 복사본 리스트 생성
        var objectsToProcess = new List<GameObject>(DroppedObject.Instance.DroppedObjects);

        Debug.Log("SearchingProcess 시작됨.");

        foreach (var obj in objectsToProcess)
        {
            
            Debug.Log($"이동 시작: {obj.name} 위치로 이동 중.");

            // 물체의 위치로 이동
            MoveTo(obj.transform.position);

            if (isDestination.ischase == true)
                yield break;

            yield return new WaitUntil(() => isDestination.HasReachedDestination());

            Debug.Log($"이동 완료: {obj.name} 위치에 도착함.");

            // 물체와 상호작용하는 애니메이션 실행
            anim.SetBool("SearchObj", true);

            if (isDestination.ischase == true)
                yield break;

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Searching")); // 애니메이션 시작 대기

            Debug.Log($"애니메이션 시작: {obj.name}와 상호작용 중.");

            if (isDestination.ischase == true)
                yield break;

            // 애니메이션 완료까지 대기
            yield return new WaitUntil(() => IsSearching());

            Debug.Log($"애니메이션 완료: {obj.name}와의 상호작용 완료.");

            if (isDestination.ischase == true)
                yield break;

            // 애니메이션을 종료하고 다음 오브젝트로 넘어감
            anim.SetBool("SearchObj", false);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Gaint_Idle")); // 애니메이션 종료 대기
        }

        Debug.Log("모든 오브젝트에 대한 탐색 완료.");
        DroppedObject.Instance.DroppedObjects.Clear();
    }

    // 애니메이션 상태를 설정하는 보조 메소드
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


    public bool IsSearching()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Searching") && stateInfo.normalizedTime >= 1.0f;
    }
    #endregion

    #region 플레이어 공격

    public virtual void AttackToPlayer(GameObject player)
    {
        // AI 일시 정지
        //this.Agent.isStopped = true;
        this.Agent.velocity = Vector3.zero;

        // 플레이어 방향으로 회전
        Vector3 directionToTarget = player.transform.position - transform.position;
        directionToTarget.y = 0; // Y축 고정

        if (directionToTarget != Vector3.zero) // 회전 필요 시
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f); // 회전 속도
        }

        // 타격 트리거 on
        attackCol.GetComponent<SphereCollider>().enabled = true;

        // 공격 애니메이션 실행
        SetAnimationState("Giant_Attack");

        // 애니메이션 종료 후 AI 재개
        StartCoroutine(ResumeAgentAfterAttack());
    }

    // 공격 후 다시 NavMeshAgent 동작 재개
    private IEnumerator ResumeAgentAfterAttack()
    {
        // 공격 애니메이션 시간이 1.5초라고 가정
        yield return new WaitForSeconds(1.5f);

        // 타격 트리거 off
        attackCol.GetComponent<SphereCollider>().enabled = false;

        // AI 재개
        //this.Agent.isStopped = false;
    }

    public virtual void MissingPlayer(Vector3 destination)
    {
        CancelCurrentCommand();

        //attackCol.GetComponent<SphereCollider>().enabled = false;

        this.Agent.speed = runSpeed;

        anim.SetBool("Attack", false);

        SetDestination(destination);
    }

    public virtual void CatchPlayer()
    {
        CancelCurrentCommand();

        this.Agent.isStopped = true;
        this.Agent.velocity = Vector3.zero;

        SetAnimationState("Giant_Roaring");
    }

    #endregion

    #region 문 두드리기

    public void StartKnockingDoor(Vector3 intractionPos, Quaternion doorRotation)
    {
        StartCoroutine(KnockingProcess(intractionPos, doorRotation));
    }

    private IEnumerator KnockingProcess(Vector3 intractionPos, Quaternion doorRotation)
    {

        anim.SetBool("Run", false);

        MoveTo(intractionPos);

        while (Quaternion.Angle(transform.rotation, doorRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, doorRotation, Time.deltaTime * 5f);
            yield return null;
        }

        // 타격 트리거 on
        attackCol.GetComponent<SphereCollider>().enabled = true;

        anim.SetBool("Knocking", true);

    }
    #endregion
    public virtual void SetDestination(Vector3 destination)
    {
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
        {
            if (Agent.CalculatePath(hitResult.position, new NavMeshPath()))
            {
                Agent.SetDestination(hitResult.position);
                DestinationSet = true;
                ReachedDestination = false;
            }
            else
            {
                Debug.LogError("Failed to calculate path to destination.");
            }
        }
        else
        {
            Debug.LogError("SamplePosition failed, no valid NavMesh position found.");
        }
    }
}