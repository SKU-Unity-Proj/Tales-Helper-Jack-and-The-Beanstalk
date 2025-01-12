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
        // ��Ӵٿ��� �⺻ ���ð��� �����մϴ�.
        Dropdown.value = 0;
        Dropdown.RefreshShownValue(); // UI�� ������Ʈ�մϴ�.

        // �⺻ ������ ������������ �����մϴ�.
        SetPipeline(0);

        //LoadGraphicsSettings();
        LoadResolutionSettings();
        LoadAudioSettings();
    }

    // ���� ��ư
    public void OnClickSoundBtn()
    {
        bgmSlider.onValueChanged.AddListener(OnBgmSliderValueChanged);
        soundEffectSlider.onValueChanged.AddListener(OnSoundEffectSliderValueChanged);
    }

    // �� ��ư
    public void OnClickBackBtn()
    {
        BackPuaseBtn();
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
        QualitySettings.SetQualityLevel(value, true);
        QualitySettings.renderPipeline = RenderPipelineAssets[value];

        Debug.Log("Selected pipeline index: " + value);
        Debug.Log("Current Quality Level: " + QualitySettings.GetQualityLevel());
        Debug.Log("Current Render Pipeline: " + QualitySettings.renderPipeline);
    }

    void BackPuaseBtn()
    {
        SoundEffect.Play();
        mainView.SetActive(true);
        optionView.SetActive(false);
    }

    /// <summary>
    /// GPT �׷��� ����
    /// </summary>

    [Header("Graphics Settings")]
    public TMP_Dropdown graphicsDropdown;

    [Header("Resolution Settings")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    [Header("Audio Settings")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    #region Graphics
    void LoadGraphicsSettings()
    {
        graphicsDropdown.ClearOptions();
        List<string> options = new List<string>() { "Low", "Medium", "High", "Ultra" };
        graphicsDropdown.AddOptions(options);

        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 4);
        graphicsDropdown.value = savedQuality;
        graphicsDropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(savedQuality);

        graphicsDropdown.onValueChanged.AddListener(SetGraphicsQuality);
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
    }
    #endregion

    #region Resolution
    void LoadResolutionSettings()
    {
        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }
    #endregion

    #region Audio
    void LoadAudioSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
        volumeSlider.value = savedVolume;
        volumeText.text = Mathf.RoundToInt(savedVolume * 100) + "%";

        AudioListener.volume = savedVolume;

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        PlayerPrefs.SetFloat("Volume", volume);
    }
    #endregion
}