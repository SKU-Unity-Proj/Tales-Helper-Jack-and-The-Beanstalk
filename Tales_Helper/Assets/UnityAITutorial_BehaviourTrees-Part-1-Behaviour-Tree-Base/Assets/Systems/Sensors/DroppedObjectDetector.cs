using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObjectDetector : MonoBehaviour
{
    public List<GameObject> DroppedObjects { get; private set; } = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // "DroppedObject" 태그를 가진 오브젝트를 감지
        if (other.CompareTag("DroppedObject"))
        {
            DroppedObjects.Add(other.gameObject);
        }
    }

    // 감지된 물체들을 클리어하는 메소드 (필요한 경우)
    public void ClearDetectedObjects()
    {
        DroppedObjects.Clear();
    }
}
