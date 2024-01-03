using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestCondition : MonoBehaviour
{
    public float TimeToRest = 10f; // public으로 설정하여 인스펙터에서 조절 가능하도록 합니다.
    private float timer = 0f;
    private bool isConditionMet = false;

    public bool CheckCondition()
    {
        return isConditionMet;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (!isConditionMet)
        {
            timer += deltaTime;
            if (timer >= TimeToRest)
            {
                isConditionMet = true;
                // 조건 충족 시 필요한 이벤트를 발생시킵니다.
                // 예: OnRestConditionMet.Invoke();
            }
        }
    }

    public void ResetCondition()
    {
        timer = 0f;
        isConditionMet = false;
    }
}

