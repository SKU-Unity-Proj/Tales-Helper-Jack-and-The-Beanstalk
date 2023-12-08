using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour
{
    //�ó׸ӽ� ī�޶󿡼� Noise -> 6D Shake ���� ���� ����� ��


    public float ShakeDuration = 0.3f;          //ī�޶� ��鸲 ȿ���� ���ӵǴ� �ð�
    public float ShakeAmplitude = 1.2f;         //ī�޶� �Ķ����
    public float ShakeFrequency = 2.0f;         //ī�޶� �Ķ����

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