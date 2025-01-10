using UnityEngine;

public class WindowController : MonoBehaviour
{
    public Transform currentSymbol; // 현재 창문 문양(Transform)
    public Transform targetSymbol;  // 목표 문양(Transform)
    public Transform changeSymbol;  // 바뀔 문양(Transform)
    public Transform lightBeam;  // 바뀔 문양(Transform)

    public Animator windowAnimator; // 창문 열림 애니메이션

    public ValveController linkedValve; // 연결된 벨브

    private bool isMatched = false; // 문양 일치 여부

    public SoundList windowSound;

    public void RotateSymbol(float rotationAmount)
    {
        if (isMatched) return;

        // Z축으로 문양 회전
        currentSymbol.Rotate(0, 0, rotationAmount);

        // 회전 후 문양 일치 여부 확인
        CheckMatch();
    }

    private void CheckMatch()
    {
        // Z축의 회전 각도를 비교하여 문양 일치 여부 판단
        float currentZRotation = currentSymbol.localEulerAngles.z;
        float targetZRotation = targetSymbol.localEulerAngles.z;

        // 오차 허용 범위 설정 (예: 1도 이하)
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
        Debug.Log("문양이 일치하여 창문이 열립니다.");
        //linkedValve.isUnlocked = true; // 연결된 벨브를 잠금 해제

        lightBeam.gameObject.SetActive(true);
    }

    public bool IsMatched()
    {
        return isMatched;
    }
}
