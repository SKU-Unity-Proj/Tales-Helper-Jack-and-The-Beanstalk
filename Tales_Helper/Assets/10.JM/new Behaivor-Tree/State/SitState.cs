using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SitState : BehaviorNode
{
    private NavMeshAgent agent;
    private Transform sofaPosition; // 소파 위치
    private Animator animator;

    private ConditionNode conditionNode;
    private ChaseState chaseState;
    private SearchState searchState;

    private Coroutine sitCoroutine = null;
    private bool isSitting = false;
    private bool isStandingUp = false;

    public SitState(NavMeshAgent agent, Transform sofaPosition, Animator animator, ConditionNode conditionNode, ChaseState chaseState, SearchState searchState)
    {
        this.agent = agent;
        this.sofaPosition = sofaPosition;
        this.animator = animator;
        this.conditionNode = conditionNode;
        this.chaseState = chaseState;
        this.searchState = searchState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] 현재 상태: SitState");

        // 드롭 오브젝트 감지 확인
        if (DroppedObject.Instance.CheckCondition())
        {
            Debug.Log("[SitState] 드롭 오브젝트 감지 → SearchState로 이동");
            if (sitCoroutine != null)
            {
                StopSitCoroutine();
            }

            searchState.Execute();
            return NodeState.SUCCESS;
        }

        // 플레이어 감지 확인
        if (conditionNode.Execute() == NodeState.SUCCESS && !isStandingUp)
        {
            Debug.Log("[SitState] 플레이어 감지 → 일어나는 애니메이션 실행");
            StopSitCoroutine();
            sitCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(StandUpAndChaseRoutine());
            return NodeState.RUNNING;
        }

        // SitRoutine이 실행되지 않았을 경우에만 시작
        if (sitCoroutine == null)
        {
            sitCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(SitRoutine());
        }

        return NodeState.RUNNING;
    }

    private IEnumerator SitRoutine()
    {
        Debug.Log("[SitState] 소파로 이동 시작");
        agent.isStopped = false;
        agent.SetDestination(sofaPosition.position);

        // 경로 계산 대기
        while (agent.pathPending)
        {
            yield return null;
        }

        // 소파로 이동
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            Debug.Log("[SitState] 소파로 이동 중... 남은 거리: " + agent.remainingDistance);

            // 드롭 오브젝트 감지
            if (DroppedObject.Instance.CheckCondition())
            {
                Debug.Log("[SitState] 이동 중 드롭 오브젝트 감지 → SearchState로 이동");
                StopSitCoroutine();
                searchState.Execute();
                yield break;
            }

            if (conditionNode.Execute() == NodeState.SUCCESS)
            {
                Debug.Log("[SitState] 이동 중 플레이어 감지 → ChaseState로 이동");
                StopSitCoroutine();
                yield return agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(StandUpAndChaseRoutine());
                yield break;
            }

            yield return null;
        }

        Debug.Log("[SitState] 소파 도착 → 앉기 애니메이션 실행");

        agent.isStopped = true;

        while (Quaternion.Angle(agent.transform.rotation, sofaPosition.rotation) > 0.1f)
        {
            agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, sofaPosition.rotation, Time.deltaTime * 5f);
            yield return null;
        }

        animator.SetBool("Sitting", true);
        isSitting = true;

        // 앉은 상태 유지
        while (isSitting)
        {
            // 드롭 오브젝트 감지
            if (DroppedObject.Instance.CheckCondition())
            {
                Debug.Log("[SitState] 앉은 상태에서 드롭 오브젝트 감지 → SearchState로 이동");
                StopSitCoroutine();
                searchState.Execute();
                yield break;
            }

            if (conditionNode.Execute() == NodeState.SUCCESS)
            {
                Debug.Log("[SitState] 앉은 상태에서 플레이어 감지 → ChaseState로 이동");
                StopSitCoroutine();
                yield return agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(StandUpAndChaseRoutine());
                yield break;
            }
            yield return null;
        }

        StopSitCoroutine();
    }

    private IEnumerator StandUpAndChaseRoutine()
    {
        Debug.Log("[SitState] 일어나는 애니메이션 실행");
        isStandingUp = true;

        StandUpChase();

        // 일어나는 애니메이션이 완료될 때까지 대기
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Stand") &&
                                         animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        Debug.Log("[SitState] 일어나는 애니메이션 완료 → ChaseState로 이동");

        isStandingUp = false;
        animator.SetBool("Stand", false);
        StopSitCoroutine();
        chaseState.Execute();
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

    private void StandUpChase()
    {
        Debug.Log("[SitState] StandUpChase() 실행: 일어나는 준비");

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Stand", true);
        animator.SetBool("Sitting", false);

        agent.isStopped = false;
    }

    private void ResetAnimator()
    {
        animator.SetBool("Move", false);
        animator.SetBool("Run", false);
        animator.SetBool("Sitting", false);
        animator.SetBool("Stand", false);
    }
}
