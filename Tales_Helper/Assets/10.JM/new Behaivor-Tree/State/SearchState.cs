using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : BehaviorNode
{
    private NavMeshAgent agent;
    private Animator animator;
    private DroppedObject droppedObjectManager;
    private WanderState wanderState;

    private GameObject targetObject; // 탐색할 타겟 오브젝트
    private Coroutine searchCoroutine = null;

    private bool isSearching = false;
    private bool searchCompleted = false; // 탐색 완료 상태를 저장

    public SearchState(NavMeshAgent agent, Animator animator, DroppedObject droppedObjectManager, WanderState wanderState)
    {
        this.agent = agent;
        this.animator = animator;
        this.droppedObjectManager = droppedObjectManager;
        this.wanderState = wanderState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] 현재 상태: SearchState");

        // 탐색 완료 여부 확인
        if (searchCompleted)
        {
            Debug.Log("[SearchState] 탐색 완료 → WanderState로 전환");
            searchCompleted = false; // 상태 초기화

            ResetAnimator();
            wanderState.Execute();
            return NodeState.SUCCESS; // 성공 반환
        }

        // 탐색 중인지 확인
        if (!isSearching)
        {
            targetObject = FindClosestDroppedObject();
            if (targetObject == null)
            {
                Debug.Log("[SearchState] 탐색할 오브젝트가 없음 → WanderState로 이동");
                ResetAnimator();
                wanderState.Execute();
                return NodeState.SUCCESS;
            }

            Debug.Log($"[SearchState] 탐색할 오브젝트 발견: {targetObject.name}");
            isSearching = true;
            searchCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(SearchRoutine());
        }

        return NodeState.RUNNING;
    }

    private IEnumerator SearchRoutine()
    {
        Debug.Log("[SearchState] 탐색 시작 → 타겟 오브젝트로 이동");

        // 탐색 위치로 이동
        agent.isStopped = false;
        agent.SetDestination(targetObject.transform.position);

        // 이동 중
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        Debug.Log("[SearchState] 탐색 위치 도착 → 탐색 애니메이션 실행");
        agent.isStopped = true;

        // 탐색 애니메이션
        animator.SetBool("SearchObj", true);

        // 탐색 애니메이션이 완료될 때까지 대기
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Searching") &&
                                         animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        animator.SetBool("SearchObj", false);

        // 탐색 완료 후 오브젝트 제거
        droppedObjectManager.RemoveDroppedObject(targetObject);
        targetObject = null;

        Debug.Log("[SearchState] 탐색 완료 → WanderState로 이동");

        isSearching = false;
        agent.isStopped = false;

        searchCompleted = true; // 탐색 완료 플래그 설정

        if (searchCoroutine != null)
        {
            agent.gameObject.GetComponent<MonoBehaviour>().StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }
    }

    private GameObject FindClosestDroppedObject()
    {
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in droppedObjectManager.DroppedObjects)
        {
            float distance = Vector3.Distance(agent.transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    private void ResetAnimator()
    {
        animator.SetBool("Move", false);
        animator.SetBool("Run", false);
        animator.SetBool("Sitting", false);
        animator.SetBool("Stand", false);
    }
}
