using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entranceChandelier : MonoBehaviour
{

    public Transform originChandelier;
    public List<Transform> transformList = new List<Transform>();

    public void OnSignalReceiveChandelier()
    {

        originChandelier.gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("123");
            originChandelier.gameObject.GetComponent<MeshRenderer>().enabled = false;

            foreach (Transform trans in transformList)
            {

                trans.gameObject.SetActive(true);

            }
        }
    }
}
