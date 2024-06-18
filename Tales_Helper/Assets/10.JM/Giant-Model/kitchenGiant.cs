using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class kitchenGiant : MonoBehaviour
{
    public enum State { Cleaning, Chasing, Attacking, Searching }
    public State currentState;

    public int index; // �� ������ �ε���

    public kitchenGiantManager manager; // �޴��� ����
    private NavMeshAgent agent;
    private Animator _animator;
    public Transform player;

    public float sightRadius = 10f;
    public float soundRadius = 5f;
    public float sightAngle = 120f; // �þ߰�
    public float loseSightRadius = 15f; // �÷��̾ �Ҿ������ û�� ��ġ�� ���ư��� ����
    public float rotationSpeed = 90.0f; // �ʴ� 90�� ȸ��

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

        Vector3 controlPoint1 = startPos + (Vector3.up * 0.5f); // ���̸� �ٿ� �� ���ƽ�ϰ� ����
        Vector3 controlPoint2 = endPos + (Vector3.up * 0.5f);

        
        _animator.SetBool("islanded", true);  // ���� �ִϸ��̼� ����

        //agent.updatePosition = false; // ������Ʈ�� ��ġ ������Ʈ�� ��ũ��Ʈ���� ����

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

        _animator.SetBool("islanded", false);  // ���� �ִϸ��̼� ����
        yield return new WaitForSeconds(0.1f); // ���� �ִϸ��̼��� �����ϱ� ���� ��� �ð�

        agent.speed = 0f;  // ������ �� �ӵ��� �ӽ÷� 0���� ����
        //agent.updatePosition = true; // ������Ʈ�� ��ġ ������Ʈ�� �ٽ� Ȱ��ȭ
        
        agent.CompleteOffMeshLink();
        agent.speed = 3.5f; // �ӵ��� ������� ����


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

                    // û�� ��ġ�� �����ϸ�, �޴����� ���� ������ �����ͼ� ����
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

        // �÷��̾���� �Ÿ��� loseSightRadius���� ũ�� û�� ���·� ���ư��ϴ�.
        if (Vector3.Distance(transform.position, player.position) > loseSightRadius)
        {
            Debug.Log($"{gameObject.name}�� �÷��̾ �Ҿ������ û�� ��ġ�� ���ư��ϴ�.");
            currentState = State.Cleaning;
            agent.SetDestination(cleaningPosition.position);
        }
    }

    void Attack()
    {
        // ���� ���� ����
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
                        Debug.Log($"{gameObject.name}�� �þ� �ݰ� ������ �÷��̾ �����߽��ϴ�.");
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
                Debug.Log($"{gameObject.name}�� �Ҹ� �ݰ� ������ �÷��̾ �����߽��ϴ�.");
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
        Debug.Log($"{gameObject.name}�� ���ο� û�� ��ġ�� �̵��մϴ�: {cleaningPosition.name}");
    }

    public Transform GetCleaningPosition()
    {
        return cleaningPosition;
    }

    private IEnumerator WaitAndClean()
    {
        isWaiting = true;
        Debug.Log($"{gameObject.name}�� û�� ���Դϴ�: {cleaningPosition.name}");
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
        Debug.Log($"{gameObject.name}�� û�� ��ġ�� �����մϴ�: {cleaningPosition.name}");
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
                        Debug.Log($"{gameObject.name}�� û�� ���� �þ� �ݰ� ������ �÷��̾ �����߽��ϴ�.");
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
                Debug.Log($"{gameObject.name}�� û�� ���� �Ҹ� �ݰ� ������ �÷��̾ �����߽��ϴ�.");
                currentState = State.Chasing;
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        // �Ҹ� ���� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, soundRadius);

        // �þ߰� ǥ��
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

    private void SetLayerPriority(int StateLayer = 1, int Priority = 1) // �ִϸ������� ���̾� �켱���� ��(����) ����
    {
        _animator.SetLayerWeight(StateLayer, Priority);
    }
}
