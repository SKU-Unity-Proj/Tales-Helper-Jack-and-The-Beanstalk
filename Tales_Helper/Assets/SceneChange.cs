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
        // 충돌한 오브젝트의 태그가 "GHD"인지 확인
        if (other.CompareTag("GHD"))
        {
            
            // "GiantMap" 씬으로 이동
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

