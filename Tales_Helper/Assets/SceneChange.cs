using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 태그가 "GHD"인지 확인
        if (other.CompareTag("GHD"))
        {
            // "GiantMap" 씬으로 이동
            SceneManager.LoadScene("GiantMap");
        }
    }
}