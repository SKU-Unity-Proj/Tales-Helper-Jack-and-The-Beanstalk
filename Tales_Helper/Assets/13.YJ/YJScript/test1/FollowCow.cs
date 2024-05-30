using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowCow : MonoBehaviour
{

    public Transform player; // �÷��̾��� ��ġ�� �����ϱ� ���� ����
    private NavMeshAgent navMeshAgent;
    private Animator anim;

    void Start()
    {
        // NavMeshAgent ������Ʈ�� ������
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // �÷��̾ ����
        ChasePlayer();
        MoveCheck();
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            // �÷��̾��� ��ġ�� �̵�
            navMeshAgent.SetDestination(player.position);
        }
        anim.CrossFade("Cow_Walk", 0f);
    }

    void MoveCheck()
    {
        anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }
}
