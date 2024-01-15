using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.Components;

public class TrackedTarget
{
    public DetectableTarget Detectable; // ���� ������ ���
    public Vector3 RawPosition; // ����� ���� ��ġ

    public float LastSensedTime = -1f; // ���������� ������ �ð�
    public float Awareness; // ������
                            // 0 = �������� ����
                            // 0-1 = �뷫�� ��ġ ����
                            // 1-2 = ��Ȯ�� ��ġ ����
                            // 2 = ������ ����

    // �������� ������Ʈ�ϴ� �޼ҵ�
    public bool UpdateAwareness(DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        var oldAwareness = Awareness;

        if (target != null)
            Detectable = target; // ��� ����
        RawPosition = position; // ��ġ ����
        LastSensedTime = Time.time; // ������ ���� �ð� ����
        Awareness = Mathf.Clamp(Mathf.Max(Awareness, minAwareness) + awareness, 0f, 2f); // ������ ������Ʈ

        // ������ ��ȭ�� ���� ��ȯ�� ����
        if (oldAwareness < 2f && Awareness >= 2f)
            return true;
        if (oldAwareness < 1f && Awareness >= 1f)
            return true;
        if (oldAwareness <= 0f && Awareness >= 0f)
            return true;

        return false;
    }

    // ������ ���� �޼ҵ�
    public bool DecayAwareness(float decayTime, float amount)
    {
        // �ֱٿ� �����Ǿ��ٸ� ��ȭ ����
        if ((Time.time - LastSensedTime) < decayTime)
            return false;

        var oldAwareness = Awareness;

        // ������ ����
        Awareness -= amount;

        // ������ ��ȭ�� ���� ��ȯ�� ����
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
    // �ν� ���� ����
    [SerializeField] AnimationCurve VisionSensitivity; // �ð��� ����
    [SerializeField] float VisionMinimumAwareness = 1f; // �ð� �ּ� ������
    [SerializeField] float VisionAwarenessBuildRate = 10f; // �ð� ������ ������

    [SerializeField] float HearingMinimumAwareness = 0f; // û�� �ּ� ������
    [SerializeField] float HearingAwarenessBuildRate = 0.5f; // û�� ������ ������

    [SerializeField] float ProximityMinimumAwareness = 0f; // ���� �ּ� ������
    [SerializeField] float ProximityAwarenessBuildRate = 1f; // ���� ������ ������

    [SerializeField] float AwarenessDecayDelay = 0.1f; // ������ ���� ���� �ð�
    [SerializeField] float AwarenessDecayRate = 0.1f; // ������ ������

    Dictionary<GameObject, TrackedTarget> Targets = new Dictionary<GameObject, TrackedTarget>(); // ���� ��� ���
    EnemyAI LinkedAI; // ����� �� AI

    public Dictionary<GameObject, TrackedTarget> ActiveTargets => Targets; // Ȱ��ȭ�� ���鿡 ���� ����

    // ���� �� ȣ��
    void Start()
    {
        LinkedAI = GetComponent<EnemyAI>(); // EnemyAI ������Ʈ ����
    }

    // �� ������ ������Ʈ
    void Update()
    {
        List<GameObject> toCleanup = new List<GameObject>(); // ������ ��� ���
        foreach (var targetGO in Targets.Keys)
        {
            if (Targets[targetGO].DecayAwareness(AwarenessDecayDelay, AwarenessDecayRate * Time.deltaTime))
            {
                if (Targets[targetGO].Awareness <= 0f) // �������� 0 ������ ���
                {
                    LinkedAI.OnFullyLost(); // ������ �������� ���� ���
                    toCleanup.Add(targetGO); // ���� ��Ͽ� �߰�
                }
                else
                {
                    // ������ ���ҿ� ���� ó��
                    if (Targets[targetGO].Awareness >= 1f)
                        LinkedAI.OnLostDetect(targetGO); // ������ �ҽǵ� ���
                    else
                        LinkedAI.OnLostSuspicion(); // �ǽ��� ���ҵ� ���
                }
            }
        }

        // �� �̻� �������� �ʴ� ���� ����
        foreach (var target in toCleanup)
            Targets.Remove(target);
    }

    private void CheckAndHandlePlayerDetection(GameObject targetGO, TrackedTarget trackedTarget)
    {
        if (targetGO.tag == "Player" && trackedTarget.Awareness >= 2f)
        {
            // �÷��̾��� Health ������Ʈ ã��
            Health playerHealth = targetGO.GetComponent<Health>();
            if (playerHealth != null)
            {
                // �÷��̾��� ü���� 0���� ����
                playerHealth.Damage(playerHealth.CurrentHP);
            }
        }
    }

    // ������ ������Ʈ
    void UpdateAwareness(GameObject targetGO, DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        if (!Targets.ContainsKey(targetGO)) // ����� ����Ʈ�� ���� ���
            Targets[targetGO] = new TrackedTarget(); // �� ��� �߰�

        // ��� ������ ������Ʈ
        if (Targets[targetGO].UpdateAwareness(target, position, awareness, minAwareness))
        {
            //CheckAndHandlePlayerDetection(targetGO, Targets[targetGO]);

            // �������� ���� ó��
            if (Targets[targetGO].Awareness >= 2f)
                LinkedAI.OnFullyDetected(targetGO); // ������ ������ ���
            else if (Targets[targetGO].Awareness >= 1f)
                LinkedAI.OnDetected(targetGO); // ������ ���
            else if (Targets[targetGO].Awareness >= 0f)
                LinkedAI.OnSuspicious(); // �ǽɵǴ� ���
        }
    }

    // �ð��� ���� ����
    public void ReportCanSee(DetectableTarget seen)
    {
        // ����� �þ� ���� ���� �ִ��� Ȯ��
        var vectorToTarget = (seen.transform.position - LinkedAI.EyeLocation).normalized;
        var dotProduct = Vector3.Dot(vectorToTarget, LinkedAI.EyeDirection);

        // ������ �⿩�� ���
        var awareness = VisionSensitivity.Evaluate(dotProduct) * VisionAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(seen.gameObject, seen, seen.transform.position, awareness, VisionMinimumAwareness);
    }

    // û���� ���� ����
    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        // û���� ������ ���
        var awareness = intensity * HearingAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(source, null, location, awareness, HearingMinimumAwareness);
    }

    // ���� ���� ����
    public void ReportInProximity(DetectableTarget target)
    {
        // ���� ������ ���
        var awareness = ProximityAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, ProximityMinimumAwareness);
    }
}