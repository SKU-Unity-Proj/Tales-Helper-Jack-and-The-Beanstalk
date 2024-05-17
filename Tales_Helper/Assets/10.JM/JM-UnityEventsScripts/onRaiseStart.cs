using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onRaiseStart : MonoBehaviour
{
    public Animator animator;
    public float delay = 0.2f; // ���� �ð��� 0.2�ʷ� ����
    public float Rdelay = 0.2f; // ���� �ð��� 0.2�ʷ� ����

    // ������ �������� ���� ���� �ִϸ��̼� Ʈ���� �Լ�
    private void TriggerAnimation()
    {
        animator.SetTrigger("RaiseTrigger");
    }

    private void OpenTriggerAnimation()
    {
        animator.SetTrigger("OpenTrigger");
    }

    // �ܺο��� ȣ���� �޼ҵ�, ������ ����
    public void StartRaiseAnimation()
    {
        Invoke("TriggerAnimation", delay); // 'TriggerAnimation'�� 'delay' �ð� �Ŀ� ȣ��
    }

    public void EndRaiseAnimation()
    {
        Invoke("OpenTriggerAnimation", Rdelay); // 'TriggerAnimation'�� 'delay' �ð� �Ŀ� ȣ��
    }
}
