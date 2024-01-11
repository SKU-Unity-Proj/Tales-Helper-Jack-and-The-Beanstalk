using UnityEngine;

public class DroppedObjectHandler : MonoBehaviour
{
    private DroppedObject manager;

    void Start()
    {
        manager = GameObject.FindObjectOfType<DroppedObject>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // ���ο��� ����ĳ��Ʈ�� ��� ���� DroppedObject ��ũ��Ʈ�� ����
        manager.HandleObjectCollision(gameObject);
    }
}
