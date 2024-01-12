using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class HearingSensor : MonoBehaviour
{
    EnemyAI LinkedAI; // �� ��ũ��Ʈ�� ������ ���� ������Ʈ�� ����� EnemyAI ������Ʈ

    void Start()
    {
        LinkedAI = GetComponent<EnemyAI>(); // EnemyAI ������Ʈ�� ã�� ����
        HearingManager.Instance.Register(this); // HearingManager�� �� ������ ���
    }


    void Update()
    {

    }

    // ���� ������Ʈ�� �ı��� �� ȣ��Ǵ� OnDestroy �Լ�
    void OnDestroy()
    {
        // HearingManager�� �����ϸ� �� ������ ����� ����
        if (HearingManager.Instance != null)
            HearingManager.Instance.Deregister(this);
    }

    // �Ҹ��� ����� �� ȣ��Ǵ� �Լ�
    public void OnHeardSound(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        // �Ҹ��� ��ġ�� û�� ���� �ۿ� ������ ����
        if (Vector3.Distance(location, LinkedAI.EyeLocation) > LinkedAI.HearingRange)
            return;

        // EnemyAI�� ���� �Ҹ��� ����
        LinkedAI.ReportCanHear(source, location, category, intensity);
    }
}
