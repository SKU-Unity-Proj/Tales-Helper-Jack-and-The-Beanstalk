using UnityEngine;
using UnityEngine.Events;

public class LaserTarget : MonoBehaviour
{
    [Header("Gem Settings")]
    [SerializeField] private Color requiredColor; // 이 보석이 요구하는 레이저 색상
    [SerializeField] private Material activeMaterial; // 보석이 활성화될 때의 밝은 색상
    [SerializeField] private Material inactiveMaterial; // 보석이 비활성화될 때의 기본 색상
    [SerializeField] private MeshRenderer gemRenderer; // 보석의 MeshRenderer

    [Header("Gem Manager")]
    [SerializeField] private GemManager gemManager; // GemManager 참조

    private bool isActivated = false; // 보석이 활성화 상태인지 확인
    private LaserDevice currentLaser; // 현재 활성화한 레이저

    private void Update()
    {
        // 레이저가 감지되지 않으면 비활성화
        if (currentLaser == null || currentLaser.IsHittingTarget == false)
        {
            DeactivateGem();
        }
    }

    public void CheckLaserHit(LaserDevice laser)
    {
        Debug.Log($"Laser hitting {gameObject.name}: Laser Color = {laser.LaserColor}, Required Color = {requiredColor}");

        // 요구 색상 확인
        if (laser.LaserColor == requiredColor)
        {
            currentLaser = laser;
            gemRenderer.sharedMaterial = activeMaterial;

            if (!isActivated)
            {
                isActivated = true;
                Debug.Log($"{gameObject.name} has been activated!");

                // GemManager에 타겟 활성화 알림
                if (gemManager != null)
                {
                    gemManager.IncrementActiveTargets();
                }
            }
        }
    }

    private void DeactivateGem()
    {
        if (isActivated)
        {
            Debug.Log($"{gameObject.name} is already deactivated.");
            gemRenderer.sharedMaterial = inactiveMaterial; // 비활성화 상태로 변경
            return; // 이미 비활성화 상태라면 실행하지 않음
        }

        isActivated = false; // 활성화 상태 초기화
        currentLaser = null; // 현재 감지 중인 레이저 초기화
        gemRenderer.sharedMaterial = inactiveMaterial; // 비활성화 상태로 변경

        Debug.Log($"{gameObject.name} has been deactivated! Material set to {inactiveMaterial.name}");
    }
}
