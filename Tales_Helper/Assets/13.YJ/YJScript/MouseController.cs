using UnityEngine;
using UnityEngine.AI;

public class MouseController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void MoveToTarget(Transform targetPosition)
    {
        if (_navMeshAgent != null)
        {
            _navMeshAgent.SetDestination(targetPosition.position);
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
}
