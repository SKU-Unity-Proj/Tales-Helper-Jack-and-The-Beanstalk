using NPOI.POIFS.Properties;
using UnityEngine;
using UnityEngine.AI;

public class MouseController : MonoBehaviour
{
    private NavMeshAgent agent; // NavMeshAgent ������Ʈ�� ���� ����
    private Animator anim; // Animator ������Ʈ�� ���� ����
    private Rigidbody rigid;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    // ��� ��ġ�� �̵��ϴ� �޼���
    public void MoveToTarget(Transform targetPosition)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPosition.position); // NavMeshAgent�� ����Ͽ� ��� ��ġ�� �̵� ��� ����
        }
    }

    private void Update()
    {
        if (agent.velocity.magnitude > 0) // NavMeshAgent�� �ӵ��� 0���� ū�� Ȯ��
        {
            anim.SetBool("isMove", true); // �ִϸ��̼� ���¸� �̵� ������ ����
            MoveCharacter(agent.desiredVelocity); // ĳ���� �̵� ���� ����
        }
        else
        {
            anim.SetBool("isMove", false); // �ִϸ��̼� ���¸� ���� ���·� ����
        }
    }

    // ĳ���� �̵��� ó���ϴ� �޼���
    void MoveCharacter(Vector3 velocity)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(velocity); // ���� ���������� �ӵ��� ���� �������� ��ȯ
    }

    public void Die()
    {
        anim.SetTrigger("isDie");
        rigid.constraints = RigidbodyConstraints.FreezeAll;
    }
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
