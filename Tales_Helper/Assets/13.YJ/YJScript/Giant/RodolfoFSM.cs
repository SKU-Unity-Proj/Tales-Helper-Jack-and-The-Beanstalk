using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RodolfoFSM : MonoBehaviour
{
    public float viewAngle;
    public float viewDistance;

    public Transform targetTr;
    private NavMeshAgent agent;
    private Animator anim;
    public GameObject mouse;
    private bool isPathResetting = false;

    public string IdleAnimName;

    public float checkDistance;
    public LayerMask groundLayer;

    private bool Grounded = true;
    private float GroundedOffset = -0.14f;
    private float GroundedRadius = 0.28f;

    public bool isMouseCatch = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        ChangeState(State.IDLE);
        StartCoroutine(CheckMonsterState());
    }

    #region �þ߰�
    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    IEnumerator View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.CompareTag("Player"))
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.CompareTag("Player"))
                        {
                            Debug.Log("�÷��̾ �þ� ���� �ֽ��ϴ�.");

                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);

                            ChangeState(State.IDLE);
                        }
                        else if (_targetTf == null)
                        {
                            Debug.Log("cantfind");
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(View());
    }
    #endregion

    #region FSM

    #region STATE
    public enum State
    {
        FALL,
        IDLE,
        MOVE,
        TRACE
    }

    public State state;

    IEnumerator CheckMonsterState()
    {
        yield return new WaitForSeconds(0.5f);

        switch (state)
        {
            case State.FALL:
                FALL();
                break;
            case State.IDLE:
                IDLE();
                break;
            case State.MOVE:
                MOVE();
                break;
            case State.TRACE:
                TRACE();
                break;
        }
        StartCoroutine(CheckMonsterState());
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (state)
        {
            case State.FALL:
                FALLTrigger(other);
                break;
            case State.IDLE:
                IDLETrigger(other);
                break;
            case State.MOVE:
                MOVETrigger(other);
                break;
            case State.TRACE:
                TRACETrigger(other);
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (state)
        {
            case State.IDLE:
                IDLECollision(other);
                break;
        }
    }

    public void ChangeState(State state)
    {
        switch (this.state)
        {
            case State.FALL:
                FALLExit();
                break;
            case State.IDLE:
                IDLEExit();
                break;
            case State.MOVE:
                MOVEExit();
                break;
            case State.TRACE:
                TRACEExit();
                break;
        }

        this.state = state;

        switch (state)
        {
            case State.FALL:
                FALLEnter();
                break;
            case State.IDLE:
                IDLEEnter();
                break;
            case State.MOVE:
                MOVEEnter();
                break;
            case State.TRACE:
                TRACEEnter();
                break;
        }
    }
    #endregion

    #region IDLE
    private void IDLEEnter()
    {
        // ��� �ൿ ����
        SetAnimationState(IdleAnimName);
    }
    private void IDLE()
    {
        // �ڸ� ���ƺ��� �߰� ���·� ��ȯ
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Turning Right"))
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                ChangeState(State.TRACE);

        // �ڸ� ���ƺ��� �߰� ���·� ��ȯ
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Looking Around"))
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                ChangeState(State.TRACE);
    }
    private void IDLETrigger(Collider other)
    {
        if (other.CompareTag("Player") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Looking Around"))
        {
            SetAnimationState("Looking Around");
        }
    }
    private void IDLECollision(Collision other)
    {
        Debug.Log("b");
        if (other.collider.CompareTag("Player"))
        {
            Debug.Log("a");
            SetAnimationState("Turning Right");
        }
    }
    private void IDLEExit()
    {

    }
    #endregion


    #region FALL
    private void FALLEnter()
    {
        agent.isStopped = true;
        SetAnimationState("Fall");
    }
    private void FALL()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);

        // �������� ���� �� üũ �� �ִϸ��̼� ��ȯ
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
        {
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, groundLayer, QueryTriggerInteraction.Ignore);
            if (Grounded)
            {
                anim.CrossFadeInFixedTime("HardLanding", 0f);
            }
        }

        // ���� �� ���� ��ȯ
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("HardLanding"))
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                if (isMouseCatch)
                    ChangeState(State.MOVE);
                else
                    ChangeState(State.TRACE);
            }
    }
    private void FALLTrigger(Collider other)
    {

    }
    private void FALLExit()
    {

    }
    #endregion


    #region MOVE
    private void MOVEEnter()
    {
        transform.LookAt(mouse.transform.position);
        SetAnimationState("Scream");
    }
    private void MOVE()
    {
        MoveCheck();

        // ������ �̵�
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
            agent.SetDestination(mouse.transform.position);
    }
    private void MOVETrigger(Collider other)
    {
        if(other.gameObject == mouse)
            mouse.SetActive(false);
    }
    private void MOVEExit()
    {

    }
    #endregion


    #region TRACE
    private void TRACEEnter()
    {
        transform.LookAt(targetTr);
        SetAnimationState("Scream");
    }
    private void TRACE()
    {
        // �ӵ��� ���� �ִϸ��̼� �ӵ� ��ȭ
        MoveCheck();

        //�߰�
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
            agent.SetDestination(targetTr.position);

        Vector3 checkPosition = transform.position + transform.forward * checkDistance;
        if (!IsGrounded(checkPosition))
        {
            ChangeState(State.FALL);
        }

    }
    private void TRACETrigger(Collider other)
    {

    }
    private void TRACEExit()
    {
        StartCoroutine(ResetPathAndWait());
    }
    #endregion
    //end FSM
    #endregion





    void MoveCheck()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void SetAnimationState(string stateName, float transitionDuration = 0.2f)
    {
        if (anim.HasState(0, Animator.StringToHash(stateName)))
        {
            anim.CrossFadeInFixedTime(stateName, transitionDuration);
        }
    }

    IEnumerator ResetPathAndWait()
    {
        // ��� �ʱ�ȭ
        agent.ResetPath();
        isPathResetting = true;

        // ��ΰ� ������ �ʱ�ȭ�� ������ ���
        yield return new WaitUntil(() => agent.path == null);
        yield return null;

        // ��� �ʱ�ȭ �Ϸ�
        isPathResetting = false;
    }

    bool IsGrounded(Vector3 position)
    {
        return Physics.CheckSphere(position, 0.2f, groundLayer);
    }
}
