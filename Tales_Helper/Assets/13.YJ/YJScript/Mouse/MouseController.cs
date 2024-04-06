using NPOI.POIFS.Properties;
using UnityEngine;
using UnityEngine.AI;

public class MouseController : MonoBehaviour
{
    private NavMeshAgent agent; // NavMeshAgent 컴포넌트에 대한 참조
    private Animator anim; // Animator 컴포넌트에 대한 참조
    private Rigidbody rigid;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    // 대상 위치로 이동하는 메서드
    public void MoveToTarget(Transform targetPosition)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPosition.position); // NavMeshAgent를 사용하여 대상 위치로 이동 명령 전달
        }
    }

    private void Update()
    {
        if (agent.velocity.magnitude > 0) // NavMeshAgent의 속도가 0보다 큰지 확인
        {
            anim.SetBool("isMove", true); // 애니메이션 상태를 이동 중으로 설정
            MoveCharacter(agent.desiredVelocity); // 캐릭터 이동 방향 설정
        }
        else
        {
            anim.SetBool("isMove", false); // 애니메이션 상태를 정지 상태로 설정
        }
    }

    // 캐릭터 이동을 처리하는 메서드
    void MoveCharacter(Vector3 velocity)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(velocity); // 월드 공간에서의 속도를 로컬 공간으로 변환
    }

    public void Die()
    {
        anim.SetTrigger("isDie");
        rigid.constraints = RigidbodyConstraints.FreezeAll;
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
