using UnityEngine;

public class SearchState : BehaviorNode
{
    private float searchDuration = 5f;
    private float timer = 0f;
    private WanderState wanderState; // WanderState 참조

    // WanderState 설정 (지연 초기화)
    public void SetWanderState(WanderState wanderState)
    {
        this.wanderState = wanderState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] 현재 상태: SearchState");

        timer += Time.deltaTime;

        if (timer >= searchDuration)
        {
            Debug.Log("[SearchState] 탐색 완료 → WanderState로 전환");
            timer = 0f;
            wanderState.Execute(); // WanderState로 이동
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
