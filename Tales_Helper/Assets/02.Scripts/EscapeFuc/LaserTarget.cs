using UnityEngine;
using UnityEngine.Events;

public class LaserTarget : MonoBehaviour
{
    [Header("Gem Settings")]
    [SerializeField] private Color requiredColor; // �� ������ �䱸�ϴ� ������ ����
    [SerializeField] private Material activeMaterial; // ������ Ȱ��ȭ�� ���� ���� ����
    [SerializeField] private Material inactiveMaterial; // ������ ��Ȱ��ȭ�� ���� �⺻ ����
    [SerializeField] private MeshRenderer gemRenderer; // ������ MeshRenderer

    [Header("Gem Manager")]
    [SerializeField] private GemManager gemManager; // GemManager ����

    private bool isActivated = false; // ������ Ȱ��ȭ �������� Ȯ��
    private LaserDevice currentLaser; // ���� Ȱ��ȭ�� ������

    private void Update()
    {
        // �������� �������� ������ ��Ȱ��ȭ
        if (currentLaser == null || currentLaser.IsHittingTarget == false)
        {
            DeactivateGem();
        }
    }

    public void CheckLaserHit(LaserDevice laser)
    {
        Debug.Log($"Laser hitting {gameObject.name}: Laser Color = {laser.LaserColor}, Required Color = {requiredColor}");

        // �䱸 ���� Ȯ��
        if (laser.LaserColor == requiredColor)
        {
            currentLaser = laser;
            gemRenderer.sharedMaterial = activeMaterial;

            if (!isActivated)
            {
                isActivated = true;
                Debug.Log($"{gameObject.name} has been activated!");

                // GemManager�� Ÿ�� Ȱ��ȭ �˸�
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
            gemRenderer.sharedMaterial = inactiveMaterial; // ��Ȱ��ȭ ���·� ����
            return; // �̹� ��Ȱ��ȭ ���¶�� �������� ����
        }

        isActivated = false; // Ȱ��ȭ ���� �ʱ�ȭ
        currentLaser = null; // ���� ���� ���� ������ �ʱ�ȭ
        gemRenderer.sharedMaterial = inactiveMaterial; // ��Ȱ��ȭ ���·� ����

        Debug.Log($"{gameObject.name} has been deactivated! Material set to {inactiveMaterial.name}");
    }
}
