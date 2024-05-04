using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class cellerGiant : MonoBehaviour
{
    [SerializeField] private Transform interactPos;  // 목표 위치
    [SerializeField] private Transform stopPos;
    [SerializeField] private float traceRange = 3f;
    [SerializeField] private float attackRange = 3f;

    private Transform playerTransform;
    public LayerMask playerLayer;
    private Collider[] playerCheck;

    private NavMeshAgent agent;
    private Animator _animator;


    private bool isTrace = false;
    private bool isAttack = false;

    private float delay = 2.0f;

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

            agent.isStopped = true;
            SetAnimationState("Stop");
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
            traceRange = 20.0f;  // 추적 범위를 15.0f로 확장
            agent.SetDestination(playerTransform.position);
            isTrace = true;
            isAttack = false;

            SetAnimationState("Trace");

        }
    }

    // 외부에서 호출할 메소드, 지연을 포함
    public void DelayFunc()
    {
        Invoke("ForceTracePlayer", delay); // 'TriggerAnimation'을 'delay' 시간 후에 호출
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

        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Trace") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0))
            SetAnimationState("Attack");
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

    private void SetLayerPriority(int StateLayer = 1, int Priority = 1) // 애니메이터의 레이어 우선순위 값(무게) 설정
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
