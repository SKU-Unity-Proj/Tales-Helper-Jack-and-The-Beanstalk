using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestCondition : MonoBehaviour
{
    public float TimeToRest = 10f; // public���� �����Ͽ� �ν����Ϳ��� ���� �����ϵ��� �մϴ�.
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
                // ���� ���� �� �ʿ��� �̺�Ʈ�� �߻���ŵ�ϴ�.
                // ��: OnRestConditionMet.Invoke();
            }
        }
    }

    public void ResetCondition()
    {
        timer = 0f;
        isConditionMet = false;
    }
}

