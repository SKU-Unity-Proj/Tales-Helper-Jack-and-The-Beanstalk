using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnOff : MonoBehaviour
{
    public float minOuterSpotAngle = 106f; // 최소 각도
    public float middleOuterSpotAngle = 145f; // 중간 각도
    public float maxOuterSpotAngle = 179f; // 최대 각도
    public float changeDownSpeed = 380f; // 스포트 각도 감소 속도
    public float changeUpSpeed = 450f; // 스포트 각도 증가 속도

    private Light lightComponent;
    private bool increasing = true; // 스포트 각도가 증가 중인지 여부
    private bool isMiddleAngel = false;

    void Start()
    {
        lightComponent = GetComponent<Light>();

        if (lightComponent == null)
        {
            Debug.LogError("Light 컴포넌트를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        // 초기 외부 스포트 각도 설정
        lightComponent.spotAngle = maxOuterSpotAngle;
    }

    void Update()
    {
        if (increasing) // 증가
        {
            if (isMiddleAngel) // 중간값까지 증가
            {
                if (lightComponent.spotAngle >= middleOuterSpotAngle)
                {
                    increasing = false;
                    isMiddleAngel = false;
                }

                lightComponent.spotAngle += changeUpSpeed * Time.deltaTime;
            }
            else // 최대값까지 증가
            {
                if (lightComponent.spotAngle >= maxOuterSpotAngle)
                {
                    increasing = false;
                    isMiddleAngel = true;
                }

                lightComponent.spotAngle += changeUpSpeed * Time.deltaTime;
            }
        }
        else // 감소
        {
            if (lightComponent.spotAngle <= minOuterSpotAngle)
                increasing = true;

            lightComponent.spotAngle -= changeDownSpeed * Time.deltaTime;
        }
    }
}