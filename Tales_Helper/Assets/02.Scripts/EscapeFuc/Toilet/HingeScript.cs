using UnityEngine;

public class HingeScript : MonoBehaviour
{
    public delegate void HingeDestroyedAction();
    public static event HingeDestroyedAction OnHingeDestroyed; // 나무판자에게 이벤트를 보냄

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer")) // Hammer 태그를 가진 오브젝트와 충돌
        {
            // 경첩 제거
            Destroy(gameObject);

            // 이벤트 호출
            if (OnHingeDestroyed != null)
            {
                OnHingeDestroyed.Invoke();
            }
        }
    }
}
