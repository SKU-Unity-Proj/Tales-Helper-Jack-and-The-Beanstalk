using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shakeDoor : MonoBehaviour
{
    public float shakeDuration = 1.0f; // ��鸮�� �ð�
    public float shakeAmount = 0.1f; // ��鸮�� ����
    public float decreaseFactor = 1.0f; // ��鸲�� �پ��� �ӵ�

    private Vector3 originalPosition;
    private float currentShakeDuration = 0.0f;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("clubCol"))
        {
            currentShakeDuration = shakeDuration;
        }
    }

    void Update()
    {
        if (currentShakeDuration > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
            transform.localPosition = new Vector3(originalPosition.x, transform.localPosition.y, transform.localPosition.z);

            currentShakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            currentShakeDuration = 0.0f;
            transform.localPosition = originalPosition;
        }
    }
}
