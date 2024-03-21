using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MicroPhoneListener : MonoBehaviour
{
    public float sensitivity = 100; // �Ҹ� ����
    public float loudness = 0; // �Ҹ� ũ��
    public float pitch = 0; // �Ҹ� ������
    AudioSource _audio; // ����� �ҽ�

    public float RmsValue; // Root Mean Square (RMS) ��
    public float DbValue; // ���ú� ��
    public float PitchValue; // ���� ������ ��

    private const int QSamples = 1024; // ���� ��
    private const float RefValue = 0.1f; // ���ذ�
    private const float Threshold = 0.02f; // �Ӱ谪

    float[] _samples; // ���� �迭
    private float[] _spectrum; // ���ļ� ����Ʈ�� �迭
    private float _fSample; // ���ø� ����Ʈ

    // ������ �� ����ũ �����ʸ� ���� ����
    public bool startMicOnStartup = true;

    // ���� �߿� ����ũ �����ʸ� �����ϰų� ������ �� �ִ� �ɼ�
    public bool stopMicrophoneListener = false;
    public bool startMicrophoneListener = false;

    private bool microphoneListenerOn = false;

    // �ӽ÷� ����ũ ����� ����Ŀ�� ��� ���� ��츦 ���� ���� ����
    // ���������δ� ����ũ �����ʰ� ���� �ִ��� ���ο� ���� ��� �Ҹ��� ����Ŀ�� ��ȯ�մϴ�.
    public bool disableOutputSound = false;

    // �� ��ũ��Ʈ�� ����� ������ ������Ʈ�� �ִ� ����� �ҽ�
    AudioSource src;

    // 'Create' �޴����� ����� �ͼ��� ���� ���� �� ��ũ��Ʈ�� ���� �ʵ�� ����� �����ϴ�.
    // ����� �ͼ��� ���� Ŭ���� ���� "groups" ���� ���� "+" �������� Ŭ���Ͽ� ������ �׷��� ������ �߰��մϴ�.
    // �׷� ���� ����� �ҽ����� "output" �ɼǿ��� ��� ���� �������� �ڽ��� �����մϴ�.
    // �ٽ� ����� �ͼ� �ν����� â���� ���ư��� ��� ���� "����ũ"�� Ŭ���� ���� �ν����� â���� "Volume"�� ���콺 ������ ���߷� Ŭ���ϰ� "��ũ��Ʈ�� ����� �������� ����"�� �����մϴ�.
    // �׷� ���� ����� �ͼ� â���� "����� �Ű�����"�� Ŭ���� ���� "MyExposedParameter"�� Ŭ���ϰ� "Volume"�� �̸��� �����մϴ�.
    public AudioMixer masterMixer;

    float timeSinceRestart = 0;

    void Start()
    {
        // ������ �� ����ũ ������ ����
        if (startMicOnStartup)
        {
            RestartMicrophoneListener();
            StartMicrophoneListener();

            _audio = GetComponent<AudioSource>();
            _audio.clip = Microphone.Start(null, true, 10, 44100); // ����ũ���� ����� Ŭ���� ���ɴϴ�.
            _audio.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { } // ����ũ�� ��ġ�� ������� Ȯ���Ͽ� ��� ���
            _audio.Play(); // ����� ��� ����
            _samples = new float[QSamples];
            _spectrum = new float[QSamples];
            _fSample = AudioSettings.outputSampleRate;
            // ����Ƽ 5.x���ʹ� ����� �ҽ��� ���ҰŸ� �ϸ� ���������� ������ �鸮�� �ʽ��ϴ�.
            // ����� �ͼ����� ������ ������ -80���� �����Ͽ� �Ҹ��� ������� �ʵ��� �մϴ�.
            // _audio.mute = true;
        }
    }

    void Update()
    {
        // �ν����Ϳ� ��Ÿ���� �� �������� ����� �� ������, �ٸ� ��ũ��Ʈ���� ���� public �Լ��� ȣ���� ���� �ֽ��ϴ�.
        if (stopMicrophoneListener)
        {
            StopMicrophoneListener();
        }
        if (startMicrophoneListener)
        {
            StartMicrophoneListener();
        }
        // ������ �� ���� �����Ϸ��� �Ű������� false�� �缳���մϴ�.
        stopMicrophoneListener = false;
        startMicrophoneListener = false;

        // �ݵ�� ������Ʈ���� �����ؾ߸� �۵��մϴ�.
        MicrophoneIntoAudioSource(microphoneListenerOn);

        // ���ϴ� ��� �ν����Ϳ��� �Ҹ��� ���Ұ��� �� �ֽ��ϴ�.
        DisableSound(!disableOutputSound);

        loudness = GetAveragedVolume() * sensitivity; // ��� ������ ���ɴϴ�.
        GetPitch(); // ���� �����̸� �����ɴϴ�.

        // �Ʒ��� Ŀ������ �ҽ�
        // �Ҹ��� Ư�� ũ�� �̻��� ��� �̹����� ä��ϴ�.
        //if (loudness > 5f)
        //    _imageSound.fillAmount = 1f;
        //else
        //{
        //    _imageSound.fillAmount = 0.65f;
        //}

        // GameManager���� ���� �Ҹ� ũ�⸦ �����ͼ� �����մϴ�.
        //FindObjectOfType<GameManager>().currentLoud = loudness;
        // TextVol.text = "vol:" + loudness; // ���� �ؽ�Ʈ ������Ʈ
    }

    // ����ũ�κ��� ����� �����͸� �����ͼ� ��� ������ ����ϴ� �Լ�
    float GetAveragedVolume()
    {
        // 256���� ������ ������ �迭
        float[] data = new float[256];
        float a = 0;
        // ����ũ�κ��� ����� �����͸� �����ͼ� data �迭�� ����
        _audio.GetOutputData(data, 0);
        // ��� ������ ���밪�� ���Ͽ� �ջ�
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        // ��� ��� �� ��ȯ
        return a / 256;
    }

    // ����ũ�κ��� ����� �����͸� �޾Ƽ� ���ļ��� ����ϴ� �Լ�
    void GetPitch()
    {
        // _samples �迭�� ����� �����͸� ä��
        GetComponent<AudioSource>().GetOutputData(_samples, 0);
        int i;
        float sum = 0;
        // ���� �������� ���� ���� �ջ�
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i];
        }
        // ��հ��� �������� ����Ͽ� RMS �� ���
        RmsValue = Mathf.Sqrt(sum / QSamples);
        // RMS ���� ���� ���ú� �� ���
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue);
        // �ּ� ���ú� ���� ���� Ŭ����
        if (DbValue < -160) DbValue = -160;
        // ���ļ� ����Ʈ�� �����͸� ������
        GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        // �ִ� ���ļ��� �ش� ���ļ��� ��(ũ��)�� ã��
        for (i = 0; i < QSamples; i++)
        {
            // �ִ밪���� ũ�� �Ӱ谪���� ū ���ļ��� ã��
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;
            maxV = _spectrum[i];
            maxN = i; // �ִ밪�� �ε��� ����
        }
        float freqN = maxN; // �ε����� float ������ ����
        if (maxN > 0 && maxN < QSamples - 1)
        {
            // �̿��� ������ ����Ͽ� ���ļ� ����
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        // �ε����� ���ļ��� ��ȯ
        PitchValue = freqN * (_fSample / 2) / QSamples;
    }

    // ����ũ �����ʸ� �����ϰ� ��� ���� �ʱ�ȭ�ϴ� �Լ�
    public void StopMicrophoneListener()
    {
        // ����ũ ������ ����
        microphoneListenerOn = false;
        // ������ �ͼ��� �Ҹ� �ٽ� Ȱ��ȭ
        disableOutputSound = false;
        // ����� �ҽ����� ����ũ Ŭ�� ����
        src.Stop();
        src.clip = null;
        // ����ũ ����
        Microphone.End(null);
    }

    // ����ũ �����ʸ� �����ϴ� �Լ�
    public void StartMicrophoneListener()
    {
        // ����ũ ������ ����
        microphoneListenerOn = true;
        // �Ҹ� ��� ��Ȱ��ȭ (����ũ �Է��� ��� ���� ����)
        disableOutputSound = true;
        // ����� �ҽ� �ʱ�ȭ
        RestartMicrophoneListener();
    }

    // �Ҹ��� ���ų� �Ѵ� �Լ�
    public void DisableSound(bool SoundOn)
    {
        float volume = 0;
        if (SoundOn)
        {
            volume = 0.0f; // �Ҹ��� ��
        }
        else
        {
            volume = -80.0f; // �Ҹ��� ��
        }
        // ������ �ͼ��� ���� ����
        masterMixer.SetFloat("MasterVolume", volume);
    }

    // ����ũ ������ ����� �Լ�
    public void RestartMicrophoneListener()
    {
        src = GetComponent<AudioSource>();
        // ����� �ҽ����� ��� ���� ���� ����
        src.clip = null;
        // �ð� �ʱ�ȭ
        timeSinceRestart = Time.time;
    }

    // ����ũ �Է��� ����� �ҽ��� �ִ� �Լ�
    void MicrophoneIntoAudioSource(bool MicrophoneListenerOn)
    {
        if (MicrophoneListenerOn)
        {
            // ����� �ҽ��� Ŭ�� ����
            if (Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
            {
                src.clip = Microphone.Start(null, true, 10, 44100);
                while (!(Microphone.GetPosition(null) > 0)) { }
                // ����ũ ��ġ ã�� ������ ��� �� ���
                src.Play();
            }
        }
    }
}

