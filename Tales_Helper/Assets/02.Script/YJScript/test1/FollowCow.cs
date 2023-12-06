using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowCow : MonoBehaviour
{

    public Transform target; // µû¶ó°¥ Å¸°ÙÀÇ Æ®·£½º Æû
    private float Dist;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("CS Character Controller").transform;
    }

    void FixedUpdate()
    {
        //transform.LookAt(target.transform);
        Dist = Vector3.Distance(target.transform.position, this.transform.position);

        if (Dist > 4f)
        {
            agent.destination = target.transform.position;
        }
    }
}
