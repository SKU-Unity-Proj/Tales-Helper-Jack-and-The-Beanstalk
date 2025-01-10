using UnityEngine;
using System.Collections;

public class GemManager : MonoBehaviour
{
    [Header("Gem Activation Settings")]
    [SerializeField] private int totalTargets = 3; // 필요한 총 타겟 개수
    private int activeTargets = 0; // 현재 활성화된 타겟 개수

    [Header("Motor Settings")]
    [SerializeField] private Animator motorAnimator; // 모터의 애니메이터
    [SerializeField] private float maxMotorSpeed = 2f; // 모터의 최대 속도
    [SerializeField] private float speedIncreaseStep = 0.67f; // 속도 증가량 (maxMotorSpeed / totalTargets)

    [Header("Motor Settings")]
    [SerializeField] private AudioSource motorSound; // 모터 사운드
    [SerializeField] private float[] motorVolumes = { 0.05f, 0.15f, 0.3f }; // 각 타겟 활성화에 따른 볼륨 값

    [Header("Ladder Settings")]
    [SerializeField] private GameObject ladder; // 사다리 오브젝트
    [SerializeField] private Vector3 ladderDropPosition = new Vector3(2.57999992f, 8.64000034f, -103.75f); // 사다리가 떨어질 위치
    [SerializeField] private float ladderDropSpeed = 5f; // 사다리가 떨어지는 속도

    [Header("Effects")]
    [SerializeField] private AudioSource dropSound; // 쿵 소리 효과음
    [SerializeField] private GameObject dustEffectPrefab; // 충격 먼지 효과

    private bool isLadderDropped = false; // 사다리가 이미 떨어졌는지 확인

    public void IncrementActiveTargets()
    {
        if (isLadderDropped) return; // 이미 사다리가 떨어졌다면 실행하지 않음

        activeTargets++;
        Debug.Log($"Active Targets: {activeTargets}/{totalTargets}");

        // 모터 속도 및 사운드 업데이트
        UpdateGearSpeed();
        UpdateMotorSound();

        // 모든 타겟이 활성화되면 사다리 떨어뜨리기
        if (activeTargets >= totalTargets)
        {
            DropLadder();
        }
    }

    private void UpdateGearSpeed()
    {
        // 활성화된 타겟 수에 따라 모터 속도 증가
        float currentSpeed = Mathf.Clamp(activeTargets * speedIncreaseStep, 0, maxMotorSpeed);

        // 특정 상태(Gear)의 Speed 파라미터 설정
        if (motorAnimator != null)
        {
            motorAnimator.SetFloat("Speed", currentSpeed); // Gear 상태의 속도만 조절
            Debug.Log($"Gear Speed updated: {currentSpeed}");
        }
        else
        {
            Debug.LogError("Motor Animator is not assigned!");
        }
    }

    private void UpdateMotorSound()
    {
        if (motorSound == null) return;

        // 타겟 개수에 따라 볼륨 조절
        if (activeTargets > 0 && activeTargets <= motorVolumes.Length)
        {
            motorSound.volume = motorVolumes[activeTargets - 1];
            if (!motorSound.isPlaying)
            {
                motorSound.loop = true; // 반복 재생 활성화
                motorSound.Play(); // 사운드 시작
            }
            Debug.Log($"Motor sound volume updated: {motorSound.volume}");
        }
    }

    private void DropLadder()
    {
        if (ladder == null)
        {
            Debug.LogError("Ladder is not assigned in GemManager!");
            return;
        }

        Debug.Log("Dropping the ladder!");
        isLadderDropped = true;

        StartCoroutine(DropLadderRoutine());

        // 사운드 중지
        if (motorSound != null)
        {
            motorSound.Stop();
            motorAnimator.SetFloat("Speed", 0f); // Gear 상태의 속도만 조절
        }
    }

    private IEnumerator DropLadderRoutine()
    {
        Vector3 startPosition = ladder.transform.position; // 현재 위치
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y - 5f, startPosition.z); // Y축 -5 이동

        // 사다리를 서서히 떨어뜨리기
        float time = 0;
        while (Vector3.Distance(ladder.transform.position, endPosition) > 0.01f)
        {
            ladder.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            time += Time.deltaTime * ladderDropSpeed;
            yield return null;
        }

        ladder.transform.position = endPosition; // 정확히 목표 위치에 설정

        // 먼지 이펙트 재생
        PlayDustEffect();

        // 효과 재생
        if (dropSound != null) dropSound.Play();

        Debug.Log("Ladder dropped with impact!");
    }

    private void PlayDustEffect()
    {
        if (dustEffectPrefab == null)
        {
            Debug.LogError("Dust Effect Prefab is not assigned!");
            return;
        }

        // 먼지 이펙트를 사다리의 위치에 생성
        Instantiate(dustEffectPrefab, ladder.transform.position, Quaternion.identity);
        Debug.Log("Dust effect instantiated at ladder position.");
    }
}
