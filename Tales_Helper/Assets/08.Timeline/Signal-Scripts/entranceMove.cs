using UnityEngine;
using UnityEngine.AI;

public class entranceMove : MonoBehaviour
{
    public Transform newPosition;

    public Transform originObject;
    public Transform setObject;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

}
