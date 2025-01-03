using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onFallDown : MonoBehaviour
{
    public Animator animator;
    public float delay = 0.2f; // 지연 시간을 0.2초로 설정

    public SoundList fallStart, falling;

    // 지연을 포함하지 않은 실제 애니메이션 트리거 함수
    private void TriggerAnimation()
    {
        animator.SetTrigger("FallDownTrigger");
    }


    // 외부에서 호출할 메소드, 지연을 포함
    public void StartFallAnimation()
    {
        Invoke("TriggerAnimation", delay); // 'TriggerAnimation'을 'delay' 시간 후에 호출
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
