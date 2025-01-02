using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Chapter : MonoBehaviour
{
    public GameObject mainView;
    public GameObject chapterView;
    public AudioSource SoundEffect;

    // 프롤로그 버튼
    public void OnClickPrologueBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("IntroAnimation");
    }

    // 튜토리얼 버튼
    public void OnClickTutorialBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("IntroAnimation");
    }

    // 백 버튼
    public void OnClickBackBtn()
    {
        BackPauseBtn();
        //Invoke("BackPauseBtn", 1.4f);
    }

    // ch1 버튼
    public void OnClickCH1Btn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("GaintMap");
    }

    // ch2 버튼
    public void OnClickCH2Btn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("GaintMap");
    }

    void BackPauseBtn()
    {
        SoundEffect.Play();
        mainView.SetActive(true);
        chapterView.SetActive(false);
    }
}
