using UnityEngine;

public class WindowController : MonoBehaviour
{
    // === ���� ���� ===
    [Header("Symbol Settings")]
    public Transform currentSymbol; // ���� â�� ����(Transform)
    public Transform targetSymbol;  // ��ǥ ����(Transform)
    public Transform changeSymbol;  // �ٲ� ����(Transform)

    // === �� ���� ===
    [Header("Light Settings")]
    public Transform lightBeam;  // �� ������Ʈ(Ȱ��ȭ ���� ����)
    public Transform sunTarget; // �� ������ ��ġ
    public LineRenderer lineRenderer; // ���η�����
    public LayerMask hitMask; // �� ���� ���̾� ����ũ
    public SymbolFilling symbolFilling; // Fill Progress�� �����ϴ� ��ũ��Ʈ

    // === â�� �ִϸ��̼� �� ���� ===
    [Header("Window Settings")]
    public Animator windowAnimator; // â�� ���� �ִϸ��̼�
    public ValveController linkedValve; // ����� ����

    // === ���� ���� ===
    [Header("Sound Settings")]
    public SoundList windowSound; // â�� ���� ����

    private bool isMatched = false; // ���� ��ġ ����

    // === ���� ȸ�� ===
    public void RotateSymbol(float rotationAmount)
    {
        if (isMatched) return;

        // Z������ ���� ȸ��
        currentSymbol.Rotate(0, 0, rotationAmount);

        // ȸ�� �� ���� ��ġ ���� Ȯ��
        CheckMatch();
    }

    // === ���� ��ġ Ȯ�� ===
    private void CheckMatch()
    {
        // Z���� ȸ�� ������ ���Ͽ� ���� ��ġ ���� �Ǵ�
        float currentZRotation = currentSymbol.localEulerAngles.z;
        float targetZRotation = targetSymbol.localEulerAngles.z;

        // ���� ��� ���� ���� (��: 1�� ����)
        float rotationThreshold = 1f;

        if (Mathf.Abs(Mathf.DeltaAngle(currentZRotation, targetZRotation)) <= rotationThreshold)
        {
            isMatched = true;
            UnlockWindow();
        }
    }

    // === â�� ���� ===
    private void UnlockWindow()
    {
        // ���� ���
        SoundManager.Instance.PlayOneShotEffect((int)windowSound, transform.position, 1f);

        // ���� ��ü
        targetSymbol.gameObject.SetActive(false);
        changeSymbol.gameObject.SetActive(true);

        // â�� ���� �ִϸ��̼� ����
        windowAnimator.SetTrigger("Open");
        Debug.Log("������ ��ġ�Ͽ� â���� �����ϴ�.");

        // �� Ȱ��ȭ
        lightBeam.gameObject.SetActive(true);

        // LineRenderer�� �� ���
        StartLightBeam();
    }

    // === �� ��� ===
    private void StartLightBeam()
    {
        // ���η����� Ȱ��ȭ
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;

        // ���η������� �������� ���� ����
        lineRenderer.SetPosition(0, lightBeam.position);
        lineRenderer.SetPosition(1, sunTarget.position);

        // ���� �� ���翡 �����ߴ��� Ȯ��
        RaycastHit hit;
        if (Physics.Raycast(lightBeam.position, (sunTarget.position - lightBeam.position).normalized, out hit, Mathf.Infinity, hitMask))
        {
            if (hit.collider.CompareTag("SunSymbol"))
            {
                // �� ���翡 Fill Progress ����
                symbolFilling.AddProgress(0.15f);
                Debug.Log("���� �� ���翡 �����߽��ϴ�.");
            }
        }
    }

    // === ���� ��ġ ���� Ȯ�� ===
    public bool IsMatched()
    {
        return isMatched;
    }
}
