using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class ProximitySensor : MonoBehaviour
{
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

            // 대상과의 거리가 근접 감지 범위 내에 있는지 확인
            if (Vector3.Distance(LinkedAI.EyeLocation, candidateTarget.transform.position) <= LinkedAI.ProximityDetectionRange)
                LinkedAI.ReportInProximity(candidateTarget); // 근접 대상을 EnemyAI에 보고
        }
    }
}
