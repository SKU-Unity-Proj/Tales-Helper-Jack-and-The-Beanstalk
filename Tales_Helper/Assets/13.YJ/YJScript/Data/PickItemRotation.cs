using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickItemRotation : MonoBehaviour
{
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;

    [SerializeField] private Transform originBottle;
    [SerializeField] private Transform setBottle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            originBottle.gameObject.GetComponent<MeshRenderer>().enabled = false;
            originBottle.gameObject.GetComponent<MeshCollider>().enabled = false;

            setBottle.gameObject.SetActive(true);
        }
    }
}
