using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class ProximitySensor : MonoBehaviour
{
    EnemyAI LinkedAI; // �� ��ũ��Ʈ�� ������ ���� ������Ʈ�� ����� EnemyAI ������Ʈ

    void Start()
    {
        LinkedAI = GetComponent<EnemyAI>(); // EnemyAI ������Ʈ�� ã�� ����  
    }

    void Update()
    {
        // DetectableTargetManager���� �����ϴ� ��� ������ ��ȸ��
        for (int index = 0; index < DetectableTargetManager.Instance.AllTargets.Count; ++index)
        {
            var candidateTarget = DetectableTargetManager.Instance.AllTargets[index]; // ���� �˻� ���� ���

            // �ڱ� �ڽ��� ��� �˻����� �ʴ´�
            if (candidateTarget.gameObject == gameObject)
                continue;

            // ������ �Ÿ��� ���� ���� ���� ���� �ִ��� Ȯ��
            if (Vector3.Distance(LinkedAI.EyeLocation, candidateTarget.transform.position) <= LinkedAI.ProximityDetectionRange)
                LinkedAI.ReportInProximity(candidateTarget); // ���� ����� EnemyAI�� ����
        }
    }
}
