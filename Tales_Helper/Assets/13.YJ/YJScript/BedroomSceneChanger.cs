using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedroomSceneChanger : MonoBehaviour
{
    //침실에서 거실로 씬 이동, 거위가 있을시 엔딩 씬 이동

    public Transform targetPos;
    public string sceneName;

    private void OnCollisionEnter(Collision collision)
    {
        if(targetPos.childCount != 0)
        {
            LoadingSceneController.Instance.LoadScene("endingScene");
        }
        else
        {
            LoadingSceneController.Instance.LoadScene(sceneName);
        }
    }
}
