using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SitState : BehaviorNode
{
    private NavMeshAgent agent;
    private Transform sofaPosition; // ���� ��ġ
    private Animator animator;

    private ConditionNode conditionNode;
    private ChaseState chaseState;
    private SearchState searchState;

    private Coroutine sitCoroutine = null;
    private bool isSitting = false;
    private bool isStandingUp = false;

    public SitState(NavMeshAgent agent, Transform sofaPosition, Animator animator, ConditionNode conditionNode, ChaseState chaseState, SearchState searchState)
    {
        this.agent = agent;
        this.sofaPosition = sofaPosition;
        this.animator = animator;
        this.conditionNode = conditionNode;
        this.chaseState = chaseState;
        this.searchState = searchState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] ���� ����: SitState");

        // ��� ������Ʈ ���� Ȯ��
        if (DroppedObject.Instance.CheckCondition())
        {
            Debug.Log("[SitState] ��� ������Ʈ ���� �� SearchState�� �̵�");
            if (sitCoroutine != null)
            {
                StopSitCoroutine();
            }

            searchState.Execute();
            return NodeState.SUCCESS;
        }

        // �÷��̾� ���� Ȯ��
        if (conditionNode.Execute() == NodeState.SUCCESS && !isStandingUp)
        {
            Debug.Log("[SitState] �÷��̾� ���� �� �Ͼ�� �ִϸ��̼� ����");
            StopSitCoroutine();
            sitCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(StandUpAndChaseRoutine());
            return NodeState.RUNNING;
        }

        // SitRoutine�� ������� �ʾ��� ��쿡�� ����
        if (sitCoroutine == null)
        {
            sitCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(SitRoutine());
        }

        return NodeState.RUNNING;
    }

    private IEnumerator SitRoutine()
    {
        Debug.Log("[SitState] ���ķ� �̵� ����");
        agent.isStopped = false;
        agent.SetDestination(sofaPosition.position);

        // ��� ��� ���
        while (agent.pathPending)
        {
            yield return null;
        }

        // ���ķ� �̵�
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            Debug.Log("[SitState] ���ķ� �̵� ��... ���� �Ÿ�: " + agent.remainingDistance);

            // ��� ������Ʈ ����
            if (DroppedObject.Instance.CheckCondition())
            {
                Debug.Log("[SitState] �̵� �� ��� ������Ʈ ���� �� SearchState�� �̵�");
                StopSitCoroutine();
                searchState.Execute();
                yield break;
            }

            if (conditionNode.Execute() == NodeState.SUCCESS)
            {
                Debug.Log("[SitState] �̵� �� �÷��̾� ���� �� ChaseState�� �̵�");
                StopSitCoroutine();
                yield return agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(StandUpAndChaseRoutine());
                yield break;
            }

            yield return null;
        }

        Debug.Log("[SitState] ���� ���� �� �ɱ� �ִϸ��̼� ����");

        agent.isStopped = true;

        while (Quaternion.Angle(agent.transform.rotation, sofaPosition.rotation) > 0.1f)
        {
            agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, sofaPosition.rotation, Time.deltaTime * 5f);
            yield return null;
        }

        animator.SetBool("Sitting", true);
        isSitting = true;

        // ���� ���� ����
        while (isSitting)
        {
            // ��� ������Ʈ ����
            if (DroppedObject.Instance.CheckCondition())
            {
                Debug.Log("[SitState] ���� ���¿��� ��� ������Ʈ ���� �� SearchState�� �̵�");
                StopSitCoroutine();
                searchState.Execute();
                yield break;
            }

            if (conditionNode.Execute() == NodeState.SUCCESS)
            {
                Debug.Log("[SitState] ���� ���¿��� �÷��̾� ���� �� ChaseState�� �̵�");
                StopSitCoroutine();
                yield return agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(StandUpAndChaseRoutine());
                yield break;
            }
            yield return null;
        }

        StopSitCoroutine();
    }

    private IEnumerator StandUpAndChaseRoutine()
    {
        Debug.Log("[SitState] �Ͼ�� �ִϸ��̼� ����");
        isStandingUp = true;

        StandUpChase();

        // �Ͼ�� �ִϸ��̼��� �Ϸ�� ������ ���
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Stand") &&
                                         animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        Debug.Log("[SitState] �Ͼ�� �ִϸ��̼� �Ϸ� �� ChaseState�� �̵�");

        isStandingUp = false;
        animator.SetBool("Stand", false);
        StopSitCoroutine();
        chaseState.Execute();
    }

    private void StopSitCoroutine()
    {
        if (sitCoroutine != null)
        {
            agent.gameObject.GetComponent<MonoBehaviour>().StopCoroutine(sitCoroutine);
            sitCoroutine = null;
        }

        isSitting = false;
        agent.isStopped = false;
        ResetAnimator();
    }

    private void StandUpChase()
    {
        Debug.Log("[SitState] StandUpChase() ����: �Ͼ�� �غ�");

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Stand", true);
        animator.SetBool("Sitting", false);

        agent.isStopped = false;
    }

    private void ResetAnimator()
    {
        animator.SetBool("Move", false);
        animator.SetBool("Run", false);
        animator.SetBool("Sitting", false);
        animator.SetBool("Stand", false);
    }
}
