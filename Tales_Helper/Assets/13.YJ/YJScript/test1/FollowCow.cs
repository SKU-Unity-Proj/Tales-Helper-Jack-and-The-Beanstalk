using NPOI.POIFS.Properties;
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

    void FixedUpdate()
    {
        // �÷��̾ ����
        ChasePlayer();
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            // �÷��̾��� ��ġ�� �̵�
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
