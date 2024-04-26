using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;
using DiasGames.Abilities;

public class PuppetController : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    public bool isTrace = false;
    public GameObject dyingLight;
    public Transform playerPos;

    public DyingZombieAbility dyingZombieAbility; // DyingZombieAbility 스크립트의 인스턴스를 저장할 변수

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if(playerPos == null )
            playerPos = GameObject.FindWithTag("Player").transform;
        if(dyingZombieAbility == null )
            dyingZombieAbility = FindObjectOfType<DyingZombieAbility>();

        dyingLight.SetActive(false);
    }

    void Update()
    {
        AnimatorMovement();
        TracePlayerUpdate();
    }

    public void TracePlayer() // 트리거에서 추격 명령
    {
        agent.SetDestination(playerPos.position);
        isTrace = true;
    }

    private void TracePlayerUpdate() // 추격 업데이트
    {
        if(isTrace)
            agent.SetDestination(playerPos.position);
    }

    private void AnimatorMovement() // 기어가는 애니메이션 속도 조절
    {
        if (agent.velocity.magnitude > 0)
        {
            float speed = Mathf.Clamp(agent.velocity.magnitude / agent.speed, 0f, 1f);
            anim.SetFloat("Speed", speed);
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }
    }

    private void OnTriggerEnter(Collider col) // 콜라이더에 닿으면 플레이어 죽음
    {
        if (col.CompareTag("Player"))
        {
            anim.CrossFadeInFixedTime("Biting", 0f);

            dyingZombieAbility.isDie = true;

            dyingLight.SetActive(true);
        }
    }
}
