using UnityEngine;
using UnityEngine.AI;

public class GiantAIController : MonoBehaviour
{
    private BehaviorNode rootNode;

    [Header("AI Components")]
    public NavMeshAgent agent; // �׺���̼� ������Ʈ
    public Animator animator; // �ִϸ�����

    [Header("Player Detection")]
    public Transform player; // �÷��̾� Transform
    public LayerMask obstacleMask; // ��ֹ� ����ũ

    [Header("Detection Settings")]
    public float hearingRange = 10f; // �Ҹ� ���� �Ÿ�
    public float detectRange = 15f; // �þ� ���� �Ÿ�
    public float viewAngle = 90f; // �þ߰�
    public float wanderRadius = 5f; // ��Ȳ ����
    public float attackRadius = 2f; // ���� ����

    [Header("AI Acting Settings")]
    public Transform sofaPos;

    private string currentState = "";

    void Start()
    {
        BuildBehaviorTree();
    }

    void Update()
    {
        NodeState result = rootNode.Execute();
        DebugState(result);
    }

    /// <summary>
    /// �ൿ Ʈ�� ����
    /// </summary>
    void BuildBehaviorTree()
    {
        // WanderState ���� (ConditionNode�� ���߿� ����)
        WanderState wander = new WanderState(agent, wanderRadius, null, null, animator);

        // AttackState ���� (chase�� ���߿� ����)
        AttackState attack = new AttackState(agent, player, attackRadius, detectRange, animator, null, wander);

        // ChaseState ���� (attackState ����)
        ChaseState chase = new ChaseState(agent, player, detectRange, attackRadius, attack, wander, animator);

        // AttackState�� ChaseState ���� �߰�
        attack = new AttackState(agent, player, attackRadius, detectRange, animator, chase, wander);

        // ConditionNode ����
        ConditionNode condition = new ConditionNode(transform, player, hearingRange, viewAngle, detectRange, obstacleMask, chase);

        SitState sit = new SitState(agent, sofaPos, animator, condition, chase);

        // WanderState�� ConditionNode�� SitState ���� �߰�
        wander = new WanderState(agent, wanderRadius, condition, sit, animator);


        // SequenceNode (Wander �� Sit)
        SequenceNode wanderSequence = new SequenceNode();
        wanderSequence.AddChild(wander); // WanderState
        wanderSequence.AddChild(sit);    // SitState

        // SequenceNode (Ž�� �� �ൿ)
        SequenceNode chaseSequence = new SequenceNode();
        chaseSequence.AddChild(condition); // Ž�� ���� �˻�
        chaseSequence.AddChild(chase); // �߰�
        chaseSequence.AddChild(attack); // ����

        // Root SelectorNode
        SelectorNode rootSelector = new SelectorNode();
        rootSelector.AddChild(wanderSequence); // WanderState (�⺻ ����)
        rootSelector.AddChild(chaseSequence); // Ž�� ���� �� �ൿ

        rootNode = rootSelector;
    }


    #region ���� �����
    void DebugState(NodeState state)
    {
        string newState = "";

        switch (state)
        {
            case NodeState.SUCCESS:
                newState = "SUCCESS";
                break;
            case NodeState.FAILURE:
                newState = "FAILURE";
                break;
            case NodeState.RUNNING:
                newState = "RUNNING";
                break;
        }

        if (currentState != newState)
        {
            Debug.Log($"[GiantAIController] ���� ����: {newState}");
            currentState = newState;
        }
    }
    #endregion

    #region ���� �ð�ȭ
    // Gizmos�� �ð��� ������ ���� ǥ��
    private void OnDrawGizmos()
    {
        if (agent == null)
            return;

        // Wander ���� (��� ��)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        // Hearing ���� (�Ķ��� ��)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        // Detect ���� (������ ��)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // FOV ���� (����� ��)
        Gizmos.color = Color.yellow;
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * detectRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * detectRange);

        // FOV ������ ������ ä���
        float stepCount = 10; // ȣ�� �׸� ���� ����
        float stepAngle = viewAngle / stepCount;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -viewAngle / 2 + stepAngle * i;
            Vector3 direction = DirFromAngle(angle, false);
            Gizmos.DrawLine(transform.position, transform.position + direction * detectRange);
        }
    }

    // �������� ���� ���͸� ���
    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion
}
