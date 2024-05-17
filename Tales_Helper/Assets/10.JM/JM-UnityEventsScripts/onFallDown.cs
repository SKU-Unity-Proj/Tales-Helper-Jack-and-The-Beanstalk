using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onFallDown : MonoBehaviour
{
    public Animator animator;
    public float delay = 0.2f; // ���� �ð��� 0.2�ʷ� ����


    // ������ �������� ���� ���� �ִϸ��̼� Ʈ���� �Լ�
    private void TriggerAnimation()
    {
        animator.SetTrigger("FallDownTrigger");
    }


    // �ܺο��� ȣ���� �޼ҵ�, ������ ����
    public void StartFallAnimation()
    {
        Invoke("TriggerAnimation", delay); // 'TriggerAnimation'�� 'delay' �ð� �Ŀ� ȣ��
    }

}
