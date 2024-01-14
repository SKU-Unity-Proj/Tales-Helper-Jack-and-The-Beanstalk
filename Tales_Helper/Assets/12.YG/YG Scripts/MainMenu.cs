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

    public GameObject graphicOption;

    public AudioSource BGM;
    public AudioSource SoundEffect;

    // �÷��� ��ư
    public void OnClickPlayBtn()
    {
        Invoke("PlayPauseBtn", 0.2f);
    }
    void PlayPauseBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("IntroAnimation");
    }
    // ó������ ���� ��ư
    public void OnClickRestartBtn()
    {
        Invoke("RestartPauseBtn", 0.2f);
    }
    void RestartPauseBtn()
    {
        SoundEffect.Play();
        Debug.Log("click btn");
    }
    
    // é�� ��ư
    public void OnClickChapterBtn()
    {
        Invoke("ChapterPauseBtn", 1.4f);

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
        Invoke("OptionPauseBtn", 1.4f);
    }
    void OptionPauseBtn()
    {
        SoundEffect.Play();
        mainView.SetActive(false);
        chapterView.SetActive(false);
        optionView.SetActive(true);
        graphicOption.SetActive(false);
    }
    
    // ���� ��ư
    public void OnClickQuitBtn()
    {
        SoundEffect.Play();
        Debug.Log("���� ����");
        Application.Quit();
    }

}
