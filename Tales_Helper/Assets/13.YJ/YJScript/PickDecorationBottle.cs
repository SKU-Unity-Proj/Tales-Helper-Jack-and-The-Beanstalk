using UnityEngine;

public class PickDecorationBottle : MonoBehaviour
{
    private Transform currentParent;
    private Rigidbody rigid;

    private void Start()
    {
        currentParent = transform.parent; // �θ� ����
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.parent != currentParent) //�θ� �ٲ�
        {
            currentParent = transform.parent; // �θ� ������Ʈ
            rigid.isKinematic = false;
        }
    }
}
