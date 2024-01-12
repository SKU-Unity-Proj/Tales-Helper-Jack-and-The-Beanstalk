using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class VisionSensor : MonoBehaviour
{
    [SerializeField] LayerMask DetectionMask = ~0; // Ž�� ����� �����ϴ� ���̾� ����ũ

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

            var vectorToTarget = candidateTarget.transform.position - LinkedAI.EyeLocation; // �������� ���� ���� ���

            // ����� ������ �þ� ���� �ۿ� �ִٸ� �˻����� �ʴ´�
            if (vectorToTarget.sqrMagnitude > (LinkedAI.VisionConeRange * LinkedAI.VisionConeRange))
                continue;

            vectorToTarget.Normalize(); // ���� ���͸� ����ȭ

            // ����� ������ �þ߰� �ۿ� �ִٸ� �˻����� �ʴ´�.
            if (Vector3.Dot(vectorToTarget, LinkedAI.EyeDirection) < LinkedAI.CosVisionConeAngle)
                continue;

            // ��� ���� ����ĳ��Ʈ�� ����
            RaycastHit hitResult;
            if (Physics.Raycast(LinkedAI.EyeLocation, vectorToTarget, out hitResult,
                                LinkedAI.VisionConeRange, DetectionMask, QueryTriggerInteraction.Collide))
            {
                // ����ĳ��Ʈ�� ��� �����ߴٸ�, EnemyAI�� ����� ����
                if (hitResult.collider.GetComponentInParent<DetectableTarget>() == candidateTarget)
                    LinkedAI.ReportCanSee(candidateTarget);
            }
        }
    }
}
