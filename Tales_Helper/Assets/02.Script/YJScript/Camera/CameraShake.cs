using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour
{
    //시네머신 카메라에서 Noise -> 6D Shake 설정 먼저 해줘야 함


    public float ShakeDuration = 0.3f;          //카메라 흔들림 효과가 지속되는 시간
    public float ShakeAmplitude = 1.2f;         //카메라 파라미터
    public float ShakeFrequency = 2.0f;         //카메라 파라미터

    private float ShakeElapsedTime = 0f;

    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    void Start()
    {
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        /*
        if (Input.GetKey(KeyCode.G))
        {
            ShakeElapsedTime = ShakeDuration;
        }
        */

        if (VirtualCamera != null && virtualCameraNoise != null)
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
}