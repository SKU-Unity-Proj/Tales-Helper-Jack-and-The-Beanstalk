using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RestCondition : MonoBehaviour
{
    private float TimeToRest = 0.05f; // public으로 설정하여 인스펙터에서 조절 가능하도록 합니다.
    private float timer = 0f;
    private bool isConditionMet = false;

    EnemyAI LinkedAI;
    Animator anim;

    private void Update()
    {
        LinkedAI = GetComponent<EnemyAI>();
        anim = GetComponent<Animator>();
    }

    public bool CheckCondition()
    {
        return isConditionMet;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (!isConditionMet)
        {
            timer += deltaTime;
            Debug.Log($"RestCondition Timer: {timer} / {TimeToRest}"); // 타이머 로그 출력

            if (timer >= TimeToRest)
            {
                isConditionMet = true;
                LinkedAI.OnRest();
                // 조건 충족 시 필요한 이벤트를 발생시킵니다.
                Debug.Log("RestCondition met."); // 조건이 충족되었음을 로그로 출력

            }
        }
    }

    public void ResetCondition()
    {
        timer = 0f;
        isConditionMet = false;
    }

    // '일어나기' 애니메이션이 실행 중인지 확인하는 메소드
    public bool IsStandingUp()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Stand") && stateInfo.normalizedTime >= 1.0f;
    }

}

