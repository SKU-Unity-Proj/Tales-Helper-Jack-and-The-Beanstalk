using UnityEngine;
using System.Collections;

public class ValveController : MonoBehaviour
{
    public bool isUnlocked = false; // 벨브 잠금 상태
    public float rotationSpeed = 360f; // 벨브 회전 속도 (도/초)

    public Transform valveTransform; // 벨브 오브젝트의 Transform
    public WindowController linkedWindow; // 연결된 창문

    public SoundList valveSound, lockSound;

    public void Interact()
    {
        if (isUnlocked)
        {
            Debug.Log("이미 벨브가 열려 있습니다.");
            return;
        }

        if (linkedWindow != null && linkedWindow.IsMatched())
        {
            StartCoroutine(TurnValve());
        }
        else
        {
            Debug.Log("창문을 먼저 열어야 벨브를 닫을 수 있습니다.");
            SoundManager.Instance.PlayOneShotEffect((int)lockSound, transform.position, 1f);
        }
    }

    private IEnumerator TurnValve()
    {
        float totalRotation = 0f;

        SoundManager.Instance.PlayOneShotEffect((int)valveSound, transform.position, 1f);

        while (totalRotation < 720f) // 한 바퀴 회전 (360도)
        {
            float rotationStep = rotationSpeed * Time.deltaTime; // 프레임 기반 회전량 계산
            valveTransform.Rotate(0, -rotationStep, 0); // Z축 기준으로 회전
            totalRotation += rotationStep;
            yield return null;
        }

        // 회전 완료 후 Z축 값 정리
        valveTransform.localEulerAngles = new Vector3(
            valveTransform.localEulerAngles.x,
            valveTransform.localEulerAngles.y,
            Mathf.Round(valveTransform.localEulerAngles.z) // 회전값을 정확히 360도로 맞춤
        );

        Debug.Log("벨브를 성공적으로 회전시켰습니다.");
        isUnlocked = true; // 벨브를 잠금 해제 상태로 설정
    }
}
