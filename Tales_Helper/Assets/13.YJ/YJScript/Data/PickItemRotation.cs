using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickItemRotation : MonoBehaviour
{
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;

    public Transform currentParent;

    private bool isFix = false;

    private void Start()
    {
        currentParent = transform.parent; // 부모 저장
    }

    private void Update()
    {
        if (transform.parent != currentParent) //부모 바뀜
        {
            isFix = true;

            currentParent = transform.parent; // 부모 업데이트
        }

        if (transform.parent != null && isFix) // 위치 고정
        {
            transform.localPosition = position;
            transform.localRotation = Quaternion.Euler(rotation);
        }

        if(transform.parent == null) // 위치 고정 해제
        {
            isFix = false;
        }
    }


    /*
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
    */
}
