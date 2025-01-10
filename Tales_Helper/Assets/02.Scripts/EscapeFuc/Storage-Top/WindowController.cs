using UnityEngine;

public class WindowController : MonoBehaviour
{
    // === 문양 관련 ===
    [Header("Symbol Settings")]
    public Transform currentSymbol; // 현재 창문 문양(Transform)
    public Transform targetSymbol;  // 목표 문양(Transform)
    public Transform changeSymbol;  // 바뀔 문양(Transform)

    // === 빛 관련 ===
    [Header("Light Settings")]
    public Transform lightBeam;  // 빛 오브젝트(활성화 여부 관리)
    public Transform sunTarget; // 해 문양의 위치
    public LineRenderer lineRenderer; // 라인랜더러
    public LayerMask hitMask; // 해 문양 레이어 마스크
    public SymbolFilling symbolFilling; // Fill Progress를 제어하는 스크립트

    // === 창문 애니메이션 및 벨브 ===
    [Header("Window Settings")]
    public Animator windowAnimator; // 창문 열림 애니메이션
    public ValveController linkedValve; // 연결된 벨브

    // === 사운드 관련 ===
    [Header("Sound Settings")]
    public SoundList windowSound; // 창문 열림 사운드

    private bool isMatched = false; // 문양 일치 여부

    // === 문양 회전 ===
    public void RotateSymbol(float rotationAmount)
    {
        if (isMatched) return;

        // Z축으로 문양 회전
        currentSymbol.Rotate(0, 0, rotationAmount);

        // 회전 후 문양 일치 여부 확인
        CheckMatch();
    }

    // === 문양 일치 확인 ===
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

    // === 창문 열림 ===
    private void UnlockWindow()
    {
        // 사운드 재생
        SoundManager.Instance.PlayOneShotEffect((int)windowSound, transform.position, 1f);

        // 문양 교체
        targetSymbol.gameObject.SetActive(false);
        changeSymbol.gameObject.SetActive(true);

        // 창문 열림 애니메이션 실행
        windowAnimator.SetTrigger("Open");
        Debug.Log("문양이 일치하여 창문이 열립니다.");

        // 빛 활성화
        lightBeam.gameObject.SetActive(true);

        // LineRenderer로 빛 쏘기
        StartLightBeam();
    }

    // === 빛 쏘기 ===
    private void StartLightBeam()
    {
        // 라인랜더러 활성화
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;

        // 라인랜더러의 시작점과 끝점 설정
        lineRenderer.SetPosition(0, lightBeam.position);
        lineRenderer.SetPosition(1, sunTarget.position);

        // 빛이 해 문양에 도달했는지 확인
        RaycastHit hit;
        if (Physics.Raycast(lightBeam.position, (sunTarget.position - lightBeam.position).normalized, out hit, Mathf.Infinity, hitMask))
        {
            if (hit.collider.CompareTag("SunSymbol"))
            {
                // 해 문양에 Fill Progress 증가
                symbolFilling.AddProgress(0.15f);
                Debug.Log("빛이 해 문양에 도달했습니다.");
            }
        }
    }

    // === 문양 일치 여부 확인 ===
    public bool IsMatched()
    {
        return isMatched;
    }
}
