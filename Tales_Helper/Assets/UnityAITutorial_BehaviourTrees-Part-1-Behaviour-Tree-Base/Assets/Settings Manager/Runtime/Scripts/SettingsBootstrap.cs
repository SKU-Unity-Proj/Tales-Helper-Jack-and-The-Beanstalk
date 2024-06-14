using UnityEngine;
using UnityEngine.SceneManagement;

static public class SettingsBootstrap
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadSettingsScene()
    {
        // 공통적으로 로드해야 할 매니저들
        LoadCommonManagers();

        // 현재 활성화된 씬을 가져옴
        Scene activeScene = SceneManager.GetActiveScene();
        string activeSceneName = activeScene.name;

        Debug.Log(activeSceneName);

        // 씬 이름에 따라 다른 오브젝트를 로드
        //LoadSceneSpecificManagers(activeSceneName);

    }

    private static void LoadCommonManagers()
    {
        if (GameObject.FindObjectOfType<DataManager>() == null)
        {
            new GameObject("DataManager", typeof(DataManager));
        }
        if (GameObject.FindObjectOfType<SoundManager>() == null)
        {
            new GameObject("SoundManager", typeof(SoundManager));
        }
        if (GameObject.FindObjectOfType<EffectManager>() == null)
        {
            new GameObject("EffectManager", typeof(EffectManager));
        }
        if (GameObject.FindObjectOfType<BGMController>() == null)
        {
            new GameObject("BGMController", typeof(BGMController));
        }
        if (GameObject.FindObjectOfType<SettingsManager>() == null)
        {
            new GameObject("SettingsManager", typeof(SettingsManager));
        }
    }

    private static void LoadSceneSpecificManagers(string sceneName)
    {
        switch (sceneName)
        {
            case "GiantHouse":
                LoadGiantHouseManagers();
                break;
            case "JackHouse":
                LoadJackHouseManagers();
                break;
            case "GiantMap":
                LoadGiantMapManagers();
                break;
            // 추가적인 씬에 대한 로직 추가
            default:
                Debug.LogWarning("No specific managers defined for scene: " + sceneName);
                break;
        }
    }

    private static void LoadGiantHouseManagers()
    {
     
        // GiantHouse 씬에 필요한 추가적인 매니저들 로드
    }

    private static void LoadJackHouseManagers()
    {
     
        // JackHouse 씬에 필요한 추가적인 매니저들 로드
    }

    private static void LoadGiantMapManagers()
    {
 
        // GiantMap 씬에 필요한 추가적인 매니저들 로드
    }
}
