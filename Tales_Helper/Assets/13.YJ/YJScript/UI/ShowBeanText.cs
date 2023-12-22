using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
