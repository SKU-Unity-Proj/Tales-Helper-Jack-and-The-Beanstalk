using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsBootstrap : MonoBehaviour
{
    private static bool _isInitialized;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        if (_isInitialized) return;

        LoadCommonManagers();

        // 씬 로드 후 특정 매니저 로드 조건을 위한 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        _isInitialized = true;
    }

    //모든씬에 필요한 메니저 로드
    private static void LoadCommonManagers()
    {
        CreateSingleton<DataManager>("DataManager");
        CreateSingleton<SoundManager>("SoundManager");
        CreateSingleton<EffectManager>("EffectManager");
        CreateSingleton<BGMController>("BGMController");
        CreateSingleton<SettingsManager>("SettingsManager");
    }

    private static void LoadSceneSpecificManagers(string sceneName)
    {
        //특정 씬에서만 필요한 매니저 로드
        if (sceneName == "GiantMap")
        {
            CreateSingleton<BasicManager>("BasicManager");
            CreateSingleton<DetectableTargetManager>("DetectableTargetManager");
            CreateSingleton<HearingManager>("HearingManager");
        }
    }

    private static void CreateSingleton<T>(string managerName) where T : Component
    {
        if (GameObject.FindObjectOfType<T>() == null)
        {
            new GameObject(managerName, typeof(T));
        }
    }
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Current scene: {scene.name}"); // 씬 이름 확인 로그
        LoadSceneSpecificManagers(scene.name);
    }
}
