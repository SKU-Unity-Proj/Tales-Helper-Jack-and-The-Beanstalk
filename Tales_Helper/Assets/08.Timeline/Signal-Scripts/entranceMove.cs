using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class entranceMove : MonoBehaviour
{
    public Transform newPosition;

    public Transform originObject;
    public Transform setObject;

    public Transform originChandelier;
    public List<Transform> transformList = new List<Transform>();

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // �ڽ� ������Ʈ�� ����Ʈ�� �߰�
        foreach (Transform child in originChandelier)
        {
            transformList.Add(child);
        }

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

    public void OnSignalReceiveChandelier()
    {
        foreach (Transform trans in transformList)
        {
            MeshCollider meshCollider = trans.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.enabled = true;
            }
        }

        originChandelier.gameObject.GetComponent<MeshRenderer>().enabled = false;
        originChandelier.gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

}
