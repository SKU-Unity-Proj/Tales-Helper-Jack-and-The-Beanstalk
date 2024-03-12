using NPOI.POIFS.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowCow : MonoBehaviour
{

    public Transform player; // 플레이어의 위치를 추적하기 위한 변수
    private NavMeshAgent navMeshAgent;
    private Animator anim;

    void Start()
    {
        // NavMeshAgent 컴포넌트를 가져옴
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // 플레이어를 추적
        ChasePlayer();
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            // 플레이어의 위치로 이동
            navMeshAgent.SetDestination(player.position);
        }

        if (navMeshAgent.hasPath && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            anim.SetBool("isMove", true);
        }
        else
        {
            anim.SetBool("isMove", false);
        }
    }
}
