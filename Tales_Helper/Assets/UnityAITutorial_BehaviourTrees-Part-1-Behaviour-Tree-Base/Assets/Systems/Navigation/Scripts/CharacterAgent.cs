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
    [SerializeField] float NearestPointSearchRange = 5f; // 최근접 탐색 범위

    NavMeshAgent Agent; // NavMeshAgent 컴포넌트
    Animator anim; // 애니메이터 컴포넌트

    private float walkSpeed = 1.5f; // 걷기 속도
    private float runSpeed = 3.5f; // 뛰기 속도

    private DroppedObject droppedObject; // 떨어진 물체 정보

    bool DestinationSet = false; // 목적지 설정 여부
    bool ReachedDestination = false; // 목적지 도달 여부

    EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted; // OffmeshLink 상태

    public bool IsMoving => Agent.velocity.magnitude > float.Epsilon; // 이동 중 여부
    public bool AtDestination => ReachedDestination; // 목적지 도달 여부 반환


    // Start is called before the first frame update
    protected void Start()
    {
        Agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 초기화
        anim = GetComponent<Animator>(); // Animator 컴포넌트 초기화

        // 떨어진 물체 정보를 가져옴
        droppedObject = GameObject.FindObjectOfType<DroppedObject>();
        if (droppedObject == null)
        {
            Debug.LogError("DroppedObject component not found in the scene.");
            return;
        }
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

        this.Agent.speed = runSpeed;
        anim.SetBool("Run", true);

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
        yield return new WaitUntil(() => AtDestination);

        // 목표 회전에 도달할 때까지 회전을 진행
        while (Quaternion.Angle(transform.rotation, chairRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, chairRotation, Time.deltaTime * 5f);
            yield return null;
        }

        // 앉는 애니메이션 실행
        anim.SetBool("Sitting", true);
    }

    public void StandUp()
    {
        // 일어나는 애니메이션 실행
        anim.SetBool("Sitting", false);
    }
    #endregion
  
    
    #region 떨어진 물체 탐색

    public void SearchingObject()
    {
        if(droppedObject.DroppedObjects.Count > 0)
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
        for (int i = droppedObject.DroppedObjects.Count - 1; i >= 0; i--)
        {
            GameObject currentObj = droppedObject.DroppedObjects[i];

            // 물체의 위치로 이동
            MoveTo(currentObj.transform.position);
            yield return new WaitUntil(() => AtDestination);

            // 물체와 상호작용하는 애니메이션 실행
            anim.SetBool("SearchObj", true);

            // 애니메이션 완료까지 대기
            yield return new WaitUntil(() => IsSearching());

            // 현재 오브젝트 처리 후 리스트에서 제거
            droppedObject.DroppedObjects.RemoveAt(i);
            anim.SetBool("SearchObj", false);
        }
        
    }


    public bool IsSearching()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Searching") && stateInfo.normalizedTime >= 1.0f;
    }
    #endregion

    public virtual void SetDestination(Vector3 destination)
    {
        // find nearest spot on navmesh and move there
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
        {
            Agent.SetDestination(hitResult.position);
            DestinationSet = true;
            ReachedDestination = false;
        }
    }
}