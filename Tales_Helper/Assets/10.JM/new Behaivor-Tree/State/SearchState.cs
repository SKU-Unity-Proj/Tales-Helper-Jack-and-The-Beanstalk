using UnityEngine;

public class SearchState : BehaviorNode
{
    private float searchDuration = 5f;
    private float timer = 0f;
    private WanderState wanderState; // WanderState ����

    // WanderState ���� (���� �ʱ�ȭ)
    public void SetWanderState(WanderState wanderState)
    {
        this.wanderState = wanderState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] ���� ����: SearchState");

        timer += Time.deltaTime;

        if (timer >= searchDuration)
        {
            Debug.Log("[SearchState] Ž�� �Ϸ� �� WanderState�� ��ȯ");
            timer = 0f;
            wanderState.Execute(); // WanderState�� �̵�
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
