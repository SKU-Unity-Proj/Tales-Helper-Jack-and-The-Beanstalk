using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BehaviorNode
{
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    private AttackState attackState;
    private WanderState wanderState;

    private float detectRange;
    private float attackRadius;
    private float chaseSpeed = 3.5f;

    public ChaseState(NavMeshAgent agent, Transform player, float detectRange, float attackRadius, AttackState attackState, WanderState wanderState, Animator animator)
    {
        this.agent = agent;
        this.player = player;
        this.detectRange = detectRange;
        this.attackRadius = attackRadius;
        this.attackState = attackState;
        this.wanderState = wanderState;
        this.animator = animator;
    }


    public override NodeState Execute()
    {
        Debug.Log("[State] ���� ����: ChaseState");

        ResetAnimator();
        animator.SetBool("Move", true);
        animator.SetBool("Run", true);
        agent.isStopped = false; // �̵� �簳

        if (player == null || Vector3.Distance(agent.transform.position, player.position) > detectRange)
        {
            Debug.Log("[ChaseState] �÷��̾ �������� �ʰų� ��ħ �� WanderState�� �̵�");
            wanderState.Execute();

            GiantAIController controller = agent.GetComponent<GiantAIController>();
            controller?.DebugState(NodeState.SUCCESS, "ChaseState");

            return NodeState.SUCCESS;
        }

        if (Vector3.Distance(agent.transform.position, player.position) <= attackRadius)
        {
            Debug.Log("[ChaseState] ���� ���� �� �� AttackState�� ��ȯ");
            attackState.Execute();

            GiantAIController controller = agent.GetComponent<GiantAIController>();
            controller?.DebugState(NodeState.SUCCESS, "ChaseState");

            return NodeState.SUCCESS;
        }

        agent.SetDestination(player.position);

        GiantAIController chaseController = agent.GetComponent<GiantAIController>();
        chaseController?.DebugState(NodeState.RUNNING, "ChaseState");

        return NodeState.RUNNING;
    }

    private void ResetAnimator()
    {
        animator.SetBool("Move", false);
        animator.SetBool("Attack", false);
    }
}
