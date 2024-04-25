using UnityEngine;
using UnityEngine.AI;

public class PuppetController : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    public Transform playerPos;
    public bool isTrace = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if(playerPos == null )
            playerPos = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        AnimatorMovement();
        TracePlayerUpdate();
    }

    public void TracePlayer()
    {
        agent.SetDestination(playerPos.position);
        isTrace = true;
    }

    private void TracePlayerUpdate()
    {
        if(isTrace)
            agent.SetDestination(playerPos.position);
    }

    private void AnimatorMovement()
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
}
