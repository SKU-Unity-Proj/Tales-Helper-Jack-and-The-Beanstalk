using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CloserGiant : MonoBehaviour
{/*
    public float ShakeDuration = 0.1f;          //카메라 흔들림 효과가 지속되는 시간
    public float ShakeAmplitude = 3f;         //카메라 파라미터
    public float ShakeFrequency = 2.0f;         //카메라 파라미터

    private float ShakeElapsedTime = 0f;

    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    [SerializeField]
    private LayerMask layerMask;
    public float radius = 3;

    void Start()
    {
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Giant")
        {
            StartCoroutine("FeetVibration");
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Giant")
        {
            StopAllCoroutines();
        }
    }

    IEnumerator FeetVibration()
    {
        virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
        virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

        yield return new WaitForSecondsRealtime(0.1f);

        virtualCameraNoise.m_AmplitudeGain = 0f;

        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine("FeetVibration");
    }
    */
}