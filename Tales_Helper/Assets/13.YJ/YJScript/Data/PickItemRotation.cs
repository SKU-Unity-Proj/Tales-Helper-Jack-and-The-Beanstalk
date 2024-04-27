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
        currentParent = transform.parent; // �θ� ����
    }

    private void Update()
    {
        if (transform.parent != currentParent) //�θ� �ٲ�
        {
            isFix = true;

            currentParent = transform.parent; // �θ� ������Ʈ
        }

        if (transform.parent != null && isFix) // ��ġ ����
        {
            transform.localPosition = position;
            transform.localRotation = Quaternion.Euler(rotation);
        }

        if(transform.parent == null) // ��ġ ���� ����
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
