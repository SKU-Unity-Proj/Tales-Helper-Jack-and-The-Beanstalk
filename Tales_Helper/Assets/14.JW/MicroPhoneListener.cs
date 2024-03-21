using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MicroPhoneListener : MonoBehaviour
{
    public float sensitivity = 100; // 소리 감도
    public float loudness = 0; // 소리 크기
    public float pitch = 0; // 소리 높낮이
    AudioSource _audio; // 오디오 소스

    public float RmsValue; // Root Mean Square (RMS) 값
    public float DbValue; // 데시벨 값
    public float PitchValue; // 음의 높낮이 값

    private const int QSamples = 1024; // 샘플 수
    private const float RefValue = 0.1f; // 기준값
    private const float Threshold = 0.02f; // 임계값

    float[] _samples; // 샘플 배열
    private float[] _spectrum; // 주파수 스펙트럼 배열
    private float _fSample; // 샘플링 레이트

    // 시작할 때 마이크 리스너를 켤지 여부
    public bool startMicOnStartup = true;

    // 실행 중에 마이크 리스너를 시작하거나 중지할 수 있는 옵션
    public bool stopMicrophoneListener = false;
    public bool startMicrophoneListener = false;

    private bool microphoneListenerOn = false;

    // 임시로 마이크 출력을 스피커로 듣고 싶은 경우를 위한 공용 변수
    // 내부적으로는 마이크 리스너가 켜져 있는지 여부에 따라 출력 소리를 스피커로 전환합니다.
    public bool disableOutputSound = false;

    // 이 스크립트가 연결된 동일한 오브젝트에 있는 오디오 소스
    AudioSource src;

    // 'Create' 메뉴에서 오디오 믹서를 만든 다음 이 스크립트의 공용 필드로 끌어다 놓습니다.
    // 오디오 믹서를 더블 클릭한 다음 "groups" 섹션 옆의 "+" 아이콘을 클릭하여 마스터 그룹의 하위로 추가합니다.
    // 그런 다음 오디오 소스에서 "output" 옵션에서 방금 만든 마스터의 자식을 선택합니다.
    // 다시 오디오 믹서 인스펙터 창으로 돌아가서 방금 만든 "마이크"를 클릭한 다음 인스펙터 창에서 "Volume"을 마우스 오른쪽 단추로 클릭하고 "스크립트에 노출된 볼륨으로 설정"을 선택합니다.
    // 그런 다음 오디오 믹서 창에서 "노출된 매개변수"를 클릭한 다음 "MyExposedParameter"를 클릭하고 "Volume"로 이름을 변경합니다.
    public AudioMixer masterMixer;

    float timeSinceRestart = 0;

    void Start()
    {
        // 시작할 때 마이크 리스너 시작
        if (startMicOnStartup)
        {
            RestartMicrophoneListener();
            StartMicrophoneListener();

            _audio = GetComponent<AudioSource>();
            _audio.clip = Microphone.Start(null, true, 10, 44100); // 마이크에서 오디오 클립을 얻어옵니다.
            _audio.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { } // 마이크의 위치가 양수인지 확인하여 재생 대기
            _audio.Play(); // 오디오 재생 시작
            _samples = new float[QSamples];
            _spectrum = new float[QSamples];
            _fSample = AudioSettings.outputSampleRate;
            // 유니티 5.x부터는 오디오 소스의 음소거를 하면 정상적으로 음성이 들리지 않습니다.
            // 오디오 믹서에서 마스터 볼륨을 -80으로 설정하여 소리를 출력하지 않도록 합니다.
            // _audio.mute = true;
        }
    }

    void Update()
    {
        // 인스펙터에 나타나는 이 변수들을 사용할 수 있으며, 다른 스크립트에서 직접 public 함수를 호출할 수도 있습니다.
        if (stopMicrophoneListener)
        {
            StopMicrophoneListener();
        }
        if (startMicrophoneListener)
        {
            StartMicrophoneListener();
        }
        // 변수를 한 번만 실행하려면 매개변수를 false로 재설정합니다.
        stopMicrophoneListener = false;
        startMicrophoneListener = false;

        // 반드시 업데이트에서 실행해야만 작동합니다.
        MicrophoneIntoAudioSource(microphoneListenerOn);

        // 원하는 경우 인스펙터에서 소리를 음소거할 수 있습니다.
        DisableSound(!disableOutputSound);

        loudness = GetAveragedVolume() * sensitivity; // 평균 볼륨을 얻어옵니다.
        GetPitch(); // 음의 높낮이를 가져옵니다.

        // 아래는 커스텀한 소스
        // 소리가 특정 크기 이상인 경우 이미지를 채웁니다.
        //if (loudness > 5f)
        //    _imageSound.fillAmount = 1f;
        //else
        //{
        //    _imageSound.fillAmount = 0.65f;
        //}

        // GameManager에서 현재 소리 크기를 가져와서 설정합니다.
        //FindObjectOfType<GameManager>().currentLoud = loudness;
        // TextVol.text = "vol:" + loudness; // 볼륨 텍스트 업데이트
    }

    // 마이크로부터 오디오 데이터를 가져와서 평균 볼륨을 계산하는 함수
    float GetAveragedVolume()
    {
        // 256개의 샘플을 저장할 배열
        float[] data = new float[256];
        float a = 0;
        // 마이크로부터 오디오 데이터를 가져와서 data 배열에 저장
        _audio.GetOutputData(data, 0);
        // 모든 샘플의 절대값을 더하여 합산
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        // 평균 계산 후 반환
        return a / 256;
    }

    // 마이크로부터 오디오 데이터를 받아서 주파수를 계산하는 함수
    void GetPitch()
    {
        // _samples 배열에 오디오 데이터를 채움
        GetComponent<AudioSource>().GetOutputData(_samples, 0);
        int i;
        float sum = 0;
        // 샘플 데이터의 제곱 값을 합산
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i];
        }
        // 평균값의 제곱근을 계산하여 RMS 값 계산
        RmsValue = Mathf.Sqrt(sum / QSamples);
        // RMS 값에 따라 데시벨 값 계산
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue);
        // 최소 데시벨 값에 대한 클램핑
        if (DbValue < -160) DbValue = -160;
        // 주파수 스펙트럼 데이터를 가져옴
        GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        // 최대 주파수와 해당 주파수의 값(크기)를 찾음
        for (i = 0; i < QSamples; i++)
        {
            // 최대값보다 크고 임계값보다 큰 주파수를 찾음
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;
            maxV = _spectrum[i];
            maxN = i; // 최대값의 인덱스 저장
        }
        float freqN = maxN; // 인덱스를 float 변수로 전달
        if (maxN > 0 && maxN < QSamples - 1)
        {
            // 이웃한 값들을 사용하여 주파수 보정
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        // 인덱스를 주파수로 변환
        PitchValue = freqN * (_fSample / 2) / QSamples;
    }

    // 마이크 리스너를 중지하고 모든 설정 초기화하는 함수
    public void StopMicrophoneListener()
    {
        // 마이크 리스너 중지
        microphoneListenerOn = false;
        // 마스터 믹서의 소리 다시 활성화
        disableOutputSound = false;
        // 오디오 소스에서 마이크 클립 제거
        src.Stop();
        src.clip = null;
        // 마이크 종료
        Microphone.End(null);
    }

    // 마이크 리스너를 시작하는 함수
    public void StartMicrophoneListener()
    {
        // 마이크 리스너 시작
        microphoneListenerOn = true;
        // 소리 출력 비활성화 (마이크 입력은 듣고 싶지 않음)
        disableOutputSound = true;
        // 오디오 소스 초기화
        RestartMicrophoneListener();
    }

    // 소리를 끄거나 켜는 함수
    public void DisableSound(bool SoundOn)
    {
        float volume = 0;
        if (SoundOn)
        {
            volume = 0.0f; // 소리를 켬
        }
        else
        {
            volume = -80.0f; // 소리를 끔
        }
        // 마스터 믹서의 볼륨 설정
        masterMixer.SetFloat("MasterVolume", volume);
    }

    // 마이크 리스너 재시작 함수
    public void RestartMicrophoneListener()
    {
        src = GetComponent<AudioSource>();
        // 오디오 소스에서 모든 사운드 파일 제거
        src.clip = null;
        // 시간 초기화
        timeSinceRestart = Time.time;
    }

    // 마이크 입력을 오디오 소스에 넣는 함수
    void MicrophoneIntoAudioSource(bool MicrophoneListenerOn)
    {
        if (MicrophoneListenerOn)
        {
            // 오디오 소스에 클립 설정
            if (Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
            {
                src.clip = Microphone.Start(null, true, 10, 44100);
                while (!(Microphone.GetPosition(null) > 0)) { }
                // 마이크 위치 찾을 때까지 대기 후 재생
                src.Play();
            }
        }
    }
}

