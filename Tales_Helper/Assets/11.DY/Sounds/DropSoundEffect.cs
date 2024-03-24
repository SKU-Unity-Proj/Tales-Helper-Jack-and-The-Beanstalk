using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSoundEffect : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // �ٴڰ��� �浹�� ����
        if (collision.gameObject.tag == "Floor")
        {
            audioSource.Play();
        }
    }
}

