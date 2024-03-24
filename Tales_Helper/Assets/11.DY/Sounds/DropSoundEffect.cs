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
        // 바닥과의 충돌을 감지
        if (collision.gameObject.tag == "Floor")
        {
            audioSource.Play();
        }
    }
}

