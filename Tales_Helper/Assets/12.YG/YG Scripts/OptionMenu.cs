using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionMenu : MonoBehaviour
{
    /*사운드*/
    //프리팹
    //public Slider BgmPrefab;
    //public Slider SoundEffectPrefab;
    //위치
    public Transform bgmTransform;
    public Transform effectTransform;
    //변수
    public Slider BgmSlider;
    public Slider SoundEffectSlider;

    //오디오파일
    public AudioSource Bgm;
    public AudioSource SoundEffect;

    /* 키 */
    //프리팹
    //public TMP_InputField WPrefab;
    //public InputField SPrefab;
    //public InputField APrefab;
    //public InputField DPrefab;
    //public InputField JumpPrefab;
    //변수
    public TMP_InputField WField;
    InputField SField;
    InputField AField;
    InputField DField;
    InputField JumpField;
    //생기는 위치
    public Transform wTransform;
    public Transform aTransform;
    public Transform sTransform;
    public Transform dTransform;
    public Transform jumpTransform;
    private string wkey = null;

    private void Start()
    {
        //HideUI();
        wkey = WField.GetComponent<InputField>().text;
        
    }

    // 메인 버튼들
    public void OnClickSoundBtn()
    {
        SoundEffect.Play();
        //HideUI();
        //BgmSlider = Instantiate(BgmPrefab, bgmTransform);
        //SoundEffectSlider = Instantiate(SoundEffectPrefab, effectTransform);

        BgmSlider.onValueChanged.AddListener(OnBgmSliderValueChanged);
        SoundEffectSlider.onValueChanged.AddListener(OnSoundEffectSliderValueChanged);
    }
    public void OnClickKeyBtn()
    {
        SoundEffect.Play();
        //HideUI();
        //WField = Instantiate(WPrefab, wTransform);
        WField.onValueChanged.AddListener(OnWFieldValueChanged);

    }
    
    public void OnClickDisplayBtn()
    {
        SoundEffect.Play();
        //HideUI();

    }

    // 볼륨
    void OnBgmSliderValueChanged(float volume)
    {
        Bgm.volume = volume;
    }
    void OnSoundEffectSliderValueChanged(float volume)
    {
        SoundEffect.volume = volume;
    }

    //키보드
    void OnWFieldValueChanged(string value)
    {
        if (wkey.Length > 0 && Input.GetKeyDown(KeyCode.Return))
        {
            wkey = WField.text;
            //PlayerPrefs.SetString("CurrentWkey", wkey);
            //GameManager.instance.ScoreSet(GameManager.instance.score, playerName);

        }
    }
    /*
    private void HideUI()
    {
        if (BgmSlider != null)
        {
            Destroy(BgmSlider);
        }
        if (SoundEffectSlider != null)
        {
            Destroy(SoundEffectSlider);
        }
        if (WField != null)
        {
            Destroy(WField);
        }
    }
    */
}
