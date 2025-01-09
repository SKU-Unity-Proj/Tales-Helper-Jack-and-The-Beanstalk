using UnityEngine;
using UnityEngine.Events;

public class LaserTarget : MonoBehaviour
{
    [Header("Gem Settings")]
    [SerializeField] private Color requiredColor; // 이 보석이 요구하는 레이저 색상
    [SerializeField] private Material activeMaterial; // 보석이 활성화될 때의 밝은 색상
    [SerializeField] private MeshRenderer gemRenderer; // 보석의 MeshRenderer

    private bool isActivated = false; // 보석이 활성화되었는지 상태

    public bool IsActivated => isActivated; // 외부에서 활성화 여부 확인 가능

    public void CheckLaserHit(LaserDevice laser)
    {
        if (isActivated) return; // 이미 활성화된 경우 무시

        Debug.Log($"Checking Laser Hit: Laser Color = {laser.LaserColor}, Required Color = {requiredColor}");

        if (laser.LaserColor == requiredColor)
        {
            ActivateGem(); // 색상이 일치하면 활성화
        }
    }

    private void ActivateGem()
    {
        isActivated = true;
        gemRenderer.material = activeMaterial; // 활성화된 상태로 밝은 색상 변경
        Debug.Log($"{gameObject.name} has been activated!");
    }
}
