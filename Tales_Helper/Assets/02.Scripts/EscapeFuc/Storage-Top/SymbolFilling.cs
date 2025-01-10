using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� �ٷ�� ���� �ʿ�

public class SymbolFilling : MonoBehaviour
{
    public Image targetImage; // ��� UI �̹��� (CanvasRenderer ���)
    private Material materialInstance; // UI Material �ν��Ͻ�
    private float fillProgress = 0f; // ���� Fill Progress

    void Start()
    {
        // �̹����� Material �ν��Ͻ��� ������
        materialInstance = targetImage.material;

        // �ʱ� Fill Progress ������Ʈ
        UpdateFillProgress();
    }

    public void AddProgress(float amount)
    {
        // Fill Progress ����
        fillProgress += amount;
        fillProgress = Mathf.Clamp01(fillProgress); // 0~1�� ����

        // Material�� Fill Progress �ݿ�
        UpdateFillProgress();
    }

    private void UpdateFillProgress()
    {
        if (materialInstance != null)
        {
            // Shader�� _FillProgress ���� ������Ʈ
            materialInstance.SetFloat("_FillProgress", fillProgress);

            // ����� ���
            Debug.Log($"���� Fill Progress: {fillProgress}");
        }
        else
        {
            Debug.LogError("Material Instance�� �������� �ʾҽ��ϴ�!");
        }

        // Fill Progress�� 1�� �����ϸ� �Ϸ� ���
        if (fillProgress >= 1f)
        {
            Debug.Log("�� ������ ������ ä�������ϴ�!");
        }
    }
}
