using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BehaviorNode
{
    private NavMeshAgent agent;
    private Transform player;
    private float detectRange;
    private SearchState searchState; // SearchState ���� �߰�

    public ChaseState(NavMeshAgent agent, Transform player, float detectRange, SearchState searchState)
    {
        this.agent = agent;
        this.player = player;
        this.detectRange = detectRange;
        this.searchState = searchState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] ���� ����: ChaseState");

        if (player == null)
        {
            Debug.Log("[ChaseState] �÷��̾ �������� ���� �� SearchState�� �̵�");
            searchState.Execute(); // SearchState�� ���� �̵�
            return NodeState.SUCCESS;
        }

        float distanceToPlayer = Vector3.Distance(agent.transform.position, player.position);

        if (distanceToPlayer > detectRange)
        {
            Debug.Log("[ChaseState] �÷��̾ ��ħ �� SearchState�� �̵�");
            searchState.Execute(); // SearchState�� ���� �̵�
            return NodeState.SUCCESS;
        }

        agent.SetDestination(player.position);
        return NodeState.RUNNING;
    }
}
