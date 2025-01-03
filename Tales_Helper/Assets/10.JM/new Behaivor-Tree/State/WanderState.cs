using UnityEngine;
using UnityEngine.AI;

public class WanderState : BehaviorNode
{
    private NavMeshAgent agent;
    private float wanderRadius;
    private ConditionNode conditionNode;

    public WanderState(NavMeshAgent agent, float wanderRadius, ConditionNode conditionNode)
    {
        this.agent = agent;
        this.wanderRadius = wanderRadius;
        this.conditionNode = conditionNode;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] 현재 상태: WanderState");

        //  ConditionNode 검사 (한 번만 검사)
        NodeState conditionResult = conditionNode.Execute();
        if (conditionResult == NodeState.SUCCESS)
        {
            Debug.Log("[WanderState] 플레이어 감지 → SequenceNode로 이동");
            return NodeState.SUCCESS; // SequenceNode로 이동
        }

        //  순찰 행동 수행
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += agent.transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
            }
        }

        return NodeState.RUNNING;
    }
}
