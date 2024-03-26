using NPOI.POIFS.Properties;
using UnityEngine;
using UnityEngine.AI;

public class MouseController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void MoveToTarget(Transform targetPosition)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPosition.position);
        }
    }

    private void Update()
    {
        if (agent.velocity.magnitude > 0)
        {
            anim.SetBool("isMove", true);
            MoveCharacter(agent.desiredVelocity);
        }
        else
        {
            anim.SetBool("isMove", false);
        }
    }

    void MoveCharacter(Vector3 velocity)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        anim.SetFloat("Speed", speed);
    }



    /*
    NavMeshAgent.remainingDistance
NavMeshAgent�� ������ ���������� ���� �Ÿ��� ��ȯ.

(ó���� ���� �����̱� ������ �׻� 0�� ��ȯ�Ѵ�.���� �� ĳ���Ͱ� �̵� ���̶�� ���ǵ� �Բ� �Ǵ��ؾ� �Ѵ�.)



NavMeshAgent.velocity
�ӵ��� �ǹ���.�� �Ӽ��� ũ��� �̵� ���θ� �Ǵ��Ѵ�.



NavMeshAgent.velocity.sqrMagnitude
�� �� ���� �Ÿ��� ���� �� ����Ѵ�.

������ ������ ����ϴ� Vector3.Distance ���� ������ ����.*/
}
