using UnityEngine;
using UnityEngine.SceneManagement;

public class Chapter : MonoBehaviour
{
    public GameObject mainView;
    public GameObject chapterView;
    public AudioSource SoundEffect;

    private void OnEnable()
    {
        ChapterManager.Instance.LoadGameData();
    }

    #region button
    // ���ѷα� ��ư
    public void OnClickJackHouseBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("IntroAnimation");
    }

    // Ʃ�丮�� ��ư
    public void OnClickCh0Btn()
    {
        SoundEffect.Play();
        ChapterManager.Instance.SelectChapter = 0;
        SceneManager.LoadScene(4);
    }

    // ch1 ��ư
    public void OnClickCH1Btn()
    {
        SoundEffect.Play();
        ChapterManager.Instance.SelectChapter = 1;
        SceneManager.LoadScene(4);
    }

    // ch2 ��ư
    public void OnClickCH2Btn()
    {
        SoundEffect.Play();
        ChapterManager.Instance.SelectChapter = 2;
        SceneManager.LoadScene(4);
    }

    public void OnClickBackBtn()
    {
        BackPauseBtn();
        //Invoke("BackPauseBtn", 1.4f);
    }
    void BackPauseBtn()
    {
        SoundEffect.Play();
        mainView.SetActive(true);
        chapterView.SetActive(false);
    }
    #endregion

    void LockChapter(int num)
    {
        for (int i = num + 1; i < 5; i++)
        {
            ChapterManager.Instance.LockChapter(i);
        }
    }
}
