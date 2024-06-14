using UnityEngine;

public class DroppedObjectHandler : MonoBehaviour
{
    private DroppedObject manager;
    void Start()
    {
        // DroppedObjectManager의 인스턴스를 찾아서 할당
        manager = DroppedObject.Instance;

        // 매니저가 존재하지 않을 경우, 로그 메시지 출력 또는 오류 처리
        if (manager == null)
        {
            Debug.LogError("DroppedObjectManager 인스턴스를 찾을 수 없습니다.");
            // 필요한 경우, 여기에 추가적인 오류 처리 코드를 추가
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 매니저 인스턴스가 존재하는 경우에만 배열에 넣음
        if (manager != null && collision.gameObject.CompareTag("Ground"))
        {
            manager.AddDroppedObject(this.gameObject);
        }
    }
}
