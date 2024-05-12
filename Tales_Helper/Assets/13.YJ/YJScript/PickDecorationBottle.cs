using UnityEngine;

public class PickDecorationBottle : MonoBehaviour
{
    private Transform currentParent;
    private Rigidbody rigid;

    private void Start()
    {
        currentParent = transform.parent; // 부모 저장
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.parent != currentParent) //부모 바뀜
        {
            currentParent = transform.parent; // 부모 업데이트
            rigid.isKinematic = false;
        }
    }
}
