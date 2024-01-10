using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObjectDetector : MonoBehaviour
{
    public List<GameObject> DroppedObjects { get; private set; } = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // "DroppedObject" �±׸� ���� ������Ʈ�� ����
        if (other.CompareTag("DroppedObject"))
        {
            DroppedObjects.Add(other.gameObject);
        }
    }

    // ������ ��ü���� Ŭ�����ϴ� �޼ҵ� (�ʿ��� ���)
    public void ClearDetectedObjects()
    {
        DroppedObjects.Clear();
    }
}
