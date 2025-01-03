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
        Debug.Log("[State] ���� ����: WanderState");

        //  ConditionNode �˻� (�� ���� �˻�)
        NodeState conditionResult = conditionNode.Execute();
        if (conditionResult == NodeState.SUCCESS)
        {
            Debug.Log("[WanderState] �÷��̾� ���� �� SequenceNode�� �̵�");
            return NodeState.SUCCESS; // SequenceNode�� �̵�
        }

        //  ���� �ൿ ����
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
