using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EOffmeshLinkStatus
{
    NotStarted,
    InProgress
}

// ĳ����(����)�� �̵� �� �ൿ ������ ���� ��ũ��Ʈ
[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAgent : CharacterBase
{
    [SerializeField] public float NearestPointSearchRange = 5f; // �ֱ��� Ž�� ����
    [SerializeField] Transform attackCol;

    private NavMeshAgent Agent; // NavMeshAgent ������Ʈ
    private Animator anim; // �ִϸ����� ������Ʈ
    private LivingRoomGiant isDestination; // �ִϸ����� ������Ʈ

    private float walkSpeed = 2.5f; // �ȱ� �ӵ�
    private float runSpeed = 6.5f; // �ٱ� �ӵ�

    bool DestinationSet = false; // ������ ���� ����
    bool ReachedDestination = false; // ������ ���� ����

    EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted; // OffmeshLink ����

    public bool IsMoving => Agent.velocity.magnitude > float.Epsilon; // �̵� �� ����
    public bool AtDestination => ReachedDestination; // ������ ���� ���� ��ȯ

    public bool IsKnockingDoor { get; private set; } = false;

    // Start is called before the first frame update
    protected void Start()
    {
        Agent = GetComponent<NavMeshAgent>(); // NavMeshAgent ������Ʈ �ʱ�ȭ
        anim = GetComponent<Animator>(); // Animator ������Ʈ �ʱ�ȭ
        isDestination = GetComponent<LivingRoomGiant>();

        Agent.autoRepath = true; // ��ΰ� ��ȿ���� ���� �� �ڵ����� ��θ� ����
        Agent.stoppingDistance = 1.5f; // �÷��̾���� ������ �Ÿ��� ����

        // ���� Ʈ�������� �ʿ��� �� �̰� ���� ��.
        /* 
        // ������ Transform�� ������
        Transform giantTransform = this.transform;
        // DroppedObjectManager�� �ν��Ͻ��� ã�Ƽ� SetGiantTransform ȣ��
        DroppedObject.Instance.SetGiantTransform(giantTransform);
        */

    }

    // Update is called once per frame
    protected void Update()
    {
        // ������ �����Ǿ���, ��� ��� �Ϸ� �� OffMeshLink�� �ƴϰ�, �������� �����ߴ��� Ȯ��
        if (!Agent.pathPending && !Agent.isOnOffMeshLink && DestinationSet && (Agent.remainingDistance <= Agent.stoppingDistance))
        {
            DestinationSet = false;
            ReachedDestination = true;
        }

        // OffMeshLink ���� Ȯ�� �� ó��
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

    // OffmeshLink�� ���� �̵��ϴ� �ڷ�ƾ
    IEnumerator FollowOffmeshLink()
    {
        OffMeshLinkStatus = EOffmeshLinkStatus.InProgress;
        Agent.updatePosition = false; // NavMeshAgent�� ��ġ �� ȸ�� ���� ����
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
        Agent.CompleteOffMeshLink(); // OffMeshLink �Ϸ� ó��

        Agent.updatePosition = true; // NavMeshAgent ���� ����
        Agent.updateRotation = true;
        Agent.updateUpAxis = true;

        Debug.Log("OffMeshLink �Ϸ�");
    }

    // �־��� ���� ������ ������ ��ġ ����
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

    // ���� ��� ���
    protected virtual void CancelCurrentCommand()
    {
        // clear the current path
        Agent.ResetPath();

        DestinationSet = false;
        ReachedDestination = false;
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    }

    // ������ �������� �̵�
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

    #region ���� �ɱ�
    public void SitAtPosition(Vector3 interactionPos, Quaternion chairRotation)
    {
        StartCoroutine(SitProcess(interactionPos, chairRotation));
    }

    private IEnumerator SitProcess(Vector3 interactionPos, Quaternion chairRotation)
    {
        // ������ interactionPos�� �̵�
        MoveTo(interactionPos);
        yield return new WaitUntil(() => isDestination.HasReachedDestination());

        //Debug.Log(ReachedDestination);
        //Debug.Log(interactionPos);

        // ��ǥ ȸ���� ������ ������ ȸ���� ����
        while (Quaternion.Angle(transform.rotation, chairRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, chairRotation, Time.deltaTime * 5f);
            yield return null;
        }


        // �ɴ� �ִϸ��̼� ����
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

    #region ������ ��ü Ž��

    public void SearchingObject()
    {
        if(DroppedObject.Instance.GetDroppedObjectsCount() > 0)
        {
            StartCoroutine(SearchingProcess());
        }

    }

    /// <summary>
    /// do-while ���� ����ϴ� ������ ������ ��ü�� �迭(DroppedObjects)�� ���� ó���� �ּ� �� ���� �����ϱ� ����
    /// ��, �Լ��� ȣ��� �� �迭�� �ּ��� �ϳ��� ��Ұ� ������ �����ϰ�, 
    /// �� ��ҿ� ���� ó���� �����մϴ�. �� �Ŀ� �迭�� �߰� ��Ұ� �����ִٸ� ����ؼ� ó����
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private IEnumerator SearchingProcess()
    {
        // ó���� ������Ʈ�� ���纻 ����Ʈ ����
        var objectsToProcess = new List<GameObject>(DroppedObject.Instance.DroppedObjects);

        Debug.Log("SearchingProcess ���۵�.");

        foreach (var obj in objectsToProcess)
        {
            
            Debug.Log($"�̵� ����: {obj.name} ��ġ�� �̵� ��.");

            // ��ü�� ��ġ�� �̵�
            MoveTo(obj.transform.position);

            if (isDestination.ischase == true)
                yield break;

            yield return new WaitUntil(() => isDestination.HasReachedDestination());

            Debug.Log($"�̵� �Ϸ�: {obj.name} ��ġ�� ������.");

            // ��ü�� ��ȣ�ۿ��ϴ� �ִϸ��̼� ����
            anim.SetBool("SearchObj", true);

            if (isDestination.ischase == true)
                yield break;

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Searching")); // �ִϸ��̼� ���� ���

            Debug.Log($"�ִϸ��̼� ����: {obj.name}�� ��ȣ�ۿ� ��.");

            if (isDestination.ischase == true)
                yield break;

            // �ִϸ��̼� �Ϸ���� ���
            yield return new WaitUntil(() => IsSearching());

            Debug.Log($"�ִϸ��̼� �Ϸ�: {obj.name}���� ��ȣ�ۿ� �Ϸ�.");

            if (isDestination.ischase == true)
                yield break;

            // �ִϸ��̼��� �����ϰ� ���� ������Ʈ�� �Ѿ
            anim.SetBool("SearchObj", false);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Gaint_Idle")); // �ִϸ��̼� ���� ���
        }

        Debug.Log("��� ������Ʈ�� ���� Ž�� �Ϸ�.");
        DroppedObject.Instance.DroppedObjects.Clear();
    }

    // �ִϸ��̼� ���¸� �����ϴ� ���� �޼ҵ�
    private void SetAnimationState(string stateName, float transitionDuration = 0.1f, int StateLayer = 0)
    {
        if (anim.HasState(StateLayer, Animator.StringToHash(stateName)))
        {
            anim.CrossFadeInFixedTime(stateName, transitionDuration, StateLayer);

            if (StateLayer == 1)
                SetLayerPriority(1, 1);
        }

    }

    private void SetLayerPriority(int StateLayer = 1, int Priority = 1) // �ִϸ������� ���̾� �켱���� ��(����) ����
    {
        anim.SetLayerWeight(StateLayer, Priority);
    }


    public bool IsSearching()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Searching") && stateInfo.normalizedTime >= 1.0f;
    }
    #endregion

    #region �÷��̾� ����

    public virtual void AttackToPlayer(GameObject player)
    {
        // AI �Ͻ� ����
        //this.Agent.isStopped = true;
        this.Agent.velocity = Vector3.zero;

        // �÷��̾� �������� ȸ��
        Vector3 directionToTarget = player.transform.position - transform.position;
        directionToTarget.y = 0; // Y�� ����

        if (directionToTarget != Vector3.zero) // ȸ�� �ʿ� ��
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f); // ȸ�� �ӵ�
        }

        // Ÿ�� Ʈ���� on
        attackCol.GetComponent<SphereCollider>().enabled = true;

        // ���� �ִϸ��̼� ����
        SetAnimationState("Giant_Attack");

        // �ִϸ��̼� ���� �� AI �簳
        StartCoroutine(ResumeAgentAfterAttack());
    }

    // ���� �� �ٽ� NavMeshAgent ���� �簳
    private IEnumerator ResumeAgentAfterAttack()
    {
        // ���� �ִϸ��̼� �ð��� 1.5�ʶ�� ����
        yield return new WaitForSeconds(1.5f);

        // Ÿ�� Ʈ���� off
        attackCol.GetComponent<SphereCollider>().enabled = false;

        // AI �簳
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

    #region �� �ε帮��

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

        // Ÿ�� Ʈ���� on
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