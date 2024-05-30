using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liftmove : MonoBehaviour
{
    public GameObject leftdoor; 
    public GameObject rightdoor;

    private Animator anim;
    private Animator leftanim;
    private Animator rightanim;
    //�ִϸ��̼��� �����ϱ�����

    private bool check1F = false; //���������� ��ġ�� �Ǻ��ϱ� ����

    public MeshRenderer lightMeshRenderer; // ���� ǥ�õ�

    void Start()
    {
        anim = GetComponent<Animator>();
        leftanim = leftdoor.GetComponent<Animator>(); //leftdoor������Ʈ�� �ִϸ����� ������Ʈ�߰�
        rightanim = rightdoor.GetComponent<Animator>(); //rightdoor������Ʈ�� �ִϸ����� ������Ʈ�߰�
    }

    private void OnEnable()
    {
        // ���� ǥ�õ� �ѱ�
        if (lightMeshRenderer != null)
        {
            Material[] materials = lightMeshRenderer.materials;

            if (materials.Length > 1)
            {
                materials[2] = null;

                lightMeshRenderer.materials = materials;
            }
            else
            {
                Debug.LogWarning("���� ����Ʈ ����2");
            }
        }
        else
        {
            Debug.LogWarning("���� ����Ʈ ����1");
        }
    }

    void OnTriggerEnter(Collider col) 
    {
        if (col.CompareTag("Player")) //�÷��̾ ������ ž������ ��
        {

            CloseDoor(); 

            Invoke("Startlift", 4.0f); //4�ʵ� Startlift() ����
            

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
            Invoke("OpenDoor", 10.0f); //10�ʵ� OpenDoor()����
            anim.SetTrigger("isDown"); 
            check1F = true; //�⺻���� true�� ���ƿ�
        }
        else //������ ���� �Ǻ�
        {
            Invoke("OpenDoor", 10.0f); //10�ʵ� OpenDoor()����
            anim.SetTrigger("isUp");
            check1F = false; //�⺻���� false�� ���ƿ�
        }
    }

}
