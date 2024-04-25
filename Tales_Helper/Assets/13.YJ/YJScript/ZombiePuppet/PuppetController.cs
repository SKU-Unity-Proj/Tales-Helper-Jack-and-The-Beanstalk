using UnityEngine;
using UnityEngine.AI;

public class PuppetController : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    public Transform playerPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.velocity.magnitude > 0)
        {
            float speed = agent.velocity.magnitude / agent.speed;
            anim.SetFloat("Speed", speed);
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }
    }

    public void TracePlayer()
    {
        agent.SetDestination(playerPos.position);
    }
}
