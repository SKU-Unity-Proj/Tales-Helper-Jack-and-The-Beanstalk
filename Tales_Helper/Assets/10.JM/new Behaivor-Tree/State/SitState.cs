using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SitState : BehaviorNode
{
    private NavMeshAgent agent;
    private Transform sofaPosition; // 소파 위치
    private Animator animator;

    private ConditionNode conditionNode;
    private ChaseState chaseState;

    private bool isSitting = false; // 앉았는지 여부
    private bool isMovingToSofa = false; // 이동 중인지 여부
    private Coroutine sitCoroutine = null;

    public SitState(NavMeshAgent agent, Transform sofaPosition, Animator animator, ConditionNode conditionNode, ChaseState chaseState)
    {
        this.agent = agent;
        this.sofaPosition = sofaPosition;
        this.animator = animator;
        this.conditionNode = conditionNode;
        this.chaseState = chaseState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] 현재 상태: SitState");

        // 플레이어 감지 확인
        if (conditionNode.Execute() == NodeState.SUCCESS)
        {
            Debug.Log("[SitState] 플레이어 감지 → ChaseState로 이동");
            StopSitCoroutine();
            chaseState.Execute();
            return NodeState.SUCCESS;
        }

        // 이미 코루틴이 실행 중이라면 RUNNING 반환
        if (sitCoroutine != null)
        {
            return NodeState.RUNNING;
        }

        // 코루틴 시작
        sitCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(SitRoutine());
        return NodeState.RUNNING;
    }

    private IEnumerator SitRoutine()
    {
        Debug.Log("[SitState] 소파로 이동 시작");
        agent.isStopped = false;

        // 소파로 이동
        agent.SetDestination(sofaPosition.position);

        // 이동 완료 확인
        while (true)
        {
            if (conditionNode.Execute() == NodeState.SUCCESS)
            {
                Debug.Log("[SitState] 이동 중 플레이어 감지 → ChaseState로 이동");
                StopSitCoroutine();
                chaseState.Execute();
                yield break;
            }

            // 이동이 완료되었는지 확인
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    Debug.Log("[SitState] 소파 도착 확인");
                    break;
                }
            }

            yield return null; // 프레임을 넘어가며 지속 검사
        }

        // 소파 도착 후 앉기
        Debug.Log("[SitState] 소파 도착 → 앉기 애니메이션 실행");
        agent.isStopped = true;
        animator.SetBool("Sitting", true);

        isSitting = true;

        yield return new WaitForSeconds(2.0f); // 앉기 애니메이션 대기 시간

        // 앉은 후 대기
        Debug.Log("[SitState] 소파에 앉음 → 대기 상태 유지");
        while (isSitting)
        {
            if (conditionNode.Execute() == NodeState.SUCCESS)
            {
                Debug.Log("[SitState] 대기 중 플레이어 감지 → ChaseState로 이동");
                StopSitCoroutine();
                chaseState.Execute();
                yield break;
            }
            yield return NodeState.RUNNING;
        }
    }

    private void StopSitCoroutine()
    {
        if (sitCoroutine != null)
        {
            agent.gameObject.GetComponent<MonoBehaviour>().StopCoroutine(sitCoroutine);
            sitCoroutine = null;
        }

        isSitting = false;
        agent.isStopped = false;
        ResetAnimator();
    }

    private void ResetAnimator()
    {
        animator.SetBool("Move", false);
        animator.SetBool("Run", false);
        animator.SetBool("Sitting", false);
    }

    private bool HasReachedDestination()
    {
        // NavMeshAgent의 남은 거리와 도착 여부를 기반으로 도착했는지 확인
        if (!agent.pathPending) // 경로가 계산 중이 아닐 때
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // NavMeshAgent가 목적지에 거의 도착했고, 장애물에 막히지 않은 경우
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true; // 목적지에 도착
                }
            }
        }
        return false; // 아직 목적지에 도착하지 않음
    }
}
