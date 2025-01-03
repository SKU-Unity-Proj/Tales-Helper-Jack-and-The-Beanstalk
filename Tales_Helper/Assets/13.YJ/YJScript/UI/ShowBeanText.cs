using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.Controller;

namespace XEntity.InventoryItemSystem
{
    public class ShowBeanText : MonoBehaviour  //콩 심는 곳 도착했을 때 Text를 띄워주는 스크립트 BeanTalkCollider에 부착
    {
        public Animator talkPanel;
        public GameObject beanStalk;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine("PanelUpDown");
                ItemManager.Instance.canPlant = true;
                beanStalk.SetActive(true);

                other.GetComponent<CSPlayerController>().enabled = false;
                other.GetComponent<Animator>().enabled = false;
            }
        }

        IEnumerator PanelUpDown()
        {
            talkPanel.SetBool("isShow", true);

            yield return new WaitForSeconds(2.5f);

            this.gameObject.SetActive(false);
            talkPanel.SetBool("isShow", false);
        }
    }
}