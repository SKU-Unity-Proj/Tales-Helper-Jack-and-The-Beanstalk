using UnityEngine;

public class DroppedObjectHandler : MonoBehaviour
{
    private DroppedObject manager;
    void Start()
    {
        // DroppedObjectManager�� �ν��Ͻ��� ã�Ƽ� �Ҵ�
        manager = DroppedObject.Instance;

        // �Ŵ����� �������� ���� ���, �α� �޽��� ��� �Ǵ� ���� ó��
        if (manager == null)
        {
            Debug.LogError("DroppedObjectManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            // �ʿ��� ���, ���⿡ �߰����� ���� ó�� �ڵ带 �߰�
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // �Ŵ��� �ν��Ͻ��� �����ϴ� ��쿡�� �迭�� ����
        if (manager != null && collision.gameObject.CompareTag("Ground"))
        {
            manager.AddDroppedObject(this.gameObject);
        }
    }
}
