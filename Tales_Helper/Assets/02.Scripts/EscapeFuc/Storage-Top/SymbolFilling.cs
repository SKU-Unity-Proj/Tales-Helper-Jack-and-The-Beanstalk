using UnityEngine;
using UnityEngine.UI; // UI 요소를 다루기 위해 필요

public class SymbolFilling : MonoBehaviour
{
    public Image targetImage; // 대상 UI 이미지 (CanvasRenderer 기반)
    private Material materialInstance; // UI Material 인스턴스
    private float fillProgress = 0f; // 현재 Fill Progress

    void Start()
    {
        // 이미지의 Material 인스턴스를 가져옴
        materialInstance = targetImage.material;

        // 초기 Fill Progress 업데이트
        UpdateFillProgress();
    }

    public void AddProgress(float amount)
    {
        // Fill Progress 증가
        fillProgress += amount;
        fillProgress = Mathf.Clamp01(fillProgress); // 0~1로 제한

        // Material에 Fill Progress 반영
        UpdateFillProgress();
    }

    private void UpdateFillProgress()
    {
        if (materialInstance != null)
        {
            // Shader의 _FillProgress 값을 업데이트
            materialInstance.SetFloat("_FillProgress", fillProgress);

            // 디버그 출력
            Debug.Log($"현재 Fill Progress: {fillProgress}");
        }
        else
        {
            Debug.LogError("Material Instance가 설정되지 않았습니다!");
        }

        // Fill Progress가 1에 도달하면 완료 출력
        if (fillProgress >= 1f)
        {
            Debug.Log("해 문양이 완전히 채워졌습니다!");
        }
    }
}
