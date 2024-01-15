using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public enum SceneName
    {
        introamimation,
        GiantMap
    }

    /*
    public Scene LoadScene(SceneName sceneName )
    {

        Scene scene = SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Additive );

    }
    */


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