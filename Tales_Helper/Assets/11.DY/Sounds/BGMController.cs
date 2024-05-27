using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMController : MonoBehaviour
{
    public static BGMController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때 BGM 재생
        PlayBGMForScene(scene.name);

    }

    private void PlayBGMForScene(string sceneName)
    {
        Debug.Log("PlayBGMForScene called for scene: " + sceneName); // 메서드 호출 디버그 출력
        switch (sceneName)
        {
            case "GiantHouse":
                SoundManager.Instance.PlayBGM(SoundList.GiantHouse);
                break;
            case "JackHouse":
                SoundManager.Instance.PlayBGM(SoundList.JackHouse);
                break;
            case "GiantMap":
                SoundManager.Instance.PlayBGM(SoundList.Celler);
                break;
            // 추가적인 씬에 대해 필요한 경우 추가
            default:
                Debug.LogWarning("No BGM defined for scene: " + sceneName);
                break;
        }
    }

}
