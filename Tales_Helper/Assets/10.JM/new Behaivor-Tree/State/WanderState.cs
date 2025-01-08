using UnityEngine;
using UnityEngine.AI;

public class WanderState : BehaviorNode
{
    private Animator animator;
    private NavMeshAgent agent;

    private ConditionNode conditionNode;
    private SitState sitState;

    private float wanderRadius;
    private float originalSpeed;
    private float wanderSpeed = 1.5f;

    private int wanderCount = 0; // ���� ��ġ �湮 Ƚ��
    private const int MaxWanderCount = 5; // �ִ� �湮 Ƚ��

    public WanderState(NavMeshAgent agent, float wanderRadius, ConditionNode conditionNode, SitState sitState, Animator animator)
    {
        this.agent = agent;
        this.wanderRadius = wanderRadius;
        this.conditionNode = conditionNode;
        this.sitState = sitState;
        this.animator = animator;

        this.originalSpeed = agent.speed;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] ���� ����: WanderState");

        ResetAnimator();
        animator.SetBool("Move", true);

        agent.speed = wanderSpeed;

        NodeState conditionResult = conditionNode.Execute();
        if (conditionResult == NodeState.SUCCESS)
        {
            Debug.Log("[WanderState] �÷��̾� ���� �� ConditionNode�� �̵�");
            agent.speed = originalSpeed;
            return NodeState.SUCCESS;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius + agent.transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
                wanderCount++;
                Debug.Log($"[WanderState] ���� ��ġ �湮 Ƚ��: {wanderCount}/{MaxWanderCount}");
            }
        }

        // �湮 Ƚ���� �ִ뿡 �����ϸ� SitState�� �̵�
        if (wanderCount >= MaxWanderCount)
        {
            Debug.Log("[WanderState] �ִ� �湮 Ƚ�� ���� �� SitState�� �̵�");
            wanderCount = 0; // �湮 Ƚ�� �ʱ�ȭ
            sitState.Execute();
            return NodeState.SUCCESS;
        }

        GiantAIController controller = agent.GetComponent<GiantAIController>();
        controller?.DebugState(NodeState.RUNNING, "WanderState");

        return NodeState.RUNNING;
    }
    private void ResetAnimator()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Attack", false);
    }
}
