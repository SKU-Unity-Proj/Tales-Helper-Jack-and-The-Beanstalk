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

    void Update()
    {
        // 플레이어를 추적
        ChasePlayer();
        MoveCheck();
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            // 플레이어의 위치로 이동
            navMeshAgent.SetDestination(player.position);
        }
        anim.CrossFade("Cow_Walk", 0f);
    }

    void MoveCheck()
    {
        anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }
}
