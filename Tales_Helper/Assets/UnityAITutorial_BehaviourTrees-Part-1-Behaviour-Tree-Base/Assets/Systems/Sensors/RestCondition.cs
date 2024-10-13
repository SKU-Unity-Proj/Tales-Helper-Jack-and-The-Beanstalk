using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RestCondition : MonoBehaviour
{
    private float TimeToRest = 6.0f; // public으로 설정하여 인스펙터에서 조절 가능하도록 합니다.
    private float timer = 0f;

    private bool isConditionMet = false;

    //private EnemyAI LinkedAI;

    private Animator anim;

    private void Update()
    {
        //LinkedAI = GetComponent<EnemyAI>();
        anim = GetComponent<Animator>();
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
                //LinkedAI.OnRest();
                // 조건 충족 시 필요한 이벤트를 발생시킵니다.
                Debug.Log("RestCondition met."); // 조건이 충족되었음을 로그로 출력

            }
        }
    }

    public bool CheckCondition()
    {
        // 현재 조건을 반환하고, 조건이 충족된 후 리스트가 비어있다면 조건을 해제
        bool currentCondition = isConditionMet;
        if (isConditionMet && DroppedObject.Instance.DroppedObjects.Count > 0)
        {
            isConditionMet = false; // 모든 오브젝트 처리 후 조건 해제
        }
        return currentCondition;
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

