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
    private float attackCooldown = 2.0f; // ���� ��ٿ� �ð�

    private bool isAttacking = false;
    private Coroutine attackCoroutine = null; // �ڷ�ƾ ����

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
        Debug.Log("[State] ���� ����: AttackState");

        // �÷��̾� ���� ���� Ȯ��
        if (player == null || Vector3.Distance(agent.transform.position, player.position) > detectRange)
        {
            Debug.Log("[AttackState] �÷��̾ ��ħ �� WanderState�� �̵�");
            StopAttack();
            wanderState.Execute();
            return NodeState.SUCCESS;
        }

        float distanceToPlayer = Vector3.Distance(agent.transform.position, player.position);

        // ���� ������ ����� ChaseState�� �̵�
        if (distanceToPlayer > attackRadius)
        {
            Debug.Log("[AttackState] �÷��̾ ���� ������ ��� �� ChaseState�� �̵�");
            StopAttack();
            chaseState.Execute();
            return NodeState.SUCCESS;
        }

        // ���� ����
        if (!isAttacking)
        {
            isAttacking = true;
            agent.isStopped = true;
            animator.SetBool("Attack", true);

            // �÷��̾� �������� ȸ��
            Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;
            directionToPlayer.y = 0;
            agent.transform.rotation = Quaternion.LookRotation(directionToPlayer);

            Debug.Log("[AttackState] ���� ���� �� Coroutine ����");
            attackCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(AttackCoroutine());
        }

        return NodeState.RUNNING;
    }

    private IEnumerator AttackCoroutine()
    {
        // ���� ��ٿ� ���� ���
        yield return new WaitForSeconds(attackCooldown);

        Debug.Log("[AttackState] ���� �Ϸ� �� ���� Ȯ��");

        isAttacking = false;
        agent.isStopped = false;
        animator.SetBool("Attack", false);

        // ���� �� ���� Ȯ��
        if (Vector3.Distance(agent.transform.position, player.position) <= attackRadius)
        {
            Debug.Log("[AttackState] �÷��̾ ������ ���� ���� �� �� �ٽ� ����");
            isAttacking = true;
            attackCoroutine = agent.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(AttackCoroutine());
        }
        else
        {
            Debug.Log("[AttackState] �÷��̾ ���� ������ ��� �� ChaseState�� �̵�");
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
