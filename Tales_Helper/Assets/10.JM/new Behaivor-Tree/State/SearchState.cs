using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : BehaviorNode
{
    private NavMeshAgent agent;
    private Animator animator;
    private DroppedObject droppedObjectManager;
    private WanderState wanderState;

    private GameObject targetObject; // Ž���� Ÿ�� ������Ʈ
    private Coroutine searchCoroutine = null;

    private bool isSearching = false;
    private bool searchCompleted = false; // Ž�� �Ϸ� ���¸� ����

    public SearchState(NavMeshAgent agent, Animator animator, DroppedObject droppedObjectManager, WanderState wanderState)
    {
        this.agent = agent;
        this.animator = animator;
        this.droppedObjectManager = droppedObjectManager;
        this.wanderState = wanderState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] ���� ����: SearchState");

        // Ž�� �Ϸ� ���� Ȯ��
        if (searchCompleted)
        {
            Debug.Log("[SearchState] Ž�� �Ϸ� �� WanderState�� ��ȯ");
            searchCompleted = false; // ���� �ʱ�ȭ

            ResetAnimator();
            wanderState.Execute();
            return NodeState.SUCCESS; // ���� ��ȯ
        }

        // Ž�� ������ Ȯ��
        if (!isSearching)
        {
            targetObject = FindClosestDroppedObject();
            if (targetObject == null)
            {
                Debug.Log("[SearchState] Ž���� ������Ʈ�� ���� �� WanderState�� �̵�");
                ResetAnimator();
                wanderState.Execute();
                return NodeState.SUCCESS;
            }

            Debug.Log($"[SearchState] Ž���� ������Ʈ �߰�: {targetObject.name}");
            isSearching = true;
            searchCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(SearchRoutine());
        }

        return NodeState.RUNNING;
    }

    private IEnumerator SearchRoutine()
    {
        Debug.Log("[SearchState] Ž�� ���� �� Ÿ�� ������Ʈ�� �̵�");

        // Ž�� ��ġ�� �̵�
        agent.isStopped = false;
        agent.SetDestination(targetObject.transform.position);

        // �̵� ��
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        Debug.Log("[SearchState] Ž�� ��ġ ���� �� Ž�� �ִϸ��̼� ����");
        agent.isStopped = true;

        // Ž�� �ִϸ��̼�
        animator.SetBool("SearchObj", true);

        // Ž�� �ִϸ��̼��� �Ϸ�� ������ ���
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Searching") &&
                                         animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        animator.SetBool("SearchObj", false);

        // Ž�� �Ϸ� �� ������Ʈ ����
        droppedObjectManager.RemoveDroppedObject(targetObject);
        targetObject = null;

        Debug.Log("[SearchState] Ž�� �Ϸ� �� WanderState�� �̵�");

        isSearching = false;
        agent.isStopped = false;

        searchCompleted = true; // Ž�� �Ϸ� �÷��� ����

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
