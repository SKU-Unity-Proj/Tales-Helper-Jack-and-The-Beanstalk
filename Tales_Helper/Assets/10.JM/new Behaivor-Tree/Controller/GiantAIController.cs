using UnityEngine;
using UnityEngine.AI;

public class GiantAIController : MonoBehaviour
{
    private BehaviorNode rootNode;

    public Transform player;
    public LayerMask obstacleMask;
    public NavMeshAgent agent;

    [Header("Detection Settings")]
    public float hearingRange = 10f; // 소리 감지 거리
    public float detectRange = 15f;  // 시야 감지 거리
    public float viewAngle = 90f;    // 시야각
    public float wanderRadius = 5f;  // 방황 범위

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

    void BuildBehaviorTree()
    {
        // SearchState 생성
        SearchState search = new SearchState();

        // ChaseState 생성
        ChaseState chase = new ChaseState(agent, player, detectRange, search);

        // ConditionNode 생성
        ConditionNode condition = new ConditionNode(transform, player, hearingRange, viewAngle, detectRange, obstacleMask, chase);

        // WanderState 생성
        WanderState wander = new WanderState(agent, wanderRadius, condition);

        search.SetWanderState(wander);

        // SequenceNode (탐지 및 행동)
        SequenceNode chaseSequence = new SequenceNode();
        chaseSequence.AddChild(condition); // 탐지 조건 검사
        chaseSequence.AddChild(chase);     // 추격
        chaseSequence.AddChild(search);    // 탐색

        //  Root SelectorNode
        SelectorNode rootSelector = new SelectorNode();
        rootSelector.AddChild(wander);     // WanderState (기본 상태)
        rootSelector.AddChild(chaseSequence); // 탐지 성공 시 행동

        rootNode = rootSelector;
    }

    #region 상태 디버그
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
            Debug.Log($"[GiantAIController] 현재 상태: {newState}");
            currentState = newState;
        }
    }
    #endregion

    #region 범위 시각화
    //Gizmos로 시각적 범위와 각도 표시
    private void OnDrawGizmos()
    {
        if (agent == null)
            return;

        // Wander 범위 (녹색 원)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        // Hearing 범위 (파란색 원)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        // Detect 범위 (빨간색 원)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // FOV 각도 (노란색 원호)
        Gizmos.color = Color.yellow;
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * detectRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * detectRange);


        // FOV 영역을 채우기 위한 라인 그리기
        float stepCount = 10; // 호를 그릴 선의 개수
        float stepAngle = viewAngle / stepCount;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -viewAngle / 2 + stepAngle * i;
            Vector3 direction = DirFromAngle(angle, false);
            Gizmos.DrawLine(transform.position, transform.position + direction * detectRange);
        }
    }

    // 각도에서 방향 벡터를 계산
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
