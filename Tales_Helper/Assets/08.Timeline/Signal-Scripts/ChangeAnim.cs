using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnim : MonoBehaviour
{
    private Animator anim;

    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        {
            anim = GetComponent<Animator>();
        }
    }

    public void TChangeAnim()
    {
        anim.SetTrigger("isChange");
    }

    public void CameraShakeStart()
    {
        Debug.Log("CameraShake Play");

        CameraShakeManager.Instance.SetShakeDegree(1.5f, 1.5f);
        CameraShakeManager.Instance.SetShakeTime(8.8f);
    }

    public void sceneChange()
    {
        LoadingSceneController.Instance.LoadScene(sceneName);
    }
}
