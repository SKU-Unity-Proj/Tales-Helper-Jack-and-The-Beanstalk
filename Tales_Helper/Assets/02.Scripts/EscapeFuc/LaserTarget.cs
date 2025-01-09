using UnityEngine;
using UnityEngine.Events;

public class LaserTarget : MonoBehaviour
{
    [Header("Gem Settings")]
    [SerializeField] private Color requiredColor; // �� ������ �䱸�ϴ� ������ ����
    [SerializeField] private Material activeMaterial; // ������ Ȱ��ȭ�� ���� ���� ����
    [SerializeField] private MeshRenderer gemRenderer; // ������ MeshRenderer

    private bool isActivated = false; // ������ Ȱ��ȭ�Ǿ����� ����

    public bool IsActivated => isActivated; // �ܺο��� Ȱ��ȭ ���� Ȯ�� ����

    public void CheckLaserHit(LaserDevice laser)
    {
        if (isActivated) return; // �̹� Ȱ��ȭ�� ��� ����

        Debug.Log($"Checking Laser Hit: Laser Color = {laser.LaserColor}, Required Color = {requiredColor}");

        if (laser.LaserColor == requiredColor)
        {
            ActivateGem(); // ������ ��ġ�ϸ� Ȱ��ȭ
        }
    }

    private void ActivateGem()
    {
        isActivated = true;
        gemRenderer.material = activeMaterial; // Ȱ��ȭ�� ���·� ���� ���� ����
        Debug.Log($"{gameObject.name} has been activated!");
    }
}
