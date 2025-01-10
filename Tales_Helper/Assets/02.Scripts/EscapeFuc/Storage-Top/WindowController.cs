using UnityEngine;

public class WindowController : MonoBehaviour
{
    public Transform currentSymbol; // ���� â�� ����(Transform)
    public Transform targetSymbol;  // ��ǥ ����(Transform)
    public Transform changeSymbol;  // �ٲ� ����(Transform)
    public Transform lightBeam;  // �ٲ� ����(Transform)

    public Animator windowAnimator; // â�� ���� �ִϸ��̼�

    public ValveController linkedValve; // ����� ����

    private bool isMatched = false; // ���� ��ġ ����

    public SoundList windowSound;

    public void RotateSymbol(float rotationAmount)
    {
        if (isMatched) return;

        // Z������ ���� ȸ��
        currentSymbol.Rotate(0, 0, rotationAmount);

        // ȸ�� �� ���� ��ġ ���� Ȯ��
        CheckMatch();
    }

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

    private void UnlockWindow()
    {
        SoundManager.Instance.PlayOneShotEffect((int)windowSound, transform.position, 1f);

        targetSymbol.gameObject.SetActive(false);
        changeSymbol.gameObject.SetActive(true);

        windowAnimator.SetTrigger("Open");
        Debug.Log("������ ��ġ�Ͽ� â���� �����ϴ�.");
        //linkedValve.isUnlocked = true; // ����� ���긦 ��� ����

        lightBeam.gameObject.SetActive(true);
    }

    public bool IsMatched()
    {
        return isMatched;
    }
}
