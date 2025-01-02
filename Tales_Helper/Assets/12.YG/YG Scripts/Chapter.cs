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

    // ���ѷα� ��ư
    public void OnClickPrologueBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("IntroAnimation");
    }

    // Ʃ�丮�� ��ư
    public void OnClickTutorialBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("IntroAnimation");
    }

    // �� ��ư
    public void OnClickBackBtn()
    {
        BackPauseBtn();
        //Invoke("BackPauseBtn", 1.4f);
    }

    // ch1 ��ư
    public void OnClickCH1Btn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("GaintMap");
    }

    // ch2 ��ư
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
