using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuOption : MonoBehaviour
{
    public GameObject mainView;
    public GameObject optionView;
    public GameObject soundOption;
    public Slider bgmSlider;
    public Slider soundEffectSlider;
    public GameObject graphicOption;
    public AudioSource Bgm;
    public AudioSource SoundEffect;

    // 사운드 버튼
    public void OnClickSoundBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(true);
        graphicOption.SetActive(false);
        bgmSlider.onValueChanged.AddListener(OnBgmSliderValueChanged);
        soundEffectSlider.onValueChanged.AddListener(OnSoundEffectSliderValueChanged);
    }

    // 키 버튼
    public void OnClickKeyBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(false);
        graphicOption.SetActive(true);
    }

    // 백 버튼
    public void OnClickBackBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(false);
        graphicOption.SetActive(false);
        mainView.SetActive(true);
        optionView.SetActive(false);
    }

    // 사운드 조절
    public void OnBgmSliderValueChanged(float volume)
    {
        Bgm.volume = volume;
    }
    public void OnSoundEffectSliderValueChanged(float volume)
    {
        SoundEffect.volume = volume;
    }
}