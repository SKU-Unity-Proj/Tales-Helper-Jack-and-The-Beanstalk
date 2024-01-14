using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

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

    [SerializeField]
    List<RenderPipelineAsset> RenderPipelineAssets;
    [SerializeField]
    TMP_Dropdown Dropdown;

    // ���� ��ư
    public void OnClickSoundBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(true);
        graphicOption.SetActive(false);
        bgmSlider.onValueChanged.AddListener(OnBgmSliderValueChanged);
        soundEffectSlider.onValueChanged.AddListener(OnSoundEffectSliderValueChanged);
    }

    // Ű ��ư
    public void OnClickKeyBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(false);
        graphicOption.SetActive(true);
    }

    // �� ��ư
    public void OnClickBackBtn()
    {
        Invoke("BackPuaseBtn", 1.4f);
    }

    // ���� ����
    public void OnBgmSliderValueChanged(float volume)
    {
        Bgm.volume = volume;
    }
    public void OnSoundEffectSliderValueChanged(float volume)
    {
        SoundEffect.volume = volume;
    }

    // �׷��� ����
    public void SetPipeline(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = RenderPipelineAssets[value];
    }

    void BackPuaseBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(false);
        graphicOption.SetActive(false);
        mainView.SetActive(true);
        optionView.SetActive(false);
    }
}