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

    
    void Start()
    {
        // 드롭다운의 기본 선택값을 설정합니다.
        Dropdown.value = 0;
        Dropdown.RefreshShownValue(); // UI를 업데이트합니다.

        // 기본 렌더링 파이프라인을 설정합니다.
        SetPipeline(Dropdown.value);
    }

    // 사운드 버튼
    public void OnClickSoundBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(true);
        graphicOption.SetActive(false);
        bgmSlider.onValueChanged.AddListener(OnBgmSliderValueChanged);
        soundEffectSlider.onValueChanged.AddListener(OnSoundEffectSliderValueChanged);
    }

    // 그래픽 버튼
    public void OnClickKeyBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(false);
        graphicOption.SetActive(true);
    }

    // 백 버튼
    public void OnClickBackBtn()
    {
        BackPuaseBtn();
        //Invoke("BackPuaseBtn", 1.4f);
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

    // 그래픽 설정
    public void SetPipeline(int value)
    {
        Debug.Log("Selected pipeline index: " + value);
        QualitySettings.SetQualityLevel(value, true);
        QualitySettings.renderPipeline = RenderPipelineAssets[value];
        Debug.Log("Current Quality Level: " + QualitySettings.GetQualityLevel());
        Debug.Log("Current Render Pipeline: " + QualitySettings.renderPipeline);
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