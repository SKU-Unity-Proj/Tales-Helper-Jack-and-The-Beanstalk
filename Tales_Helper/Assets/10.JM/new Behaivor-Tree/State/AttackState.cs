using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AttackState : BehaviorNode
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private float attackRadius;
    private float detectRange;
    private float attackCooldown = 2.0f; // 공격 쿨다운 시간

    private bool isAttacking = false;
    private Coroutine attackCoroutine = null; // 코루틴 참조

    private ChaseState chaseState;
    private WanderState wanderState;

    public AttackState(NavMeshAgent agent, Transform player, float attackRadius, float detectRange, Animator animator, ChaseState chaseState, WanderState wanderState)
    {
        this.agent = agent;
        this.player = player;
        this.attackRadius = attackRadius;
        this.detectRange = detectRange;
        this.animator = animator;
        this.chaseState = chaseState;
        this.wanderState = wanderState;
    }

    public override NodeState Execute()
    {
        Debug.Log("[State] 현재 상태: AttackState");

        // 플레이어 존재 여부 확인
        if (player == null || Vector3.Distance(agent.transform.position, player.position) > detectRange)
        {
            Debug.Log("[AttackState] 플레이어를 놓침 → WanderState로 이동");
            StopAttack();
            wanderState.Execute();
            return NodeState.SUCCESS;
        }

        float distanceToPlayer = Vector3.Distance(agent.transform.position, player.position);

        // 공격 범위를 벗어나면 ChaseState로 이동
        if (distanceToPlayer > attackRadius)
        {
            Debug.Log("[AttackState] 플레이어가 공격 범위를 벗어남 → ChaseState로 이동");
            StopAttack();
            chaseState.Execute();
            return NodeState.SUCCESS;
        }

        // 공격 시작
        if (!isAttacking)
        {
            isAttacking = true;
            agent.isStopped = true;
            animator.SetBool("Attack", true);

            // 플레이어 방향으로 회전
            Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;
            directionToPlayer.y = 0;
            agent.transform.rotation = Quaternion.LookRotation(directionToPlayer);

            Debug.Log("[AttackState] 공격 시작 → Coroutine 실행");
            attackCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(AttackCoroutine());
        }

        return NodeState.RUNNING;
    }

    private IEnumerator AttackCoroutine()
    {
        // 공격 쿨다운 동안 대기
        yield return new WaitForSeconds(attackCooldown);

        Debug.Log("[AttackState] 공격 완료 → 상태 확인");

        isAttacking = false;
        agent.isStopped = false;
        animator.SetBool("Attack", false);

        // 공격 후 상태 확인
        if (Vector3.Distance(agent.transform.position, player.position) <= attackRadius)
        {
            Debug.Log("[AttackState] 플레이어가 여전히 공격 범위 내 → 다시 공격");
            isAttacking = true;
            attackCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(AttackCoroutine());
        }
        else
        {
            Debug.Log("[AttackState] 플레이어가 공격 범위를 벗어남 → ChaseState로 이동");
            chaseState.Execute();
        }
    }

    private void StopAttack()
    {
        if (attackCoroutine != null)
        {
            agent.gameObject.GetComponent<MonoBehaviour>().StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;
        agent.isStopped = false;
        ResetAnimator();
    }

    private void ResetAnimator()
    {
        animator.SetBool("Move", false);
        animator.SetBool("Run", false);
        animator.SetBool("Attack", false);
    }
}
