using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OrcFSM : MonoBehaviour
{
    private Transform targetPos;
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject mouse;
    public GameObject catchMouse;

    private Vector3 originalPosition; // 설거지하는 위치
    private Quaternion originalRotation;

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

        SavePositionAndRotation();
    }

    void Update()
    {
        
    }

    #region FSM
    public enum State
    {
        WANDER,
        IDLE,
        MOVE,
        RETURN
    }

    public State state;

    IEnumerator CheckMonsterState()
    {
        yield return new WaitForSeconds(0.5f);

        switch (state)
        {
            case State.WANDER:
                WANDER();
                break;
            case State.IDLE:
                IDLE();
                break;
            case State.MOVE:
                MOVE();
                break;
            case State.RETURN:
                RETURN();
                break;
        }
        StartCoroutine(CheckMonsterState());
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (state)
        {
            case State.WANDER:
                WANDERTrigger(other);
                break;
            case State.IDLE:
                IDLETrigger(other);
                break;
            case State.MOVE:
                MOVETrigger(other);
                break;
            case State.RETURN:
                RETURNTrigger(other);
                break;
        }
    }

    public void ChangeState(State state)
    {
        switch (this.state)
        {
            case State.WANDER:
                WANDERExit();
                break;
            case State.IDLE:
                IDLEExit();
                break;
            case State.MOVE:
                MOVEExit();
                break;
            case State.RETURN:
                RETURNExit();
                break;
        }

        this.state = state;

        switch (state)
        {
            case State.WANDER:
                WANDEREnter();
                break;
            case State.IDLE:
                IDLEEnter();
                break;
            case State.MOVE:
                MOVEEnter();
                break;
            case State.RETURN:
                RETURNEnter();
                break;
        }
    }


    #region IDLE
    private void IDLEEnter()
    {
        agent.ResetPath();
        SetAnimationState("WashingDish", 0.8f);
    }
    private void IDLE()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Look Over"))
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                ChangeState(State.MOVE);
    }
    private void IDLETrigger(Collider other)
    {

    }
    private void IDLEExit()
    {

    }
    #endregion


    #region WANDER
    private void WANDEREnter()
    {

    }
    private void WANDER()
    {

    }
    private void WANDERTrigger(Collider other)
    {

    }
    private void WANDERTriggerStay(Collider other)
    {

    }
    private void WANDERExit()
    {

    }
    #endregion


    #region MOVE
    private void MOVEEnter()
    {
        targetPos = mouse.transform;
        agent.SetDestination(targetPos.position);
    }
    private void MOVE()
    {
        MoveCheck();

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Catch"))
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                ChangeState(State.RETURN);
                catchMouse.SetActive(false);
            }
    }
    private void MOVETrigger(Collider other)
    {
        if (other.CompareTag("Mouse"))
        {
            agent.ResetPath();
            SetAnimationState("Catch");
            Invoke("OffMouse", 2f);
        }
    }
    private void MOVEExit()
    {

    }
    #endregion


    #region RETURN
    private void RETURNEnter()
    {
        agent.SetDestination(originalPosition);
    }
    private void RETURN()
    {
        MoveCheck();

        float distanceToSavedPosition = Vector3.Distance(transform.position, originalPosition);

        // 저장된 위치와의 거리가 임계값 이하인 경우
        if (distanceToSavedPosition <= 2f && !isPathResetting)
        {
            StartCoroutine(ResetPathAndWait());

            transform.position = originalPosition;
            transform.rotation = originalRotation;
            ChangeState(State.IDLE);
        }
    }
    private void RETURNTrigger(Collider other)
    {

    }
    private void RETURNExit()
    {

    }
    #endregion
    //end FSM
    #endregion





    void MoveCheck()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void SavePositionAndRotation()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void TrapOn()
    {
        SetAnimationState("Look Over");
    }

    void OffMouse()
    {
        mouse.SetActive(false);
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
