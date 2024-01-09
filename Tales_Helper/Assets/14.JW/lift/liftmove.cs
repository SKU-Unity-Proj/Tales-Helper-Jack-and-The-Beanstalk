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
    //애니메이션을 실행하기위함

    private bool check1F = false; //엘레베이터 위치를 판별하기 위함
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        leftanim = leftdoor.GetComponent<Animator>(); //leftdoor오브젝트에 애니메이터 컴포넌트추가
        rightanim = rightdoor.GetComponent<Animator>(); //rightdoor오브젝트에 애니메이터 컴포넌트추가

    }

    // Update is called once per frame
    void OnTriggerEnter(Collider col) 
    {
        if (col.CompareTag("Player")) //플레이어가 엘베에 탑승했을 때
        {

            CloseDoor(); 

            Invoke("Startlift", 3.0f); //3초뒤 Startlift() 실행
            

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
        if (check1F == false) //지상일 때를 판별
        {
            Invoke("OpenDoor", 4.0f); //4초뒤 OpenDoor()실행
            anim.SetTrigger("isDown"); 
            check1F = true; //기본값인 true로 돌아옴
        }
        else //지하일 때를 판별
        {
            Invoke("OpenDoor", 4.0f); //4초뒤 OpenDoor()실행
            anim.SetTrigger("isUp");
            check1F = false; //기본값인 false로 돌아옴
        }
    }

}
