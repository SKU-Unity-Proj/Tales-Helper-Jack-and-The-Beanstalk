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

    private Vector3 originalPosition; // �������ϴ� ��ġ
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

    #region FSM
    public enum State
    {
        DIE,
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
            case State.DIE:
                DIE();
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
            case State.DIE:
                DIETrigger(other);
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
            case State.RETURN:
                RETURNExit();
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
    private void IDLECollision(Collision other)
    {
        if (other.collider.CompareTag("Item"))
        {
            ChangeState(State.DIE);
        }
    }
    private void IDLEExit()
    {

    }
    #endregion


    #region DIE
    private void DIEEnter()
    {
        SetAnimationState("Die", 0.2f);
        agent.isStopped = true;
        agent.updateRotation = false;
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
            Debug.Log("a");
            agent.ResetPath();
            SetAnimationState("Angry", 0.4f);
            Invoke("OffMouse", 6.4f);
            BoxCollider boxCol = GetComponent<BoxCollider>();
            boxCol.enabled = false;
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

        // ����� ��ġ���� �Ÿ��� �Ӱ谪 ������ ���
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
        // ��� �ʱ�ȭ
        agent.ResetPath();
        isPathResetting = true;

        // ��ΰ� ������ �ʱ�ȭ�� ������ ���
        yield return new WaitUntil(() => agent.path == null);
        yield return null;

        // ��� �ʱ�ȭ �Ϸ�
        isPathResetting = false;
    }
}
