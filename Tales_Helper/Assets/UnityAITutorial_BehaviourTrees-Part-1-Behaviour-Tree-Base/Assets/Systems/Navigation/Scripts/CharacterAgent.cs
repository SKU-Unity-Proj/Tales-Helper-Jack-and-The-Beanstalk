using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EOffmeshLinkStatus
{
    NotStarted,
    InProgress
}

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAgent : CharacterBase
{
    [SerializeField] float NearestPointSearchRange = 5f;

    NavMeshAgent Agent;
    Animator anim;

    private float walkSpeed = 1.5f;
    private float runSpeed = 3.5f;

    bool DestinationSet = false;
    bool ReachedDestination = false;
    EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;


    public bool IsMoving => Agent.velocity.magnitude > float.Epsilon;

    public bool AtDestination => ReachedDestination;

    // Start is called before the first frame update
    protected void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected void Update()
    {
        // have a path and near the end point?
        if (!Agent.pathPending && !Agent.isOnOffMeshLink && DestinationSet && (Agent.remainingDistance <= Agent.stoppingDistance))
        {
            DestinationSet = false;
            ReachedDestination = true;
        }

        // are we on an offmesh link?
        if (Agent.isOnOffMeshLink)
        {
            // have we started moving along the link
            if (OffMeshLinkStatus == EOffmeshLinkStatus.NotStarted)
                StartCoroutine(FollowOffmeshLink());
        }
    }


    IEnumerator FollowOffmeshLink()
    {
        // start the offmesh link - disable NavMesh agent control
        OffMeshLinkStatus = EOffmeshLinkStatus.InProgress;
        Agent.updatePosition = false;
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        // move along the path
        Vector3 newPosition = transform.position;
        while (!Mathf.Approximately(Vector3.Distance(newPosition, Agent.currentOffMeshLinkData.endPos), 0f))
        {
            newPosition = Vector3.MoveTowards(transform.position, Agent.currentOffMeshLinkData.endPos, Agent.speed * Time.deltaTime);
            transform.position = newPosition;

            yield return new WaitForEndOfFrame();
        }

        // flag the link as completed
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
        Agent.CompleteOffMeshLink();

        // return control the agent
        Agent.updatePosition = true;
        Agent.updateRotation = true;
        Agent.updateUpAxis = true;
    }

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

    protected virtual void CancelCurrentCommand()
    {
        // clear the current path
        Agent.ResetPath();

        DestinationSet = false;
        ReachedDestination = false;
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    }

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

    public void SearchingObject(Vector3 location)
    {
        StartCoroutine(SearchingProcess(location));
    }

    private IEnumerator SearchingProcess(Vector3 location)
    {
        // 물체의 위치로 이동
        MoveTo(location); // MoveTo는 CharacterAgent의 이동을 처리하는 가상 메소드로 가정
        yield return new WaitUntil(() => AtDestination);

        // 물체와 상호작용하는 애니메이션 실행
        anim.SetBool("SearchObj", true);
    }
    
    public void FindNotingObject()
    {
        anim.SetBool("SearchObj", false);
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