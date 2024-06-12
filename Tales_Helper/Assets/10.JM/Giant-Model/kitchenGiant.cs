using UnityEngine;
using UnityEngine.AI;

public class kitchenGiant : MonoBehaviour
{
    [SerializeField] private Transform player; // �÷��̾� ��ġ
    [SerializeField] private float detectionRange = 10.0f; // �÷��̾� ���� ����
    [SerializeField] private float actionCooldown = 5.0f; // �׼� ��ٿ� �ð�
    [SerializeField] private GameObject deskNavMeshLink; // å�� NavMeshLink

    private NavMeshAgent agent;
    private Animator animator;
    private bool isClimbing = false;
    private bool isChasingPlayer = false;
    private float lastActionTime;
    private bool isOnDesk = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //agent.enabled = false; // ó������ NavMeshAgent ��Ȱ��ȭ
        StartClimbDown(); // å�󿡼� �������� ����
    }

    private void Update()
    {
        if (isClimbing)
            return;

        if (isChasingPlayer)
        {
            ChasePlayer();
        }
        else if (isOnDesk && Time.time - lastActionTime > actionCooldown)
        {
            StartClimbDown();
        }
        else if (!isOnDesk && Time.time - lastActionTime > actionCooldown)
        {
            StartClimbing();
        }

        DetectPlayer();
    }

    private void StartClimbing()
    {
        isClimbing = true;
        //agent.isStopped = true;
        //animator.SetTrigger("Climb"); // Climb �ִϸ��̼� Ʈ����
    }

    private void StartClimbDown()
    {
        isClimbing = true;
        //animator.SetTrigger("ClimbDown"); // ClimbDown �ִϸ��̼� Ʈ����
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.destination = player.position;
        }
    }

    private void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log("Distance to player: " + distance);  // �÷��̾���� �Ÿ��� �α׷� ���
        if (distance <= detectionRange)
        {
            isChasingPlayer = true;
            //agent.isStopped = false;
        }
        else
        {
            isChasingPlayer = false;
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ���� ȣ��
    public void OnClimbComplete()
    {
        isClimbing = false;
        //agent.isStopped = false;
        lastActionTime = Time.time;
        if (isOnDesk)
        {
            deskNavMeshLink.SetActive(false); // å�� ��ũ ��Ȱ��ȭ
            isOnDesk = false;
            //agent.destination = deskNavMeshLink.endTransform.position; // å�� �Ʒ��� �̵�
        }
        else
        {
            deskNavMeshLink.SetActive(true); // å�� ��ũ Ȱ��ȭ
            isOnDesk = true;
            //agent.destination = deskNavMeshLink.startTransform.position; // å�� ���� �̵�
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ���� ȣ��
    public void OnClimbDownComplete()
    {
        isClimbing = false;
        //agent.enabled = true; // NavMeshAgent Ȱ��ȭ
        //agent.isStopped = false;
        lastActionTime = Time.time;
        if (!isChasingPlayer)
        {
            deskNavMeshLink.SetActive(false); // å�� ��ũ ��Ȱ��ȭ
            //agent.destination = deskNavMeshLink.endTransform.position; // å�� �Ʒ��� �̵�
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeskTop") && !isOnDesk)
        {
            StartClimbing();
        }
        else if (other.CompareTag("DeskBottom") && isOnDesk)
        {
            StartClimbDown();
        }
    }
    */
}
