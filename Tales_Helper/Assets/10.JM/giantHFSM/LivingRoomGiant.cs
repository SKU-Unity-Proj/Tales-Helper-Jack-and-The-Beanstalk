using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class LivingRoomGiant : MonoBehaviour
{
    private CharacterAgent giantAgent; // CharacterAgent ��ũ��Ʈ ����
    private NavMeshAgent navAgent;
    private Animator anim;
    private RestCondition restCondition;

    public enum GiantState { Idle, Wander, Chase, Search, Attack, Sitting }
    public GiantState currentState;

    [Header("Wander and Rest Settings")]
    [SerializeField] private float Wander_Range = 10f;
    [SerializeField] private Transform chairTransform; // ���� Transform

    public float sightRadius = 10f; // �þ� �Ÿ�
    public float attackRadius = 2f; // ���� ����
    public float chaseRadius = 15f; // ���� �Ÿ�
    private int wanderCount;
    private int maxWanderCount = 50;
    private int minWanderCount = 45;
    private Vector3 sittingPosition; // ������ ���� ��ġ
    private Transform player; // �÷��̾��� ��ġ

    private bool hasInteracted = false;
    private bool isStandingUp = false; // �Ͼ�� ������ ����
    private bool isWandering = false; // ���� ������ ����

    private float chaseTimer; // ���� �ð� Ÿ�̸�
    private float maxChaseDuration = 6f; // ���� ���� �� 6�� �� ���� ���·� ��ȯ

    void Start()
    {
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        giantAgent = GetComponent<CharacterAgent>();
        restCondition = GetComponent<RestCondition>();

        currentState = GiantState.Idle;
        wanderCount = Random.Range(minWanderCount, maxWanderCount); // ���� Ƚ�� ����
    }

    void Update()
    {
        switch (currentState)
        {
            case GiantState.Idle:
                IdleState();
                break;
            case GiantState.Wander:
                WanderState();
                break;
            case GiantState.Chase:
                ChaseState();
                break;
            case GiantState.Search:
                SearchState();
                break;
            case GiantState.Attack:
                AttackState();
                break;
            case GiantState.Sitting:
                SittingState();
                break;
        }

        CheckTargetVisibility(); // �÷��̾� ���� ���� Ȯ��
    }

    // ���º� �޼ҵ�

    void IdleState()
    {
        if (wanderCount > 0)
        {
            currentState = GiantState.Wander;
        }
    }

    void WanderState()
    {
        // �������� �������� ��� ���ο� ��ġ�� �̵�
        if (HasReachedDestination())
        {
            // ���ο� ���� ��ġ�� �̵�
            Vector3 location = giantAgent.PickLocationInRange(Wander_Range);
            anim.SetBool("Run", false); // ���� ���� �� �޸��� �ִϸ��̼�
            giantAgent.MoveTo(location);
            player = null; // ���� �� �÷��̾ ������ ���
        }
        else
        {
            // ���� �������� �������� ���� ���� �̵� �� ���� ����
            Debug.Log("Moving to destination...");
        }

        // �޽� ���� Ȯ��
        restCondition.UpdateTimer(Time.deltaTime);
        if (restCondition.CheckCondition())
        {
            currentState = GiantState.Sitting; // �޽� ������ �����Ǹ� �ɴ� ���·� ��ȯ
            Debug.Log("Rest condition met. Switching to Sitting state.");
        }
    }

    void ChaseState()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ���� ���� ������ �÷��̾� ����
        if (distanceToPlayer <= attackRadius)
        {
            currentState = GiantState.Attack; // ���� ���·� ��ȯ
        }
        else if (distanceToPlayer <= chaseRadius)
        {
            anim.SetBool("Run", true); // ���� �ִϸ��̼� ����
            giantAgent.MoveToRun(player.position); // �÷��̾ �߰�
            Debug.Log($"Chasing player..."); // ���� ���� ���� �α� ���
        }
        else
        {
            // ���� ������ ��� ���, ���� ���� �ð��� ������Ŵ
            chaseTimer += Time.deltaTime;
            Debug.Log($"Player out of range. Chase Timer: {chaseTimer} / {maxChaseDuration}"); // Ÿ�̸� �α� ���

            // ���� ���� �ð��� �ʰ��Ǹ� ������ ��ȯ
            if (chaseTimer >= maxChaseDuration)
            {
                currentState = GiantState.Wander;
                anim.SetBool("Run", false); // ���� �ִϸ��̼� ����
                player = null; // ���� ���� �� �÷��̾ null�� ����
                chaseTimer = 0f; // Ÿ�̸� ����
                Debug.Log("Player lost, returning to Wander state.");
            }
        }
    }


    void SearchState()
    {
        // ������ ��ü�� Ž���ϴ� ����
        giantAgent.SearchingObject();

        if (giantAgent.AtDestination)
        {
            currentState = GiantState.Idle; // Ž�� �� �ٽ� Idle ���·� ���ư�
        }
    }

    void AttackState()
    {
        if (player == null) return;

        // ���� ���� ���� ������ ����
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRadius)
        {
            anim.SetTrigger("Attack"); // ���� �ִϸ��̼� Ʈ����
            giantAgent.AttackToPlayer(player.gameObject);
        }
        else
        {
            currentState = GiantState.Chase; // ���� �� �÷��̾ ���� �ۿ� ������ �ٽ� ���� ���·� ��ȯ
        }
    }

    void SittingState()
    {
        //giantAgent.SitAtPosition(chairTransform.position, chairTransform.rotation); // �ɴ� ���·� ��ȯ

        if (!isStandingUp && anim.GetBool("Sitting") && player != null)
        {
            isStandingUp = true; // ������ �Ͼ�� ������ ǥ��
            giantAgent.StandUpChase(); // ������ ������ ����� ���� ����
            restCondition.ResetCondition(); // �޽� ���� �ʱ�ȭ
            StartCoroutine(TransitionToChase());
            return;
        }

        if (!HasReachedDestination())
        {
            anim.SetBool("Run", false); // ���� ���� �ٴ� �ִϸ��̼� ����
            player = null; // �ɴ� ���¿��� �÷��̾ null�� �����Ͽ� ������ �ߴ�
            giantAgent.SitAtPosition(chairTransform.position, chairTransform.rotation); // �ɴ� ���·� ��ȯ
        }
    }

    // �÷��̾� ���� ����
    void CheckTargetVisibility()
    {
        foreach (var target in DetectableTargetManager.Instance.AllTargets)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            if (distanceToTarget <= attackRadius)
            {
                player = target.transform;
                if (currentState == GiantState.Sitting && !isStandingUp)
                {
                    isStandingUp = true;
                    giantAgent.StandUpChase();
                    restCondition.ResetCondition();
                    StartCoroutine(TransitionToChase());
                }
                else
                {
                    currentState = GiantState.Chase;
                    chaseTimer = 0f; // ���� ���·� ��ȯ�� �� Ÿ�̸� �ʱ�ȭ
                }
                return;
            }
        }
    }

    // �ڿ������� ���� ���·� ��ȯ�ϴ� �ڷ�ƾ
    private IEnumerator TransitionToChase()
    {
        yield return new WaitUntil(() => anim.GetBool("Sitting") == false); // ������ �Ͼ ������ ���
        isStandingUp = false; // �Ͼ�� ���� �Ϸ�
        currentState = GiantState.Chase; // ���� ���·� ��ȯ
    }

    public bool HasReachedDestination()
    {
        // NavMeshAgent�� ���� �Ÿ��� ���� ���θ� ������� �����ߴ��� Ȯ��
        if (!navAgent.pathPending) // ��ΰ� ��� ���� �ƴ� ��
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                // NavMeshAgent�� �������� ���� �����߰�, ��ֹ��� ������ ���� ���
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                {
                    return true; // �������� ����
                }
            }
        }
        return false; // ���� �������� �������� ����
    }
}



#if UNITY_EDITOR
[CanEditMultipleObjects]  // ���� ������Ʈ ���� ����
[CustomEditor(typeof(LivingRoomGiant))]
public class LivingRoomGiantEditor : Editor
{
    public void OnSceneGUI()
    {
        // target�� LivingRoomGiant Ÿ������ ĳ����
        var giant = (LivingRoomGiant)target;

        if (giant == null) return; // �� üũ

        // �þ� ���� (Vision Cone)
        Handles.color = new Color(1f, 0f, 0f, 0.25f); // ������, ���� 0.25
        Vector3 startPoint = Mathf.Cos(-giant.sightRadius * Mathf.Deg2Rad) * giant.transform.forward +
                             Mathf.Sin(-giant.sightRadius * Mathf.Deg2Rad) * giant.transform.right;
        Handles.DrawSolidArc(giant.transform.position, Vector3.up, startPoint, giant.sightRadius * 2f, giant.sightRadius);

        // ���� ����
        Handles.color = new Color(0f, 1f, 0f, 0.25f); // �ʷϻ�, ���� 0.25
        Handles.DrawSolidDisc(giant.transform.position, Vector3.up, giant.attackRadius);

        // ���� ����
        Handles.color = new Color(0f, 0f, 1f, 0.25f); // �Ķ���, ���� 0.25
        Handles.DrawSolidDisc(giant.transform.position, Vector3.up, giant.chaseRadius);
    }
}
#endif // UNITY_EDITOR



