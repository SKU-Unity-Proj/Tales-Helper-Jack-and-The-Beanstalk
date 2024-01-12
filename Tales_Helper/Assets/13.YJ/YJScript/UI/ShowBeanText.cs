using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//콩 심는 곳 도착했을 때 Text를 띄워주는 스크립트 BeanTalkCollider에 부착
public class ShowBeanText : MonoBehaviour
{
    public Animator talkPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine("PanelUpDown");
        }
    }

    IEnumerator PanelUpDown()
    {
        talkPanel.SetBool("isShow", true);

        yield return new WaitForSeconds(2.5f);

        talkPanel.SetBool("isShow", false);
    }
}
