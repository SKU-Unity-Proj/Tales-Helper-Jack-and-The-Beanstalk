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

    // 플레이 버튼
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
    // 처음부터 시작 버튼
    public void OnClickRestartBtn()
    {
        RestartPauseBtn();
        //Invoke("RestartPauseBtn", 0.2f);
    }
    void RestartPauseBtn()
    {
        LoadingSceneController.Instance.LoadScene("IntroAnimation");
    }
    
    // 챕터 버튼
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

    // 옵션 버튼
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
    
    // 종료 버튼
    public void OnClickQuitBtn()
    {
        SoundEffect.Play();
        Debug.Log("게임 종료");
        Application.Quit();
    }

}
