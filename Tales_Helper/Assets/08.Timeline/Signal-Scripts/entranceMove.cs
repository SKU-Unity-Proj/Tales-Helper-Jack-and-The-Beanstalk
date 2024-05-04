using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class entranceMove : MonoBehaviour
{
    public Transform newPosition;

    public Transform originObject;
    public Transform setObject;

    private NavMeshAgent agent;
    private Animator _anim;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
    }

    public void OnSignalReceivedMove()
    {
        this.agent.SetDestination(newPosition.position);
    }

    public void OnSignalReceivedDoor()
    {
        originObject.gameObject.SetActive(false);
        setObject.gameObject.SetActive(true);
    }

    public void OnSignalReceivedDying()
    {
        _anim.SetBool("isDying", true);
    }

}
