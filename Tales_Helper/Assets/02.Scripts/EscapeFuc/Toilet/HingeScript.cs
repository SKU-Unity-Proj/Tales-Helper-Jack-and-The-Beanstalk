using UnityEngine;

public class HingeScript : MonoBehaviour
{
    public delegate void HingeDestroyedAction();
    public static event HingeDestroyedAction OnHingeDestroyed; // �������ڿ��� �̺�Ʈ�� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer")) // Hammer �±׸� ���� ������Ʈ�� �浹
        {
            // ��ø ����
            Destroy(gameObject);

            // �̺�Ʈ ȣ��
            if (OnHingeDestroyed != null)
            {
                OnHingeDestroyed.Invoke();
            }
        }
    }
}
