using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginCameraShake : MonoBehaviour
{
    private float shakeTime;  //흔드는 시간
    private float shakeIntensity; //흔들림 강도
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
            // 이동하지 않는 축은 0 사용
            float x = 0f;
            float y = Random.Range(-1f, 1f);
            float z = 0f;
            cam.transform.position = startPosition + new Vector3(x,y,z) * shakeIntensity;

            //구 범위로 랜덤하게 흔들기
            //cam.transform.position = startPosition + Random.insideUnitSphere * shakeIntensity;

            shakeTime -= Time.deltaTime;

            yield return null;
        }

        cam.transform.position = startPosition;
    }
}
