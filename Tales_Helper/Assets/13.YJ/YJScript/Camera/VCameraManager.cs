using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VCameraManager : MonoBehaviour
{
    //pulic ���� ī�޶� �޾ƿ���
    //private �ٲ��� ī�޶�� ����� ��[]

    void Start()
    {
        //FindOfTag �� ī�޶� �±� �޸� �ֵ� [] <- ���⿡ ���� ���
    }

    void OnTriggerEnter(Collider other) //�ݶ��̴� ����
    {
        if (other.name == "CS Character Controller")
        {
            Debug.Log("a");
            //�Ž� ī�޶�.MoveToTopOfPrioritySubqueue();
            //�Ž� ī�޶�.Priority = 11; //ī�޶� ���� �÷��ֱ�
        }

        if (this.name == "b" && other.name == "CS Character Controller")
        {
            Debug.Log("b");
            //�ֹ� ī�޶�.MoveToTopOfPrioritySubqueue();
            //�ֹ� ī�޶�.Priority = 11; 
        }
    }

    void OnTriggerExit(Collider other) //�ݶ��̴� ������
    {
        if (this.name == "�Ž� �ݶ��̴�" && other.name == "�÷��̾�")
        {
            //���� ī�޶�.MoveToTopOfPrioritySubqueue();
            //�Ž� ī�޶�.Priority = 1; //ī�޶� ���� ���߱�
        }

        if (this.name == "�ֹ� �ݶ��̴�" && other.name == "�÷��̾�")
        {
            //���� ī�޶�.MoveToTopOfPrioritySubqueue();
            //�ֹ� ī�޶�.Priority = 1;
        }
    }
}
