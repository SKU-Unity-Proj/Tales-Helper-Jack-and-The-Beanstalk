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
    private GameObject mouse;
    private bool isPathResetting = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        mouse = GameObject.FindWithTag("Mouse");
    }

    void Start()
    {
        ChangeState(State.IDLE);
        StartCoroutine(CheckMonsterState());
    }

    #region 시야각
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
                            Debug.Log("플레이어가 시야 내에 있습니다.");

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
    public enum State
    {
        DIE,
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
            case State.DIE:
                DIE();
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
            case State.DIE:
                DIETrigger(other);
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
            case State.DIE:
                DIEExit();
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
            case State.DIE:
                DIEEnter();
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


    #region IDLE
    private void IDLEEnter()
    {
    }
    private void IDLE()
    {

    }
    private void IDLETrigger(Collider other)
    {

    }
    private void IDLECollision(Collision other)
    {

    }
    private void IDLEExit()
    {

    }
    #endregion


    #region DIE
    private void DIEEnter()
    {

    }
    private void DIE()
    {

    }
    private void DIETrigger(Collider other)
    {

    }
    private void DIEExit()
    {

    }
    #endregion


    #region MOVE
    private void MOVEEnter()
    {

    }
    private void MOVE()
    {
        MoveCheck();
    }
    private void MOVETrigger(Collider other)
    {

    }
    private void MOVEExit()
    {

    }
    #endregion


    #region RETURN
    private void TRACEEnter()
    {
        SetAnimationState("Run");
    }
    private void TRACE()
    {
        MoveCheck();
        agent.SetDestination(targetTr.position);
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
        // 경로 초기화
        agent.ResetPath();
        isPathResetting = true;

        // 경로가 완전히 초기화될 때까지 대기
        yield return new WaitUntil(() => agent.path == null);
        yield return null;

        // 경로 초기화 완료
        isPathResetting = false;
    }
}
