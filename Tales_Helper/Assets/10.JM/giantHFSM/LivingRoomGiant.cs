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
    [SerializeField] private Transform doorPos;

    public float sightRadius = 10f; // �þ� �Ÿ�
    public float attackRadius = 2f; // ���� ����
    public float chaseRadius = 15f; // ���� �Ÿ�

    private int wanderCount;
    private int maxWanderCount = 50;
    private int minWanderCount = 45;

    private Transform player; // �÷��̾��� ��ġ

    public bool ischase = false;
    public bool forceChase = false;

    private bool hasInteracted = false;
    private bool isStandingUp = false; // �Ͼ�� ������ ����
    private bool isWandering = false; // ���� ������ ����

    private float chaseTimer; // ���� �ð� Ÿ�̸�
    private float maxChaseDuration = 6f; // ���� ���� �� 6�� �� ���� ���·� ��ȯ

    private bool isPaused = false; // ���� ���� �÷���

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
        // ESC Ű�� ������ �� ���� ���¸� ��ȯ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame(); // ���� ���� ���
        }

        if (isPaused) return; // ���� ������ ���� ������Ʈ �ߴ�

        if (isPaused) return; // ���� ������ ���� ������Ʈ �ߴ�

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

    private void TogglePauseGame()
    {
        if (isPaused)
        {
            ResumeGame(); // ���� ����
        }
        else
        {
            PauseGame(); // ���� ����
        }
    }

    public void PauseGame()
    {
        if (isPaused) return; // �̹� ���� ���¸� �ƹ� �۾��� ���� ����

        isPaused = true; // ���� ���·� ����
        anim.speed = 0; // �ִϸ��̼� ����
        navAgent.isStopped = true; // NavMeshAgent ���߱�
        navAgent.velocity = Vector3.zero; // NavMeshAgent �ӵ� �ʱ�ȭ
        Debug.Log("Game Paused.");
    }

    public void ResumeGame()
    {
        if (!isPaused) return; // ���� ���°� �ƴϸ� �ƹ� �۾��� ���� ����

        isPaused = false; // ���� ����
        anim.speed = 1; // �ִϸ��̼� �簳
        navAgent.isStopped = false; // NavMeshAgent �簳
        Debug.Log("Game Resumed.");
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

        if (DroppedObject.Instance.CheckSpecialObjectCondition())
        {
            // ���� �ִϸ��̼� ����
            anim.SetBool("Run", true);

            player = BasicManager.Instance.PlayerTarget.transform;

            StartCoroutine(TransitionToChase()); // ���� ���·� ��ȯ

            // NavMeshAgent�� ��ΰ� ��ȿ���� �ʰų� �÷��̾ ������ �ʴ� ���, ��θ� ����
            if (navAgent.pathStatus == NavMeshPathStatus.PathInvalid || navAgent.remainingDistance > chaseRadius)
            {
                // ��ΰ� ��ȿ���� ������ ��θ� �ٽ� ����
                navAgent.ResetPath();
                giantAgent.MoveToRun(player.position);
            }

            Debug.Log($"Chasing player..."); // ���� ���� ���� �α� ���
        }
    }

    void ChaseState()
    {
        if (player == null) return; // �÷��̾ ������ ��ȯ

        ischase = true;

        if (DroppedObject.Instance.CheckSpecialObjectCondition())
        {
            // ���� �ִϸ��̼� ����
            anim.SetBool("Run", true);

            player = BasicManager.Instance.PlayerTarget.transform;

            // NavMeshAgent�� ���� �÷��̾� ����
            giantAgent.MoveToRun(player.position);

            // NavMeshAgent�� ��ΰ� ��ȿ���� �ʰų� �÷��̾ ������ �ʴ� ���, ��θ� ����
            if (navAgent.pathStatus == NavMeshPathStatus.PathInvalid || navAgent.remainingDistance > chaseRadius)
            {
                // ��ΰ� ��ȿ���� ������ ��θ� �ٽ� ����
                navAgent.ResetPath();
                giantAgent.MoveToRun(player.position);
            }

            Debug.Log($"Chasing player..."); // ���� ���� ���� �α� ���
        }

        if (giantAgent.IsKnockingDoor == true)
        {
            giantAgent.StartKnockingDoor(doorPos.position, doorPos.rotation);

        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // �÷��̾� ��ġ�� ���� ��� ���� �� ����
        if (distanceToPlayer <= attackRadius)
        {
            ischase = false;
            currentState = GiantState.Attack; // ���� ���·� ��ȯ
            //chaseTimer = 0f; // ���� ���·� ��ȯ�Ǹ� Ÿ�̸� ����
        }
        else if (distanceToPlayer <= chaseRadius)
        {
            // ���� �ִϸ��̼� ����
            anim.SetBool("Run", true);

            // NavMeshAgent�� ���� �÷��̾� ����
            giantAgent.MoveToRun(player.position);

            // NavMeshAgent�� ��ΰ� ��ȿ���� �ʰų� �÷��̾ ������ �ʴ� ���, ��θ� ����
            if (navAgent.pathStatus == NavMeshPathStatus.PathInvalid || navAgent.remainingDistance > chaseRadius)
            {
                // ��ΰ� ��ȿ���� ������ ��θ� �ٽ� ����
                navAgent.ResetPath();
                giantAgent.MoveToRun(player.position);
            }

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
                ischase = false;
                anim.SetBool("Run", false);
                currentState = GiantState.Wander;
                chaseTimer = 0f; // Ÿ�̸� ����
                Debug.Log("Player lost, returning to Wander state.");
            }
        }
    }

    void SearchState()
    {
        ischase = false;

        if (DroppedObject.Instance.CheckSpecialObjectCondition())
        {
            forceChase = true;
            player = BasicManager.Instance.PlayerTarget.transform;

            anim.SetBool("Run", true);
            anim.SetBool("SearchObj", false);

            StartCoroutine(TransitionToChase()); // ���� ���·� ��ȯ

            // NavMeshAgent�� ��ΰ� ��ȿ���� �ʰų� �÷��̾ ������ �ʴ� ���, ��θ� ����
            if (navAgent.pathStatus == NavMeshPathStatus.PathInvalid || navAgent.remainingDistance > chaseRadius)
            {
                // ��ΰ� ��ȿ���� ������ ��θ� �ٽ� ����
                navAgent.ResetPath();
                giantAgent.MoveToRun(player.position);
            }

            Debug.Log($"Chasing player..."); // ���� ���� ���� �α� ���
        }
        // �̹� ��ȣ�ۿ��� �Ϸ�Ǿ����� Ȯ��
        if (hasInteracted)
        {
            // �̹� ��ȣ�ۿ��� �Ϸ�Ǿ����Ƿ� �� �̻� ó������ ����
            currentState = GiantState.Wander; // ��ȣ�ۿ��� �Ϸ�Ǹ� ���� ���·� ��ȯ
            return;
        }

        // ������ �ɾ� �ִ� ���¶�� ���� �Ͼ�� Ž�� ���·� ��ȯ
        if (anim.GetBool("Sitting"))
        {
            giantAgent.StandUpSearch(); // �� �ִ� ���·� ����
            restCondition.ResetCondition(); // �޽� ���� �ʱ�ȭ
        }

        // ���� Ž�� ���� ������ ��ü�� �ִ��� Ȯ���ϰ�, ������ ���ο� ��ü�� Ž�� ����
        if (!giantAgent.IsSearching() && DroppedObject.Instance.GetDroppedObjectsCount() > 0)
        {
            Debug.Log("Starting to search for dropped objects.");
            giantAgent.SearchingObject(); // ������ ��ü�� ��ȣ�ۿ� ����
            return; // Ž���� ��� ���� ������ ��ȯ
        }

        // Ž���� �Ϸ�Ǿ����� Ȯ��
        if (giantAgent.IsSearching())
        {
            hasInteracted = true; // ��ȣ�ۿ� �Ϸ� �÷��� ����
            Debug.Log("Interaction with dropped object complete.");
            currentState = GiantState.Wander; // Ž���� �Ϸ�Ǹ� ���� ���·� ��ȯ
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRadius)
        {
            anim.SetBool("Run", true);
            anim.SetBool("SearchObj", false);
            currentState = GiantState.Chase; // ���� ���·� ��ȯ
            //chaseTimer = 0f; // ���� ���·� ��ȯ�Ǹ� Ÿ�̸� ����
        }

        // ���� Ž�� ���̶�� ���� ���� ����
        Debug.Log("Searching in progress...");
        return;
    }


    void AttackState()
    {
        if (player == null) return;

        // ���� ���� ���� ������ ����
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRadius)
        {
            //anim.SetTrigger("Attack"); // ���� �ִϸ��̼� Ʈ����
            giantAgent.AttackToPlayer(player.gameObject);
            Debug.Log("Attacking the player.");
        }
        else
        {
            currentState = GiantState.Chase; // ���� �� �÷��̾ ���� �ۿ� ������ �ٽ� ���� ���·� ��ȯ
            Debug.Log("Player moved out of attack range, switching to Chase.");
        }
    }

    void SittingState()
    {
        // 1. ������ ������ ���� �ִ� ���� �÷��̾ �߰��� ���
        if (player != null && !isStandingUp && !anim.GetBool("Sitting")) // ���� ���� �ʾ��� ��
        {
            if (DroppedObject.Instance.CheckSpecialObjectCondition())
            {
                player = BasicManager.Instance.PlayerTarget.transform;
                // ������ ����� ���� ���� �ٷ� ���� ���·� ��ȯ
                currentState = GiantState.Chase; // ��� ���� ���·� ��ȯ
            }
            // ������ ����� ���� ���� �ٷ� ���� ���·� ��ȯ
            currentState = GiantState.Chase; // ��� ���� ���·� ��ȯ
            Debug.Log("Player detected while moving to sit. Switching to Chase state.");
            return;
        }

        // 2. ������ �̹� �ɾ� �ִ� ���¿��� �÷��̾ �߰��� ���
        if (player != null && anim.GetBool("Sitting")) // �̹� ���� ���¿��� �÷��̾ �߰�
        {
            // �Ͼ�� ���� ���·� ��ȯ
            isStandingUp = true; // ������ �Ͼ�� ������ ǥ��
           
            giantAgent.StandUpChase(); // �Ͼ�� ���� ���� ����
            restCondition.ResetCondition(); // �޽� ���� �ʱ�ȭ

            anim.SetBool("Sitting", false); // �ɴ� �ִϸ��̼� �ߴ�
            anim.SetBool("Run", true); // �ɴ� �ִϸ��̼� �ߴ�

            StartCoroutine(TransitionToChase()); // ���� ���·� ��ȯ
                                                 // 1. Ư�� ������ �ߵ��Ǿ����� Ȯ��
            if (DroppedObject.Instance.CheckSpecialObjectCondition())
            {
                player = BasicManager.Instance.PlayerTarget.transform;
                // �Ͼ�� ���� ���·� ��ȯ
                isStandingUp = true; // ������ �Ͼ�� ������ ǥ��

                giantAgent.StandUpChase(); // �Ͼ�� ���� ���� ����
                restCondition.ResetCondition(); // �޽� ���� �ʱ�ȭ

                anim.SetBool("Sitting", false); // �ɴ� �ִϸ��̼� �ߴ�
                anim.SetBool("Run", true); // �ɴ� �ִϸ��̼� �ߴ�

                StartCoroutine(TransitionToChase()); // ���� ���·� ��ȯ
                return;
            }

            Debug.Log("Player detected while sitting. Standing up and switching to Chase state.");
            return;
        }

        // 3. ������(����)�� �������� �ʾҴٸ� ��� �̵�
        if (!HasReachedDestination())
        {
            anim.SetBool("Run", false); // ���� ���� �ٴ� �ִϸ��̼� ����
            giantAgent.SitAtPosition(chairTransform.position, chairTransform.rotation); // ���ڱ��� �̵�
        }
        else
        {
            // ������ ��쿡�� �ɱ� �ִϸ��̼� ����
            anim.SetBool("Sitting", true);
        }

        bool conditionMet = DroppedObject.Instance.CheckCondition();
        if (conditionMet == true)
        {
            currentState = GiantState.Search; // ��� ���� ���·� ��ȯ
        }

    }



    // �÷��̾� ���� ����
    void CheckTargetVisibility()
    {
        foreach (var target in DetectableTargetManager.Instance.AllTargets)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            // ���� ���� ���� ���� �ִ��� Ȯ��
            if (distanceToTarget <= attackRadius)
            {
                player = target.transform; // Ÿ���� �÷��̾�� ����
                currentState = GiantState.Attack; // ���� ���·� ��ȯ
                Debug.Log("Player detected in attack range. Switching to Attack state.");
                return; // ���� ���·� ��ȯ�Ǿ����Ƿ� ���� ����
            }

            // ���� ������ ���� ������ ���� ���� ������ ����
            else if (distanceToTarget <= chaseRadius)
            {
                player = target.transform; // Ÿ���� �÷��̾�� ����
                if (currentState == GiantState.Sitting && !isStandingUp)
                {
                    // �Ͼ�� ���� ���·� ��ȯ
                    isStandingUp = true;
                    //giantAgent.StandUpChase();
                    restCondition.ResetCondition();
                    StartCoroutine(TransitionToChase());
                }
                else
                {
                    currentState = GiantState.Chase; // ���� ���·� ��ȯ
                    //chaseTimer = 0f; // ���� ���·� ��ȯ�� �� Ÿ�̸� �ʱ�ȭ
                    Debug.Log("Player detected in chase range. Switching to Chase state.");
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



