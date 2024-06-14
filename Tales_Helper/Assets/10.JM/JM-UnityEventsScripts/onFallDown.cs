using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onFallDown : MonoBehaviour
{
    public Animator animator;
    public float delay = 0.2f; // ���� �ð��� 0.2�ʷ� ����

    public SoundList fallStart, falling;

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

    public void GiantPlaySwing()
    {
        SoundManager.Instance.PlayOneShotEffect((int)fallStart, transform.position, 3f);
    }
    public void GiantPlayJump()
    {
        SoundManager.Instance.PlayOneShotEffect((int)falling, transform.position, 5f);
    }

}
