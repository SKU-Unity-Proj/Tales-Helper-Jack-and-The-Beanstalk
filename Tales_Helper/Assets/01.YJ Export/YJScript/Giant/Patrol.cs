using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator anim;
    private bool outWhile;
    IEnumerator enumerator;

    void Start()
    {
        StartCoroutine("Patroling");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = this.GetComponent<Animator>();

        enumerator = Patroling();
    }


    void Update()
    {

    }
    void OnDisable()
    {
        //스크립트 꺼지면 코루틴 끄려한거 -무시
        StopCoroutine(enumerator);
        outWhile = true;
    }

    #region 거인 배회
    IEnumerator Patroling()
    {
        int changeTime = Random.Range(2, 5);

        yield return new WaitForSeconds(changeTime);

        float currentTime = 0;
        float maxTime = 10;

        //목표 위치 설정
        agent.SetDestination(CalculateWanderPosition());

        //목표 위치로 회전
        Vector3 to = new Vector3(agent.destination.x, 0, agent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);
        anim.SetBool("Move", true);

        while (true)
        {
            if (outWhile == true)
            {
                break;
            }
            currentTime += Time.deltaTime;

            //목표 위치에 근접하거나 오랜시간 배회하기 상태에 머물러 있으면
            to = new Vector3(agent.destination.x, 0, agent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.02f || currentTime >= maxTime)
            {
                anim.SetBool("Move", false);
                StartCoroutine("Patroling");
                break;
            }
            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10; //현재 위치를 원점으로 하는 반지름
        int wanderJitter = 0; //선택된 각도
        int wanderJitterMin = 0;
        int wanderJitterMax = 360;

        //현재 적 캐릭터가 월드의 중심 위치와 크기(구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        //자신의 위치를 중심으로 반지름, 거리, 각도에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targerPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        //생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
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
}
