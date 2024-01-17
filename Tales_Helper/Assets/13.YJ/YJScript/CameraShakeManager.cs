using Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    public CinemachineVirtualCamera mainCam; //Shake를 줄 카메라

    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    private float ShakeAmplitude = 3.0f;         //카메라 파라미터
    private float ShakeFrequency = 3.0f;         //카메라 파라미터
    private float ShakeElapsedTime = 0f;        //카메라 흔들림 효과가 지속되는 시간

    void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion

        if (mainCam != null)
            virtualCameraNoise = mainCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ShakeElapsedTime != 0 && mainCam != null && virtualCameraNoise != null)
        {
            if (ShakeElapsedTime > 0)
            {
                virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                ShakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;
            }
        }
    }

    //카메라 흔드는 시간
    public void SetShakeTime(float ShakeDuration)
    {
        ShakeElapsedTime = ShakeDuration;
        Debug.Log(ShakeDuration);
    }

    //카메라 흔들림 세기
    public void SetShakeDegree(float A, float B)
    {
        ShakeAmplitude = A;
        ShakeFrequency = B;
    }
}
