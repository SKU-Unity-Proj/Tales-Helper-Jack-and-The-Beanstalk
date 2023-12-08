using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainView;
    public GameObject optionView;
    public GameObject chapterView;

    public AudioSource BGM;
    public AudioSource SoundEffect;

    // 플레이 버튼
    public void OnClickPlayBtn()
    {
        Invoke("PlayPauseBtn", 0.2f);
    }
    void PlayPauseBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("JackHouse");
    }
    // 처음부터 시작 버튼
    public void OnClickRestartBtn()
    {
        Invoke("RestartPauseBtn", 0.2f);
    }
    void RestartPauseBtn()
    {
        SoundEffect.Play();
        Debug.Log("click btn");
    }
    
    // 챕터 버튼
    public void OnClickChapterBtn()
    {
        Invoke("ChapterPauseBtn", 0.2f);

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
        Invoke("OptionPauseBtn", 0.2f);
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
