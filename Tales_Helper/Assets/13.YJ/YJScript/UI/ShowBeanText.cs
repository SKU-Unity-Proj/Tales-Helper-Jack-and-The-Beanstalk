using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XEntity.InventoryItemSystem
{
    public class ShowBeanText : MonoBehaviour  //�� �ɴ� �� �������� �� Text�� ����ִ� ��ũ��Ʈ BeanTalkCollider�� ����
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