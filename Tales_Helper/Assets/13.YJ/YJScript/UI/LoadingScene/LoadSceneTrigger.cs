using UnityEngine;

public class LoadSceneTrigger : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            LoadingSceneController.Instance.LoadScene(sceneName);
    }
}
