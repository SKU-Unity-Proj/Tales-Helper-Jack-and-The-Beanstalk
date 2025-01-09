using UnityEngine;
using System.Collections;

public class GemManager : MonoBehaviour
{
    [Header("Gem Activation Settings")]
    [SerializeField] private int totalTargets = 3; // �ʿ��� �� Ÿ�� ����
    private int activeTargets = 0; // ���� Ȱ��ȭ�� Ÿ�� ����

    [Header("Motor Settings")]
    [SerializeField] private Animator motorAnimator; // ������ �ִϸ�����
    [SerializeField] private float maxMotorSpeed = 2f; // ������ �ִ� �ӵ�
    [SerializeField] private float speedIncreaseStep = 0.67f; // �ӵ� ������ (maxMotorSpeed / totalTargets)

    [Header("Motor Settings")]
    [SerializeField] private AudioSource motorSound; // ���� ����
    [SerializeField] private float[] motorVolumes = { 0.05f, 0.15f, 0.3f }; // �� Ÿ�� Ȱ��ȭ�� ���� ���� ��

    [Header("Ladder Settings")]
    [SerializeField] private GameObject ladder; // ��ٸ� ������Ʈ
    [SerializeField] private Vector3 ladderDropPosition = new Vector3(2.57999992f, 8.64000034f, -103.75f); // ��ٸ��� ������ ��ġ
    [SerializeField] private float ladderDropSpeed = 5f; // ��ٸ��� �������� �ӵ�

    [Header("Effects")]
    [SerializeField] private AudioSource dropSound; // �� �Ҹ� ȿ����
    [SerializeField] private GameObject dustEffectPrefab; // ��� ���� ȿ��

    private bool isLadderDropped = false; // ��ٸ��� �̹� ���������� Ȯ��

    public void IncrementActiveTargets()
    {
        if (isLadderDropped) return; // �̹� ��ٸ��� �������ٸ� �������� ����

        activeTargets++;
        Debug.Log($"Active Targets: {activeTargets}/{totalTargets}");

        // ���� �ӵ� �� ���� ������Ʈ
        UpdateGearSpeed();
        UpdateMotorSound();

        // ��� Ÿ���� Ȱ��ȭ�Ǹ� ��ٸ� ����߸���
        if (activeTargets >= totalTargets)
        {
            DropLadder();
        }
    }

    private void UpdateGearSpeed()
    {
        // Ȱ��ȭ�� Ÿ�� ���� ���� ���� �ӵ� ����
        float currentSpeed = Mathf.Clamp(activeTargets * speedIncreaseStep, 0, maxMotorSpeed);

        // Ư�� ����(Gear)�� Speed �Ķ���� ����
        if (motorAnimator != null)
        {
            motorAnimator.SetFloat("Speed", currentSpeed); // Gear ������ �ӵ��� ����
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

        // Ÿ�� ������ ���� ���� ����
        if (activeTargets > 0 && activeTargets <= motorVolumes.Length)
        {
            motorSound.volume = motorVolumes[activeTargets - 1];
            if (!motorSound.isPlaying)
            {
                motorSound.loop = true; // �ݺ� ��� Ȱ��ȭ
                motorSound.Play(); // ���� ����
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

        // ���� ����
        if (motorSound != null)
        {
            motorSound.Stop();
            motorAnimator.SetFloat("Speed", 0f); // Gear ������ �ӵ��� ����
        }
    }

    private IEnumerator DropLadderRoutine()
    {
        Vector3 startPosition = ladder.transform.position; // ���� ��ġ
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y - 5f, startPosition.z); // Y�� -5 �̵�

        // ��ٸ��� ������ ����߸���
        float time = 0;
        while (Vector3.Distance(ladder.transform.position, endPosition) > 0.01f)
        {
            ladder.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            time += Time.deltaTime * ladderDropSpeed;
            yield return null;
        }

        ladder.transform.position = endPosition; // ��Ȯ�� ��ǥ ��ġ�� ����

        // ���� ����Ʈ ���
        PlayDustEffect();

        // ȿ�� ���
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

        // ���� ����Ʈ�� ��ٸ��� ��ġ�� ����
        Instantiate(dustEffectPrefab, ladder.transform.position, Quaternion.identity);
        Debug.Log("Dust effect instantiated at ladder position.");
    }
}
