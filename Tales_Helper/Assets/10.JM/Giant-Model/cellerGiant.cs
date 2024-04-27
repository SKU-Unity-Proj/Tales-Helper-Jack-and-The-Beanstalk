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
    private Animator _anmator;

    [Header("Unity Events")]
    public UnityEvent onPlayerEnterTrigger;

    // State flags
    private bool isTrace = false;
    private bool isAttack = false;

    private bool hasFixedDirection = false;  // To store if the direction has been fixed for attack
    private Quaternion fixedAttackRotation;  // To store the fixed rotation during attack

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _anmator = GetComponent<Animator>();

        if (agent == null || _anmator == null)
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
    }
    private void CheckForPlayer()
    {
        playerCheck = Physics.OverlapSphere(transform.position, traceRange, playerLayer);
        bool playerFound = playerCheck.Length > 0 && playerCheck[0].gameObject.CompareTag("Player");

        _anmator.SetBool("isTrace", playerFound && !isAttack);
        _anmator.SetBool("isAttack", false);

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
    void PrepareAttack()
    {
        isAttack = true;
        _anmator.SetBool("isAttack", true);
        _anmator.SetBool("isTrace", false);
        agent.isStopped = true;

        // Fix direction only once at the start of the attack
        if (!hasFixedDirection)
        {
            fixedAttackRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
            hasFixedDirection = true;
        }
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
        // Maintain fixed direction during the attack
        if (hasFixedDirection)
        {
            transform.rotation = fixedAttackRotation;
        }

        // Check if the player moves out of attack range
        if (Vector3.Distance(transform.position, playerTransform.position) > attackRange)
        {
            isAttack = false;
            _anmator.SetBool("isAttack", false);
            _anmator.SetBool("isTrace", true);
            hasFixedDirection = false; // Reset the direction fix
        }
    }

    private void Idle()
    {
        _anmator.SetBool("isTrace", false);
        _anmator.SetBool("isAttack", false);
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
