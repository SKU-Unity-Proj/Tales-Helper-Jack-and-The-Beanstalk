using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guideTrigger : MonoBehaviour
{
    public GameObject guideEffectPrefab; // ��¦�̴� ����Ʈ ������
    public Transform targetPosition;  // ����Ʈ�� �̵��� ��ǥ ��ġ (���� Ÿ��)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ����Ʈ�� �÷��̾� �Ӹ� ���� ����
            Vector3 spawnPosition = other.transform.position + Vector3.up * 2.0f; // �÷��̾� �Ӹ� �� ��ġ
            GameObject guideEffect = Instantiate(guideEffectPrefab, spawnPosition, Quaternion.identity);

            // GuideEffect ��ũ��Ʈ�� ��ǥ ��ġ�� ����
            GuideEffect effectScript = guideEffect.GetComponent<GuideEffect>();
            if (effectScript != null)
            {
                effectScript.SetTarget(targetPosition);
            }
        }
    }
}
