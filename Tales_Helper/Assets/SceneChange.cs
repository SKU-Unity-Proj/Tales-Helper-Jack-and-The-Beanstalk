using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private bool oneTime=false;
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
            Invoke("LoadScene", 1.0f);
        }
    }
    private void LoadScene()
    {
        //SceneManager.LoadScene("GiantMap");
        if(!oneTime)
            LoadingSceneController.Instance.LoadScene("GiantMap");
        else
            return;
        oneTime = true;
    }
}

