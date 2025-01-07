using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SitState : BehaviorNode
{
    private NavMeshAgent agent;
    private Transform sofaPosition; // ���� ��ġ
    private Animator animator;

    private ConditionNode conditionNode;
    private ChaseState chaseState;

    private bool isSitting = false; // �ɾҴ��� ����
    private bool isMovingToSofa = false; // �̵� ������ ����
    private Coroutine sitCoroutine = null;

    public SitState(NavMeshAgent agent, Transform sofaPosition, Animator animator, ConditionNode conditionNode, ChaseState chaseState)
    {
        this.agent = agent;
        this.sofaPosition = sofaPosition;
        this.animator = animator;
        this.conditionNode = conditionNode;
        this.chaseState = chaseState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] ���� ����: SitState");

        // �÷��̾� ���� Ȯ��
        if (conditionNode.Execute() == NodeState.SUCCESS)
        {
            Debug.Log("[SitState] �÷��̾� ���� �� ChaseState�� �̵�");
            StopSitCoroutine();
            chaseState.Execute();
            return NodeState.SUCCESS;
        }

        // �̹� �ڷ�ƾ�� ���� ���̶�� RUNNING ��ȯ
        if (sitCoroutine != null)
        {
            return NodeState.RUNNING;
        }

        // �ڷ�ƾ ����
        sitCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(SitRoutine());
        return NodeState.RUNNING;
    }

    private IEnumerator SitRoutine()
    {
        Debug.Log("[SitState] ���ķ� �̵� ����");
        agent.isStopped = false;

        // ���ķ� �̵�
        agent.SetDestination(sofaPosition.position);

        // �̵� �Ϸ� Ȯ��
        while (true)
        {
            if (conditionNode.Execute() == NodeState.SUCCESS)
            {
                Debug.Log("[SitState] �̵� �� �÷��̾� ���� �� ChaseState�� �̵�");
                StopSitCoroutine();
                chaseState.Execute();
                yield break;
            }

            // �̵��� �Ϸ�Ǿ����� Ȯ��
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    Debug.Log("[SitState] ���� ���� Ȯ��");
                    break;
                }
            }

            yield return null; // �������� �Ѿ�� ���� �˻�
        }

        // ���� ���� �� �ɱ�
        Debug.Log("[SitState] ���� ���� �� �ɱ� �ִϸ��̼� ����");
        agent.isStopped = true;
        animator.SetBool("Sitting", true);

        isSitting = true;

        yield return new WaitForSeconds(2.0f); // �ɱ� �ִϸ��̼� ��� �ð�

        // ���� �� ���
        Debug.Log("[SitState] ���Ŀ� ���� �� ��� ���� ����");
        while (isSitting)
        {
            if (conditionNode.Execute() == NodeState.SUCCESS)
            {
                Debug.Log("[SitState] ��� �� �÷��̾� ���� �� ChaseState�� �̵�");
                StopSitCoroutine();
                chaseState.Execute();
                yield break;
            }
            yield return NodeState.RUNNING;
        }
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

    private void ResetAnimator()
    {
        animator.SetBool("Move", false);
        animator.SetBool("Run", false);
        animator.SetBool("Sitting", false);
    }

    private bool HasReachedDestination()
    {
        // NavMeshAgent�� ���� �Ÿ��� ���� ���θ� ������� �����ߴ��� Ȯ��
        if (!agent.pathPending) // ��ΰ� ��� ���� �ƴ� ��
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // NavMeshAgent�� �������� ���� �����߰�, ��ֹ��� ������ ���� ���
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true; // �������� ����
                }
            }
        }
        return false; // ���� �������� �������� ����
    }
}
