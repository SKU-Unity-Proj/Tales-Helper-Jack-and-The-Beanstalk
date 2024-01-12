using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class VisionSensor : MonoBehaviour
{
    [SerializeField] LayerMask DetectionMask = ~0; // 탐지 대상을 결정하는 레이어 마스크

    EnemyAI LinkedAI; // 이 스크립트가 부착된 게임 오브젝트에 연결된 EnemyAI 컴포넌트

    void Start()
    {
        LinkedAI = GetComponent<EnemyAI>(); // EnemyAI 컴포넌트를 찾아 연결
    }


    void Update()
    {
        // DetectableTargetManager에서 관리하는 모든 대상들을 순회함
        for (int index = 0; index < DetectableTargetManager.Instance.AllTargets.Count; ++index)
        {
            var candidateTarget = DetectableTargetManager.Instance.AllTargets[index]; // 현재 검사 중인 대상

            // 자기 자신인 경우 검사하지 않는다
            if (candidateTarget.gameObject == gameObject)
                continue;

            var vectorToTarget = candidateTarget.transform.position - LinkedAI.EyeLocation; // 대상까지의 방향 벡터 계산

            // 대상이 거인의 시야 범위 밖에 있다면 검사하지 않는다
            if (vectorToTarget.sqrMagnitude > (LinkedAI.VisionConeRange * LinkedAI.VisionConeRange))
                continue;

            vectorToTarget.Normalize(); // 방향 벡터를 정규화

            // 대상이 거인의 시야각 밖에 있다면 검사하지 않는다.
            if (Vector3.Dot(vectorToTarget, LinkedAI.EyeDirection) < LinkedAI.CosVisionConeAngle)
                continue;

            // 대상에 대한 레이캐스트를 수행
            RaycastHit hitResult;
            if (Physics.Raycast(LinkedAI.EyeLocation, vectorToTarget, out hitResult,
                                LinkedAI.VisionConeRange, DetectionMask, QueryTriggerInteraction.Collide))
            {
                // 레이캐스트가 대상에 도달했다면, EnemyAI에 대상을 보고
                if (hitResult.collider.GetComponentInParent<DetectableTarget>() == candidateTarget)
                    LinkedAI.ReportCanSee(candidateTarget);
            }
        }
    }
}
