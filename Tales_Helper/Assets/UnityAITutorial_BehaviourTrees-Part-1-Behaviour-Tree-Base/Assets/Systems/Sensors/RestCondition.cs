using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RestCondition : MonoBehaviour
{
    private float TimeToRest = 0.05f; // public���� �����Ͽ� �ν����Ϳ��� ���� �����ϵ��� �մϴ�.
    private float timer = 0f;

    private bool isConditionMet = false;

    private EnemyAI LinkedAI;
    private DroppedObject droppedObject;

    private Animator anim;

    private void Update()
    {
        LinkedAI = GetComponent<EnemyAI>();
        anim = GetComponent<Animator>();

        droppedObject = GameObject.FindObjectOfType<DroppedObject>();
        if (droppedObject == null)
        {
            Debug.LogError("DroppedObject component not found in the scene.");
            return;
        }
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

            }
        }
    }

    public bool CheckCondition()
    {
        // ���� ������ ��ȯ�ϰ�, ������ ������ �� ����Ʈ�� ����ִٸ� ������ ����
        bool currentCondition = isConditionMet;
        if (isConditionMet && droppedObject.DroppedObjects.Count > 0)
        {
            isConditionMet = false; // ��� ������Ʈ ó�� �� ���� ����
        }
        return currentCondition;
    }

    public void ResetCondition()
    {
        timer = 0f;
        isConditionMet = false;
    }

    // '�Ͼ��' �ִϸ��̼��� ���� ������ Ȯ���ϴ� �޼ҵ�
    public bool IsStandingUp()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Stand") && stateInfo.normalizedTime >= 1.0f;
    }

}

