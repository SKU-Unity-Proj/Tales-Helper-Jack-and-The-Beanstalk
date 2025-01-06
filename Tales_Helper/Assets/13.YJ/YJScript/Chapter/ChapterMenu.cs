using UnityEngine;
using UnityEngine.UI;

public class ChapterMenu : MonoBehaviour
{
    public Button[] chapterButton = new Button[5];
    private float unlockedAlpha = 1.0f; // 챕터가 해제되었을 때의 투명도 (1.0 = 불투명)
    private float lockedAlpha = 0.75f;   // 챕터가 잠겼을 때의 투명도 (0.0 = 완전히 투명)

    private void OnEnable()
    {
        ChapterManager.Instance.LoadGameData();
        var data = ChapterManager.Instance.data;

        for (int i = 0; i < chapterButton.Length; i++)
        {
            if (data.isChapterCleared[i])
            {
                // 챕터가 해제된 경우: 흰색 + 투명도 적용
                SetButtonColorAndTransparency(chapterButton[i], Color.white, unlockedAlpha);
                chapterButton[i].enabled = true;
            }
            else
            {
                // 챕터가 잠긴 경우: 검정색 + 투명도 적용
                SetButtonColorAndTransparency(chapterButton[i], Color.black, lockedAlpha);
                chapterButton[i].enabled = false;
            }
        }
    }

    private void SetButtonColorAndTransparency(Button button, Color baseColor, float alpha)
    {
        // 버튼의 이미지 색상 변경
        if (button != null && button.image != null)
        {
            baseColor.a = Mathf.Clamp(alpha, 0f, 1f); // 알파 값이 0~1 범위를 벗어나지 않도록 클램프
            button.image.color = baseColor;
        }
    }
}
