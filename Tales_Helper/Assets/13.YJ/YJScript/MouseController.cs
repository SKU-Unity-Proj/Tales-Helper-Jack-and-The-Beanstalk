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
NavMeshAgent에 지정된 목적지까지 남은 거리를 반환.

(처음엔 정지 상태이기 때문에 항상 0을 반환한다.따라서 적 캐릭터가 이동 중이라는 조건도 함께 판단해야 한다.)



NavMeshAgent.velocity
속도를 의미함.이 속성의 크기로 이동 여부를 판단한다.



NavMeshAgent.velocity.sqrMagnitude
두 점 간의 거리를 구할 때 사용한다.

복잡한 수식을 사용하는 Vector3.Distance 보다 성능이 좋다.*/
}
