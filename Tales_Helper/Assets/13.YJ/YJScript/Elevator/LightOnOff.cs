using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnOff : MonoBehaviour
{
    public float minOuterSpotAngle = 106f; // �ּ� ����
    public float middleOuterSpotAngle = 145f; // �߰� ����
    public float maxOuterSpotAngle = 179f; // �ִ� ����
    public float changeDownSpeed = 380f; // ����Ʈ ���� ���� �ӵ�
    public float changeUpSpeed = 450f; // ����Ʈ ���� ���� �ӵ�

    private Light lightComponent;
    private bool increasing = true; // ����Ʈ ������ ���� ������ ����
    private bool isMiddleAngel = false;

    void Start()
    {
        lightComponent = GetComponent<Light>();

        if (lightComponent == null)
        {
            Debug.LogError("Light ������Ʈ�� ã�� �� �����ϴ�.");
            enabled = false;
            return;
        }

        // �ʱ� �ܺ� ����Ʈ ���� ����
        lightComponent.spotAngle = maxOuterSpotAngle;
    }

    void Update()
    {
        if (increasing) // ����
        {
            if (isMiddleAngel) // �߰������� ����
            {
                if (lightComponent.spotAngle >= middleOuterSpotAngle)
                {
                    increasing = false;
                    isMiddleAngel = false;
                }

                lightComponent.spotAngle += changeUpSpeed * Time.deltaTime;
            }
            else // �ִ밪���� ����
            {
                if (lightComponent.spotAngle >= maxOuterSpotAngle)
                {
                    increasing = false;
                    isMiddleAngel = true;
                }

                lightComponent.spotAngle += changeUpSpeed * Time.deltaTime;
            }
        }
        else // ����
        {
            if (lightComponent.spotAngle <= minOuterSpotAngle)
                increasing = true;

            lightComponent.spotAngle -= changeDownSpeed * Time.deltaTime;
        }
    }
}