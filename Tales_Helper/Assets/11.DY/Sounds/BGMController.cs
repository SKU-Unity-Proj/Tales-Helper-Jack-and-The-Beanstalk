using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    private static BGMController instance;

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
        // ���� �ε�� �� BGM ���
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "JackHouse":
                SoundManager.Instance.PlayBGM(SoundList.JackHouse);
                break;
            case "GiantMap":
                SoundManager.Instance.PlayBGM(SoundList.Celler);
                break;
            // �߰����� ���� ���� �ʿ��� ��� �߰�
            default:
                Debug.LogWarning("No BGM defined for scene: " + sceneName);
                break;
        }
    }

}
