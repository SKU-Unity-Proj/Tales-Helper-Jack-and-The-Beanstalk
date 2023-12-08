using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionMenu : MonoBehaviour
{
    /*����*/
    //������
    //public Slider BgmPrefab;
    //public Slider SoundEffectPrefab;
    //��ġ
    public Transform bgmTransform;
    public Transform effectTransform;
    //����
    public Slider BgmSlider;
    public Slider SoundEffectSlider;

    //���������
    public AudioSource Bgm;
    public AudioSource SoundEffect;

    /* Ű */
    //������
    //public TMP_InputField WPrefab;
    //public InputField SPrefab;
    //public InputField APrefab;
    //public InputField DPrefab;
    //public InputField JumpPrefab;
    //����
    public TMP_InputField WField;
    InputField SField;
    InputField AField;
    InputField DField;
    InputField JumpField;
    //����� ��ġ
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

    // ���� ��ư��
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

    // ����
    void OnBgmSliderValueChanged(float volume)
    {
        Bgm.volume = volume;
    }
    void OnSoundEffectSliderValueChanged(float volume)
    {
        SoundEffect.volume = volume;
    }

    //Ű����
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
