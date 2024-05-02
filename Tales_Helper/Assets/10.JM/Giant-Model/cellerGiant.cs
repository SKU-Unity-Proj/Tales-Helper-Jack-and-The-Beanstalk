using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class cellerGiant : MonoBehaviour
{
    [SerializeField] private Transform interactPos;  // 목표 위치
    [SerializeField] private float traceRange = 10f;
    [SerializeField] private float attackRange = 2f;
    private Transform playerTransform;
    public LayerMask playerLayer;
    private Collider[] playerCheck;

    private NavMeshAgent agent;
    private Animator _animator;

    [Header("Unity Events")]
    public UnityEvent onPlayerEnterTrigger;

    // State flags
    private bool isTrace = false;
    private bool isAttack = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (agent == null || _animator == null)
        {
            Debug.LogError("Component missing: NavMeshAgent or Animator is not attached to this GameObject.");
        }
    }

    private void Update()
    {
        CheckForPlayer();

        if (playerTransform != null)
        {
            if (isAttack)
            {
                Attack();
            }
            else
            {
                Trace();
            }
        }
        else
        {
            Idle();
        }

        UpdateAnimationState();
    }

    private void CheckForPlayer()
    {
        playerCheck = Physics.OverlapSphere(transform.position, traceRange, playerLayer);
        bool playerFound = playerCheck.Length > 0 && playerCheck[0].gameObject.CompareTag("Player");

        _animator.SetBool("isTrace", playerFound && !isAttack);
        _animator.SetBool("isAttack", false);

        if (playerFound)
        {
            playerTransform = playerCheck[0].transform;
            agent.SetDestination(playerTransform.position);
            if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                PrepareAttack();
            }
        }
        else
        {
            playerTransform = null;
        }
    }

    private void UpdateAnimationState()
    {
        // Check if Sequence01 is finished
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Sequence01") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0))
        {
            SetAnimationState("Lookaround");
            // Set the agent's destination to the current position to stay in place
      
        }

        // Check if Lookaround is finished
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Lookaround") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0))
        {
            SetAnimationState("Walk");
            // Set the destination to the interact position when transitioning to Walk
            agent.SetDestination(interactPos.position);
            Debug.Log(agent.destination);
            agent.isStopped = false;
        }

        // Check if the agent has reached the interaction position during Walk state
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            if (Vector3.Distance(transform.position, interactPos.position) <= agent.stoppingDistance)
            {
                SetAnimationState("Scratch");
                agent.isStopped = true;  // Stop the agent once the destination is reached
            }
        }

        // Optionally, handle looping or ending the Scratch animation based on some condition
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Scratch") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0))
        {
            // Assuming Scratch is a looping animation, you might not need to do anything here.
            // If it's not looping, you can reset to another state or repeat the Scratch.
            // Ensure the agent remains stopped.

        }
    }



    private void SetAnimationState(string stateName, float transitionDuration = 0.1f, int StateLayer = 0)
    {
        if (_animator.HasState(StateLayer, Animator.StringToHash(stateName)))
        {
            _animator.CrossFadeInFixedTime(stateName, transitionDuration, StateLayer);

            if (StateLayer == 1)
                SetLayerPriority(1, 1);
        }
    }

    private void SetLayerPriority(int StateLayer = 1, int Priority = 1) // 애니메이터의 레이어 우선순위 값(무게) 설정
    {
        _animator.SetLayerWeight(StateLayer, Priority);
    }

    void PrepareAttack()
    {
        isAttack = true;
        _animator.SetBool("isAttack", true);
        _animator.SetBool("isTrace", false);
        agent.isStopped = true;
    }

    private void Trace()
    {
        if (!isAttack)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }
    }

    private void Attack()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Sequence01"))
        {
            agent.SetDestination(playerTransform.position); // Ensure the giant moves towards the player during the animation
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > attackRange)
        {
            isAttack = false;
            _animator.SetBool("isAttack", false);
            _animator.SetBool("isTrace", true);
        }
    }

    private void Idle()
    {
        _animator.SetBool("isTrace", false);
        _animator.SetBool("isAttack", false);
        agent.isStopped = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack and trace range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, traceRange);
    }
}
