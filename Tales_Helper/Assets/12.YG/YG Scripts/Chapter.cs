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
        SceneManager.LoadScene("JackHouse");
    }

    // 백 버튼
    public void OnClickBackBtn()
    {
        SoundEffect.Play();
        mainView.SetActive(true);
        chapterView.SetActive(false);
    }
}
