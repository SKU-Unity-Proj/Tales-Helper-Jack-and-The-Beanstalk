using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� �±װ� "GHD"���� Ȯ��
        if (other.CompareTag("GHD"))
        {
            // "GiantMap" ������ �̵�
            SceneManager.LoadScene("GiantMap");
        }
    }
}