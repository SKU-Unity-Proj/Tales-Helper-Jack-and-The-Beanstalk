using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.Components;

public class TrackedTarget
{
    public DetectableTarget Detectable; // 감지 가능한 대상
    public Vector3 RawPosition; // 대상의 원시 위치

    public float LastSensedTime = -1f; // 마지막으로 감지된 시간
    public float Awareness; // 인지도
                            // 0 = 인지하지 못함
                            // 0-1 = 대략적 위치 인지
                            // 1-2 = 정확한 위치 인지
                            // 2 = 완전히 감지

    // 인지도를 업데이트하는 메소드
    public bool UpdateAwareness(DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        var oldAwareness = Awareness;

        if (target != null)
            Detectable = target; // 대상 설정
        RawPosition = position; // 위치 설정
        LastSensedTime = Time.time; // 마지막 감지 시간 갱신
        Awareness = Mathf.Clamp(Mathf.Max(Awareness, minAwareness) + awareness, 0f, 2f); // 인지도 업데이트

        // 인지도 변화에 따른 반환값 결정
        if (oldAwareness < 2f && Awareness >= 2f)
            return true;
        if (oldAwareness < 1f && Awareness >= 1f)
            return true;
        if (oldAwareness <= 0f && Awareness >= 0f)
            return true;

        return false;
    }

    // 인지도 감소 메소드
    public bool DecayAwareness(float decayTime, float amount)
    {
        // 최근에 감지되었다면 변화 없음
        if ((Time.time - LastSensedTime) < decayTime)
            return false;

        var oldAwareness = Awareness;

        // 인지도 감소
        Awareness -= amount;

        // 인지도 변화에 따른 반환값 결정
        if (oldAwareness >= 2f && Awareness < 2f)
            return true;
        if (oldAwareness >= 1f && Awareness < 1f)
            return true;
        return Awareness <= 0f;
    }
}

[RequireComponent(typeof(EnemyAI))]
public class AwarenessSystem : MonoBehaviour
{
    // 인식 관련 설정
    [SerializeField] AnimationCurve VisionSensitivity; // 시각적 감도
    [SerializeField] float VisionMinimumAwareness = 1f; // 시각 최소 인지도
    [SerializeField] float VisionAwarenessBuildRate = 10f; // 시각 인지도 증가율

    [SerializeField] float HearingMinimumAwareness = 0f; // 청각 최소 인지도
    [SerializeField] float HearingAwarenessBuildRate = 0.5f; // 청각 인지도 증가율

    [SerializeField] float ProximityMinimumAwareness = 0f; // 근접 최소 인지도
    [SerializeField] float ProximityAwarenessBuildRate = 1f; // 근접 인지도 증가율

    [SerializeField] float AwarenessDecayDelay = 0.1f; // 인지도 감소 지연 시간
    [SerializeField] float AwarenessDecayRate = 0.1f; // 인지도 감소율

    Dictionary<GameObject, TrackedTarget> Targets = new Dictionary<GameObject, TrackedTarget>(); // 감지 대상 목록
    EnemyAI LinkedAI; // 연결된 적 AI

    public Dictionary<GameObject, TrackedTarget> ActiveTargets => Targets; // 활성화된 대상들에 대한 접근

    // 시작 시 호출
    void Start()
    {
        LinkedAI = GetComponent<EnemyAI>(); // EnemyAI 컴포넌트 참조
    }

    // 매 프레임 업데이트
    void Update()
    {
        List<GameObject> toCleanup = new List<GameObject>(); // 제거할 대상 목록
        foreach (var targetGO in Targets.Keys)
        {
            if (Targets[targetGO].DecayAwareness(AwarenessDecayDelay, AwarenessDecayRate * Time.deltaTime))
            {
                if (Targets[targetGO].Awareness <= 0f) // 인지도가 0 이하인 경우
                {
                    LinkedAI.OnFullyLost(); // 완전히 감지하지 못한 경우
                    toCleanup.Add(targetGO); // 제거 목록에 추가
                }
                else
                {
                    // 인지도 감소에 따른 처리
                    if (Targets[targetGO].Awareness >= 1f)
                        LinkedAI.OnLostDetect(targetGO); // 감지가 소실된 경우
                    else
                        LinkedAI.OnLostSuspicion(); // 의심이 감소된 경우
                }
            }
        }

        // 더 이상 감지되지 않는 대상들 제거
        foreach (var target in toCleanup)
            Targets.Remove(target);
    }

    private void CheckAndHandlePlayerDetection(GameObject targetGO, TrackedTarget trackedTarget)
    {
        if (targetGO.tag == "Player" && trackedTarget.Awareness >= 2f)
        {
            // 플레이어의 Health 컴포넌트 찾기
            Health playerHealth = targetGO.GetComponent<Health>();
            if (playerHealth != null)
            {
                // 플레이어의 체력을 0으로 설정
                playerHealth.Damage(playerHealth.CurrentHP);
            }
        }
    }

    // 인지도 업데이트
    void UpdateAwareness(GameObject targetGO, DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        if (!Targets.ContainsKey(targetGO)) // 대상이 리스트에 없는 경우
            Targets[targetGO] = new TrackedTarget(); // 새 대상 추가

        // 대상 인지도 업데이트
        if (Targets[targetGO].UpdateAwareness(target, position, awareness, minAwareness))
        {
            //CheckAndHandlePlayerDetection(targetGO, Targets[targetGO]);

            // 인지도에 따른 처리
            if (Targets[targetGO].Awareness >= 2f)
                LinkedAI.OnFullyDetected(targetGO); // 완전히 감지된 경우
            else if (Targets[targetGO].Awareness >= 1f)
                LinkedAI.OnDetected(targetGO); // 감지된 경우
            else if (Targets[targetGO].Awareness >= 0f)
                LinkedAI.OnSuspicious(); // 의심되는 경우
        }
    }

    // 시각적 감지 보고
    public void ReportCanSee(DetectableTarget seen)
    {
        // 대상이 시야 범위 내에 있는지 확인
        var vectorToTarget = (seen.transform.position - LinkedAI.EyeLocation).normalized;
        var dotProduct = Vector3.Dot(vectorToTarget, LinkedAI.EyeDirection);

        // 인지도 기여도 계산
        var awareness = VisionSensitivity.Evaluate(dotProduct) * VisionAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(seen.gameObject, seen, seen.transform.position, awareness, VisionMinimumAwareness);
    }

    // 청각적 감지 보고
    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        // 청각적 인지도 계산
        var awareness = intensity * HearingAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(source, null, location, awareness, HearingMinimumAwareness);
    }

    // 근접 감지 보고
    public void ReportInProximity(DetectableTarget target)
    {
        // 근접 인지도 계산
        var awareness = ProximityAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, ProximityMinimumAwareness);
    }
}