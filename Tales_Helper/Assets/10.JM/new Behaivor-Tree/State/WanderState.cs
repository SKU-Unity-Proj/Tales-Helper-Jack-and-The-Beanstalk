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

    private int wanderCount = 0; // 랜덤 위치 방문 횟수
    private const int MaxWanderCount = 5; // 최대 방문 횟수

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
        Debug.Log("[State] 현재 상태: WanderState");

        ResetAnimator();
        animator.SetBool("Move", true);

        agent.speed = wanderSpeed;

        NodeState conditionResult = conditionNode.Execute();
        if (conditionResult == NodeState.SUCCESS)
        {
            Debug.Log("[WanderState] 플레이어 감지 → ConditionNode로 이동");
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
                Debug.Log($"[WanderState] 랜덤 위치 방문 횟수: {wanderCount}/{MaxWanderCount}");
            }
        }

        // 방문 횟수가 최대에 도달하면 SitState로 이동
        if (wanderCount >= MaxWanderCount)
        {
            Debug.Log("[WanderState] 최대 방문 횟수 도달 → SitState로 이동");
            wanderCount = 0; // 방문 횟수 초기화
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
