using UnityEngine;
using UnityEngine.UI;

public class ChapterMenu : MonoBehaviour
{
    public Button[] chapterButton = new Button[5];
    private float unlockedAlpha = 1.0f; // é�Ͱ� �����Ǿ��� ���� ���� (1.0 = ������)
    private float lockedAlpha = 0.75f;   // é�Ͱ� ����� ���� ���� (0.0 = ������ ����)

    private void OnEnable()
    {
        ChapterManager.Instance.LoadGameData();
        var data = ChapterManager.Instance.data;

        for (int i = 0; i < chapterButton.Length; i++)
        {
            if (data.isChapterCleared[i])
            {
                // é�Ͱ� ������ ���: ��� + ���� ����
                SetButtonColorAndTransparency(chapterButton[i], Color.white, unlockedAlpha);
                chapterButton[i].enabled = true;
            }
            else
            {
                // é�Ͱ� ��� ���: ������ + ���� ����
                SetButtonColorAndTransparency(chapterButton[i], Color.black, lockedAlpha);
                chapterButton[i].enabled = false;
            }
        }
    }

    private void SetButtonColorAndTransparency(Button button, Color baseColor, float alpha)
    {
        // ��ư�� �̹��� ���� ����
        if (button != null && button.image != null)
        {
            baseColor.a = Mathf.Clamp(alpha, 0f, 1f); // ���� ���� 0~1 ������ ����� �ʵ��� Ŭ����
            button.image.color = baseColor;
        }
    }
}
