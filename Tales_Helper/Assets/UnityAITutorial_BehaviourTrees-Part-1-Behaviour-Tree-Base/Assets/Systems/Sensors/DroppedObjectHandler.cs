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
        // 거인에게 레이캐스트를 쏘는 것은 DroppedObject 스크립트에 위임
        manager.HandleObjectCollision(gameObject);
    }
}
