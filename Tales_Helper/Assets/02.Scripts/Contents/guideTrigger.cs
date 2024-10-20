using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guideTrigger : MonoBehaviour
{
    public GameObject guideEffectPrefab; // 반짝이는 이펙트 프리팹
    public Transform targetPosition;  // 이펙트가 이동할 목표 위치 (개별 타겟)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 이펙트를 플레이어 머리 위에 생성
            Vector3 spawnPosition = other.transform.position + Vector3.up * 2.0f; // 플레이어 머리 위 위치
            GameObject guideEffect = Instantiate(guideEffectPrefab, spawnPosition, Quaternion.identity);

            // GuideEffect 스크립트에 목표 위치를 설정
            GuideEffect effectScript = guideEffect.GetComponent<GuideEffect>();
            if (effectScript != null)
            {
                effectScript.SetTarget(targetPosition);
            }
        }
    }
}
