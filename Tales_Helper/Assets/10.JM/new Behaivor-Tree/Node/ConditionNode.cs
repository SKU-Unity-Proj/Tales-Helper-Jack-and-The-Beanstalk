using UnityEngine;

public class ConditionNode : BehaviorNode
{
    private Transform agent;
    private Transform player;
    private float hearingRange;
    private float viewAngle;
    private float detectRange;
    private LayerMask obstacleMask;
    private ChaseState chaseState;

    public ConditionNode(Transform agent, Transform player, float hearingRange, float viewAngle, float detectRange, LayerMask obstacleMask, ChaseState chaseState)
    {
        this.agent = agent;
        this.player = player;
        this.hearingRange = hearingRange;
        this.viewAngle = viewAngle;
        this.detectRange = detectRange;
        this.obstacleMask = obstacleMask;
        this.chaseState = chaseState;
    }

    public override NodeState Execute()
    {
        float distanceToPlayer = Vector3.Distance(agent.position, player.position);

        //  소리 감지
        if (distanceToPlayer <= hearingRange)
        {
            Debug.Log("[ConditionNode] 소리 감지 성공");
        }
        else
        {
            Debug.Log("[ConditionNode] 소리 감지 실패");
            return NodeState.FAILURE; // 탐지 실패
        }

        //  시야 감지
        Vector3 directionToPlayer = (player.position - agent.position).normalized;
        float angle = Vector3.Angle(agent.forward, directionToPlayer);

        if (distanceToPlayer <= detectRange && angle <= viewAngle / 2)
        {
            if (!Physics.Raycast(agent.position, directionToPlayer, distanceToPlayer, obstacleMask))
            {
                Debug.Log("[ConditionNode] 시야 감지 성공 → ChaseState로 전환");
                return chaseState.Execute(); // ChaseState로 이동
            }
        }

        Debug.Log("[ConditionNode] 시야 감지 실패");
        return NodeState.FAILURE; // 탐지 실패
    }
}
