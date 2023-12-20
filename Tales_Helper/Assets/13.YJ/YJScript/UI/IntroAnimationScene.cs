using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAnimationScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("NextScene", 53f);
    }

    void NextScene()
    {
        SceneManager.LoadScene("JackHouse");
    }
}
