using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class cellerGiant : MonoBehaviour
{
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

    private float idleTime = 5.0f; // Time after which we transition to Sequence01
    private float timer = 0.0f; // Timer to track idle time

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
        if (!_animator.GetBool("isTrace") && !_animator.GetBool("isAttack"))
        {
            timer += Time.deltaTime;

            if (timer >= idleTime)
            {
                _animator.SetTrigger("Sequence01");
                agent.isStopped = false;
                agent.SetDestination(this.transform.position); // Set the destination to the current position initially
                timer = 0.0f; // Reset timer
            }
        }
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
