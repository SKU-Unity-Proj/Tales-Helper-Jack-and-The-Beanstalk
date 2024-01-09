using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RestCondition : MonoBehaviour
{
    private float TimeToRest = 0.05f; // public���� �����Ͽ� �ν����Ϳ��� ���� �����ϵ��� �մϴ�.
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
            Debug.Log($"RestCondition Timer: {timer} / {TimeToRest}"); // Ÿ�̸� �α� ���

            if (timer >= TimeToRest)
            {
                isConditionMet = true;
                LinkedAI.OnRest();
                // ���� ���� �� �ʿ��� �̺�Ʈ�� �߻���ŵ�ϴ�.
                Debug.Log("RestCondition met."); // ������ �����Ǿ����� �α׷� ���
                // ��: OnRestConditionMet.Invoke();
            }
        }
    }

    public void ResetCondition()
    {
        timer = 0f;
        isConditionMet = false;
    }

    public bool IsStandingUp()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Stand");
    }
}

