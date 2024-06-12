using UnityEngine;
using UnityEngine.AI;

public class kitchenGiant : MonoBehaviour
{
    [SerializeField] private Transform player; // 플레이어 위치
    [SerializeField] private float detectionRange = 10.0f; // 플레이어 감지 범위
    [SerializeField] private float actionCooldown = 5.0f; // 액션 쿨다운 시간
    [SerializeField] private GameObject deskNavMeshLink; // 책상 NavMeshLink

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
        //agent.enabled = false; // 처음에는 NavMeshAgent 비활성화
        StartClimbDown(); // 책상에서 내려오기 시작
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
        //animator.SetTrigger("Climb"); // Climb 애니메이션 트리거
    }

    private void StartClimbDown()
    {
        isClimbing = true;
        //animator.SetTrigger("ClimbDown"); // ClimbDown 애니메이션 트리거
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
        Debug.Log("Distance to player: " + distance);  // 플레이어와의 거리를 로그로 출력
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

    // 애니메이션 이벤트를 통해 호출
    public void OnClimbComplete()
    {
        isClimbing = false;
        //agent.isStopped = false;
        lastActionTime = Time.time;
        if (isOnDesk)
        {
            deskNavMeshLink.SetActive(false); // 책상 링크 비활성화
            isOnDesk = false;
            //agent.destination = deskNavMeshLink.endTransform.position; // 책상 아래로 이동
        }
        else
        {
            deskNavMeshLink.SetActive(true); // 책상 링크 활성화
            isOnDesk = true;
            //agent.destination = deskNavMeshLink.startTransform.position; // 책상 위로 이동
        }
    }

    // 애니메이션 이벤트를 통해 호출
    public void OnClimbDownComplete()
    {
        isClimbing = false;
        //agent.enabled = true; // NavMeshAgent 활성화
        //agent.isStopped = false;
        lastActionTime = Time.time;
        if (!isChasingPlayer)
        {
            deskNavMeshLink.SetActive(false); // 책상 링크 비활성화
            //agent.destination = deskNavMeshLink.endTransform.position; // 책상 아래로 이동
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
