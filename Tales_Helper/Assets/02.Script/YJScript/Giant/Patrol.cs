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
        //��ũ��Ʈ ������ �ڷ�ƾ �����Ѱ� -����
        StopCoroutine(enumerator);
        outWhile = true;
    }

    #region ���� ��ȸ
    IEnumerator Patroling()
    {
        int changeTime = Random.Range(2, 5);

        yield return new WaitForSeconds(changeTime);

        float currentTime = 0;
        float maxTime = 10;

        //��ǥ ��ġ ����
        agent.SetDestination(CalculateWanderPosition());

        //��ǥ ��ġ�� ȸ��
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

            //��ǥ ��ġ�� �����ϰų� �����ð� ��ȸ�ϱ� ���¿� �ӹ��� ������
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
        float wanderRadius = 10; //���� ��ġ�� �������� �ϴ� ������
        int wanderJitter = 0; //���õ� ����
        int wanderJitterMin = 0;
        int wanderJitterMax = 360;

        //���� �� ĳ���Ͱ� ������ �߽� ��ġ�� ũ��(������ ��� �ൿ�� ���� �ʵ���)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        //�ڽ��� ��ġ�� �߽����� ������, �Ÿ�, ������ ��ġ�� ��ǥ�� ��ǥ�������� ����
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targerPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        //������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
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
