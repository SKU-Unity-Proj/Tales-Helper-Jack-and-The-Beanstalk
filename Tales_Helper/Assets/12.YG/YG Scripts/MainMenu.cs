using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainView;
    public GameObject optionView;
    public GameObject chapterView;

    public GameObject graphicOption;

    public AudioSource BGM;
    public AudioSource SoundEffect;

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // �÷��� ��ư
    public void OnClickPlayBtn()
    {
        PlayPauseBtn();
        //Invoke("PlayPauseBtn", 0.2f);
    }
    void PlayPauseBtn()
    {
        SoundEffect.Play();
        LoadingSceneController.Instance.LoadScene("IntroAnimation");
    }
    // ó������ ���� ��ư
    public void OnClickRestartBtn()
    {
        RestartPauseBtn();
        //Invoke("RestartPauseBtn", 0.2f);
    }
    void RestartPauseBtn()
    {
        LoadingSceneController.Instance.LoadScene("IntroAnimation");
    }
    
    // é�� ��ư
    public void OnClickChapterBtn()
    {
        ChapterPauseBtn();
        //Invoke("ChapterPauseBtn", 1.4f);
    }
    void ChapterPauseBtn()
    {
        SoundEffect.Play();
        mainView.SetActive(false);
        chapterView.SetActive(true);
        optionView.SetActive(false);
    }

    // �ɼ� ��ư
    public void OnClickOptionBtn()
    {
        OptionPauseBtn();
        //Invoke("OptionPauseBtn", 1.4f);
    }
    void OptionPauseBtn()
    {
        SoundEffect.Play();
        mainView.SetActive(false);
        chapterView.SetActive(false);
        optionView.SetActive(true);
    }
    
    // ���� ��ư
    public void OnClickQuitBtn()
    {
        SoundEffect.Play();
        Debug.Log("���� ����");
        Application.Quit();
    }

}
