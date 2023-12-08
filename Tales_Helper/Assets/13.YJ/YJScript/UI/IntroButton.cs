using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroButton : MonoBehaviour
{
    public void StartClick()
    {
        LoadingScene.LoadScene("test1");
    }
}
