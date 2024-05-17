using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedroomSceneChanger : MonoBehaviour
{
    //ħ�ǿ��� �ŽǷ� �� �̵�, ������ ������ ���� �� �̵�

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
