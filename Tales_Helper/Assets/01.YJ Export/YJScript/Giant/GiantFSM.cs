using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//IDLE 상태 일 때 해당 위치에 가서 할 일 하게 만들기
//TRACE 상태일 때 몇 초간 플레이어가 시야에 없으면 IDLE 상태로 전환
//시야각의 높이 구별로 공격 모션 다르게 하기 (플레이어가 위에 있는지 아래있는지에 따라)


public class GiantFSM : MonoBehaviour
{
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;
    [SerializeField] private GameObject KitchenPos;
    [SerializeField] private GameObject LivingPos;

    private Transform playerTr;
    private UnityEngine.AI.NavMeshAgent agent;
    private GameObject catchBox;
    private GameObject checkRoom;
    private Animator anim;
    private int findSeconds;
    private bool isMove;

    void Awake()
    {
        StartCoroutine(View());
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Start()
    {
        ChangeState(State.IDLE);
        StartCoroutine(CheckMonsterState());
        anim = this.GetComponent<Animator>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        catchBox = this.transform.GetChild(0).gameObject;
        checkRoom = this.transform.GetChild(1).gameObject;

        isMove = false;
    }

    void Update()
    {
        RoomMove();
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
            if (_targetTf.name == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.name == "Player")
                        {
                            //Debug.Log("플레이어가 시야 내에 있습니다.");

                            if (this.state != State.TRACE && catchBox.activeSelf == false)
                            {
                                ChangeState(State.TRACE);
                            }

                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                        }
                        else if (this.state == State.TRACE && _targetTf==null)
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

    #region 거인 FSM

    public enum State
    {
        WANDER,
        IDLE,
        TRACE,
        ATTACK
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
            case State.TRACE:
                TRACE();
                break;
            case State.ATTACK:
                ATTACK();
                break;
        }
        StartCoroutine(CheckMonsterState());
    }

    private void OnTriggerStay(Collider other)
    {
        if (state == State.WANDER)
        {
            WANDERTriggerStay(other);
        }
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
            case State.TRACE:
                TRACETrigger(other);
                break;
            case State.ATTACK:
                ATTACKTrigger(other);
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
            case State.TRACE:
                TRACEExit();
                break;
            case State.ATTACK:
                ATTACKExit();
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
            case State.TRACE:
                TRACEEnter();
                break;
            case State.ATTACK:
                ATTACKEnter();
                break;
        }
    }



    private void IDLEEnter()
    {
        agent.ResetPath();
        transform.GetChild(1).gameObject.SetActive(true);
        StartCoroutine("IdleToWander");
    }
    private void IDLE()
    {

    }
    private void IDLETrigger(Collider other)
    {
        if (other.GetComponent<Collider>().CompareTag("Kitchen"))
        {
            isMove = true;
            agent.SetDestination(KitchenPos.transform.position);
        }
        if (other.GetComponent<Collider>().CompareTag("Livingroom"))
        {
            isMove = true;
            agent.SetDestination(LivingPos.transform.position);
        }
    }
    private void IDLEExit()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        agent.ResetPath();
    }




    private void WANDEREnter()
    {
        StartCoroutine("Patrol");
        StartCoroutine("WanderToIdle");
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
        StopCoroutine("Patrol");
    }




    private void TRACEEnter()
    {
        agent.ResetPath();
        catchBox.SetActive(true);
    }
    private void TRACE()
    {
        agent.SetDestination(playerTr.position);
    }
    private void TRACETrigger(Collider other)
    {
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            ChangeState(State.ATTACK);
        } 
    }
    private void TRACEExit()
    {
        catchBox.SetActive(false);
    }




    private void ATTACKEnter()
    {
        Debug.Log("ATTACK Start");
        anim.SetTrigger("Catch");
    }
    private void ATTACK()
    {
        ChangeState(State.IDLE);
    }
    private void ATTACKTrigger(Collider other)
    {

    }
    private void ATTACKExit()
    {
        
    }
    #endregion

    IEnumerator WanderToIdle()
    {
        yield return new WaitForSeconds(10f);
        ChangeState(State.IDLE);

        if (state == State.TRACE)
        {
            StopCoroutine("WanderToIdle");
        }
    }
    IEnumerator IdleToWander()
    {
        yield return new WaitForSeconds(10f);
        ChangeState(State.WANDER);

        if (state == State.TRACE)
        {
            StopCoroutine("IdleToWander");
        }
    }

    #region 거인 배회
    IEnumerator Patrol()
    {
        int changeTime = Random.Range(2, 5);

        yield return new WaitForSeconds(changeTime);

        float currentTime = 0;
        float maxTime = 10;

        agent.SetDestination(CalculateWanderPosition());

        Vector3 to = new Vector3(agent.destination.x, 0, agent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);
        anim.SetBool("Walk", true);

        while (true)
        {
            currentTime += Time.deltaTime;

            to = new Vector3(agent.destination.x, 0, agent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.1f && state == State.WANDER || currentTime >= maxTime)
            {
                anim.SetBool("Walk", false);
                StartCoroutine("Patrol");
                break;
            }
            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 20; 
        int wanderJitter = 0; 
        int wanderJitterMin = 0;
        int wanderJitterMax = 360;

        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targerPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        targerPosition.x = Mathf.Clamp(targerPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targerPosition.y = 0.0f;
        targerPosition.z = Mathf.Clamp(targerPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targerPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, agent.destination - transform.position);
    }
    */
    #endregion

    #region 할 일 이동
    //방 위치에 따라 청소, TV보러 가기 등 할 일 위치로 이동하는 스크립트
    private void RoomMove()
    {
        if (isMove == true && state == State.ATTACK )
        {
            agent.SetDestination(KitchenPos.transform.position);
        }
        else
        {
            isMove = false;
        }

        if ((KitchenPos.transform.position - this.transform.position).sqrMagnitude < 0.1f && isMove == true)
        {
            isMove = false;
            anim.SetBool("Walk", false);
        }
    }
    #endregion
}
