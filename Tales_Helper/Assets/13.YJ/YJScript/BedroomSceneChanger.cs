using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedroomSceneChanger : MonoBehaviour
{
    //ħ�ǿ��� �ŽǷ� �� �̵�, ������ ������ ���� �� �̵�

    public Transform targetPos;
    public string sceneName = "GiantMap";
    public string endingSceneName = "EndingAnimation";

    private void OnCollisionEnter(Collision collision)
    {
        if(targetPos.childCount != 0)
        {
            LoadingSceneController.Instance.LoadScene(endingSceneName);
        }
        else
        {
            LoadingSceneController.Instance.LoadScene(sceneName);
        }
    }
}
