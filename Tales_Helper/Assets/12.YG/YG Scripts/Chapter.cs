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
        SceneManager.LoadScene("JackHouse");
    }

    // �� ��ư
    public void OnClickBackBtn()
    {
        SoundEffect.Play();
        mainView.SetActive(true);
        chapterView.SetActive(false);
    }
}
