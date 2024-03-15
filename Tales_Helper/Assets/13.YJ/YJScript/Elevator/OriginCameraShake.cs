using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginCameraShake : MonoBehaviour
{
    private float shakeTime;  //���� �ð�
    private float shakeIntensity; //��鸲 ����
    public Transform cam;

    public void OnShakeCamera(float shakeTime = 0.15f, float shakeIntensity = 0.3f)
    {
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;

        StopCoroutine("ShakeByPoisition");
        StartCoroutine("ShakeByPosition");
    }

    private IEnumerator ShakeByPosition()
    {
        Vector3 startPosition = cam.transform.position;

        while (shakeTime > 0.0f)
        {
            // �̵����� �ʴ� ���� 0 ���
            float x = 0f;
            float y = Random.Range(-1f, 1f);
            float z = 0f;
            cam.transform.position = startPosition + new Vector3(x,y,z) * shakeIntensity;

            //�� ������ �����ϰ� ����
            //cam.transform.position = startPosition + Random.insideUnitSphere * shakeIntensity;

            shakeTime -= Time.deltaTime;

            yield return null;
        }

        cam.transform.position = startPosition;
    }
}
