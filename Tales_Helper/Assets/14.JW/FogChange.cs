using UnityEngine;
using System.Collections;

public class FogChange: MonoBehaviour
{
    // 충돌 시 Fog로 사용할 값
    public float fogDensityOnCollision = 0.03f;
    // Fog 변경에 걸리는 시간
    public float fogChangeDuration = 2.0f;

    private float originalFogDensity;
    private bool fogChanging = false;

    void Start()
    {
        // 시작할 때 원래의 Fog 값을 저장
        originalFogDensity = RenderSettings.fogDensity;
    }

    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 태그가 "Fog"인지 확인하고, 변경 중이 아닌 경우에만 처리
        if (other.CompareTag("Fog") && !fogChanging)
        {
            // 충돌 시 Fog 값을 서서히 변경
            StartCoroutine(ChangeFog());
            // 충돌한 "Fog" 태그를 가진 오브젝트를 비활성화
            other.gameObject.SetActive(false);
        }
    }

    IEnumerator ChangeFog()
    {
        fogChanging = true; // Fog 변경 중 플래그 활성화

        float elapsedTime = 0f;

        while (elapsedTime < fogChangeDuration)
        {
            // 현재 Fog 값을 서서히 변경
            RenderSettings.fogDensity = Mathf.Lerp(originalFogDensity, fogDensityOnCollision, elapsedTime / fogChangeDuration);

            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 변경이 완료된 후 최종 목표값으로 설정
        RenderSettings.fogDensity = fogDensityOnCollision;

        fogChanging = false; // Fog 변경 중 플래그 비활성화
    }
}