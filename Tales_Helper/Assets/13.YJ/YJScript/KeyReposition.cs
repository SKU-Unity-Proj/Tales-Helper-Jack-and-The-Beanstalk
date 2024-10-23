using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyReposition : MonoBehaviour
{
    private Transform respawnPos;

    private void Start()
    {
        respawnPos = transform.GetChild(0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Key"))
        {
            collision.transform.position = respawnPos.position;
        }
    }
}
