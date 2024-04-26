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

    public DyingZombieAbility dyingZombieAbility; // DyingZombieAbility ��ũ��Ʈ�� �ν��Ͻ��� ������ ����

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

    public void TracePlayer() // Ʈ���ſ��� �߰� ���
    {
        agent.SetDestination(playerPos.position);
        isTrace = true;
    }

    private void TracePlayerUpdate() // �߰� ������Ʈ
    {
        if(isTrace)
            agent.SetDestination(playerPos.position);
    }

    private void AnimatorMovement() // ���� �ִϸ��̼� �ӵ� ����
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

    private void OnTriggerEnter(Collider col) // �ݶ��̴��� ������ �÷��̾� ����
    {
        if (col.CompareTag("Player"))
        {
            anim.CrossFadeInFixedTime("Biting", 0f);

            dyingZombieAbility.isDie = true;

            dyingLight.SetActive(true);
        }
    }
}
