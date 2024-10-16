using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class cellerGiant : MonoBehaviour
{
    [SerializeField] private Transform interactPos;  // ��ǥ ��ġ
    [SerializeField] private Transform stopPos;
    [SerializeField] private Transform attackCol;

    [SerializeField] private float traceRange = 3f;

    public float Speed // �Ӽ����� get/set ����
    {
        get { return traceRange; }
        set { traceRange = value; }
    }

    [SerializeField] private float attackRange = 3f;

    public Transform playerTransform;
    public LayerMask playerLayer;
    private Collider[] playerCheck;

    private NavMeshAgent agent;
    private Animator _animator;


    private bool isTrace = false;
    private bool isAttack = false;

    private float delay = 3.3f;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("StopCol"))
        {
            this.transform.position = stopPos.position;

            if (!agent.isStopped) // �̹� �����ִ��� Ȯ��
            {
                agent.isStopped = true;
                SetAnimationState("Stop", 0.1f); // �ִϸ��̼� ���� ����
            }
        }
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
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Sequence01") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0))
        {
            SetAnimationState("Lookaround");  
        }


        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Lookaround") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0))
        {
            SetAnimationState("Walk");

            agent.SetDestination(interactPos.position);
            Debug.Log(agent.destination);
        }


        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0)
                && Vector3.Distance(transform.position, interactPos.position) <= agent.stoppingDistance)
            {
                SetAnimationState("Scratch");
            }
        }
    }

    private void ForceTracePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            
            agent.SetDestination(playerTransform.position);
            isTrace = true;
            isAttack = false;

            SetAnimationState("Trace");

        }
    }

    // �ܺο��� ȣ���� �޼ҵ�, ������ ����
    public void DelayFunc()
    {
        Invoke("ForceTracePlayer", delay); // 'TriggerAnimation'�� 'delay' �ð� �Ŀ� ȣ��
    }

    private void Trace()
    {
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
            if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                PrepareAttack();
            }
        }

    }

    private void Attack()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) > attackRange)
        {

            isAttack = false;
            agent.SetDestination(playerTransform.position);
            SetAnimationState("Trace");
        }
    }

    void PrepareAttack()
    {
        isAttack = true;

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Trace") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0))
        {
            StartCoroutine(attackDelay());
            SetAnimationState("Attack");
        }

    }

    private IEnumerator attackDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("1");
        // Ÿ�� Ʈ���� on
        attackCol.GetComponent<SphereCollider>().enabled = true;
    }

    private void Idle()
    {
        _animator.SetBool("isTrace", false);
        _animator.SetBool("isAttack", false);
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

    private void SetLayerPriority(int StateLayer = 1, int Priority = 1) // �ִϸ������� ���̾� �켱���� ��(����) ����
    {
        _animator.SetLayerWeight(StateLayer, Priority);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, traceRange);
    }
}
