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

        //  �Ҹ� ����
        if (distanceToPlayer <= hearingRange)
        {
            Debug.Log("[ConditionNode] �Ҹ� ���� ����");
        }
        else
        {
            Debug.Log("[ConditionNode] �Ҹ� ���� ����");
            return NodeState.FAILURE; // Ž�� ����
        }

        //  �þ� ����
        Vector3 directionToPlayer = (player.position - agent.position).normalized;
        float angle = Vector3.Angle(agent.forward, directionToPlayer);

        if (distanceToPlayer <= detectRange && angle <= viewAngle / 2)
        {
            if (!Physics.Raycast(agent.position, directionToPlayer, distanceToPlayer, obstacleMask))
            {
                Debug.Log("[ConditionNode] �þ� ���� ���� �� ChaseState�� ��ȯ");
                return chaseState.Execute(); // ChaseState�� �̵�
            }
        }

        Debug.Log("[ConditionNode] �þ� ���� ����");
        return NodeState.FAILURE; // Ž�� ����
    }
}
