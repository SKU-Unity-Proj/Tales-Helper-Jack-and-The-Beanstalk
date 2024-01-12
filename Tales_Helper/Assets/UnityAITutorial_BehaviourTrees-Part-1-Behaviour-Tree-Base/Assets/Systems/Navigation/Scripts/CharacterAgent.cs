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
    [SerializeField] float NearestPointSearchRange = 5f; // �ֱ��� Ž�� ����

    NavMeshAgent Agent; // NavMeshAgent ������Ʈ
    Animator anim; // �ִϸ����� ������Ʈ

    private float walkSpeed = 1.5f; // �ȱ� �ӵ�
    private float runSpeed = 3.5f; // �ٱ� �ӵ�

    private DroppedObject droppedObject; // ������ ��ü ����

    bool DestinationSet = false; // ������ ���� ����
    bool ReachedDestination = false; // ������ ���� ����

    EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted; // OffmeshLink ����

    public bool IsMoving => Agent.velocity.magnitude > float.Epsilon; // �̵� �� ����
    public bool AtDestination => ReachedDestination; // ������ ���� ���� ��ȯ


    // Start is called before the first frame update
    protected void Start()
    {
        Agent = GetComponent<NavMeshAgent>(); // NavMeshAgent ������Ʈ �ʱ�ȭ
        anim = GetComponent<Animator>(); // Animator ������Ʈ �ʱ�ȭ

        // ������ ��ü ������ ������
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

        this.Agent.speed = runSpeed;
        anim.SetBool("Run", true);

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
        yield return new WaitUntil(() => AtDestination);

        // ��ǥ ȸ���� ������ ������ ȸ���� ����
        while (Quaternion.Angle(transform.rotation, chairRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, chairRotation, Time.deltaTime * 5f);
            yield return null;
        }

        // �ɴ� �ִϸ��̼� ����
        anim.SetBool("Sitting", true);
    }

    public void StandUp()
    {
        // �Ͼ�� �ִϸ��̼� ����
        anim.SetBool("Sitting", false);
    }
    #endregion
  
    
    #region ������ ��ü Ž��

    public void SearchingObject()
    {
        if(droppedObject.DroppedObjects.Count > 0)
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
        for (int i = droppedObject.DroppedObjects.Count - 1; i >= 0; i--)
        {
            GameObject currentObj = droppedObject.DroppedObjects[i];

            // ��ü�� ��ġ�� �̵�
            MoveTo(currentObj.transform.position);
            yield return new WaitUntil(() => AtDestination);

            // ��ü�� ��ȣ�ۿ��ϴ� �ִϸ��̼� ����
            anim.SetBool("SearchObj", true);

            // �ִϸ��̼� �Ϸ���� ���
            yield return new WaitUntil(() => IsSearching());

            // ���� ������Ʈ ó�� �� ����Ʈ���� ����
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