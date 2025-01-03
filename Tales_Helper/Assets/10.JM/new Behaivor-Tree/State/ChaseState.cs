using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BehaviorNode
{
    private NavMeshAgent agent;
    private Transform player;
    private float detectRange;
    private SearchState searchState; // SearchState 참조 추가

    public ChaseState(NavMeshAgent agent, Transform player, float detectRange, SearchState searchState)
    {
        this.agent = agent;
        this.player = player;
        this.detectRange = detectRange;
        this.searchState = searchState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] 현재 상태: ChaseState");

        if (player == null)
        {
            Debug.Log("[ChaseState] 플레이어가 존재하지 않음 → SearchState로 이동");
            searchState.Execute(); // SearchState로 직접 이동
            return NodeState.SUCCESS;
        }

        float distanceToPlayer = Vector3.Distance(agent.transform.position, player.position);

        if (distanceToPlayer > detectRange)
        {
            Debug.Log("[ChaseState] 플레이어를 놓침 → SearchState로 이동");
            searchState.Execute(); // SearchState로 직접 이동
            return NodeState.SUCCESS;
        }

        agent.SetDestination(player.position);
        return NodeState.RUNNING;
    }
}
