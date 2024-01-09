using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class liftmove : MonoBehaviour
{
    public GameObject leftdoor; 
    public GameObject rightdoor;

    private Animator anim;
    private Animator leftanim;
    private Animator rightanim;
    //�ִϸ��̼��� �����ϱ�����

    private bool check1F = false; //���������� ��ġ�� �Ǻ��ϱ� ����
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        leftanim = leftdoor.GetComponent<Animator>(); //leftdoor������Ʈ�� �ִϸ����� ������Ʈ�߰�
        rightanim = rightdoor.GetComponent<Animator>(); //rightdoor������Ʈ�� �ִϸ����� ������Ʈ�߰�

    }

    // Update is called once per frame
    void OnTriggerEnter(Collider col) 
    {
        if (col.CompareTag("Player")) //�÷��̾ ������ ž������ ��
        {

            CloseDoor(); 

            Invoke("Startlift", 3.0f); //3�ʵ� Startlift() ����
            

        }
    }

    void CloseDoor() 
    {
        leftanim.SetBool("isClose", true);
        rightanim.SetBool("isClose", true);

    }
    void OpenDoor()
    {
        leftanim.SetBool("isClose", false);
        rightanim.SetBool("isClose", false);
    }
    void Startlift()
    {
        if (check1F == false) //������ ���� �Ǻ�
        {
            Invoke("OpenDoor", 4.0f); //4�ʵ� OpenDoor()����
            anim.SetTrigger("isDown"); 
            check1F = true; //�⺻���� true�� ���ƿ�
        }
        else //������ ���� �Ǻ�
        {
            Invoke("OpenDoor", 4.0f); //4�ʵ� OpenDoor()����
            anim.SetTrigger("isUp");
            check1F = false; //�⺻���� false�� ���ƿ�
        }
    }

}
