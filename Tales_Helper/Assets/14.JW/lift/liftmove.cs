using DiasGames;
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
    //애니메이션을 실행하기위함

    private bool check1F = false; //엘레베이터 위치를 판별하기 위함

    public MeshRenderer lightMeshRenderer; // 엘베 표시등

    public AbilityScheduler abilityScheduler; // 캐릭터 움직임 

    void Start()
    {
        anim = GetComponent<Animator>();
        leftanim = leftdoor.GetComponent<Animator>(); //leftdoor오브젝트에 애니메이터 컴포넌트추가
        rightanim = rightdoor.GetComponent<Animator>(); //rightdoor오브젝트에 애니메이터 컴포넌트추가
    }

    private void OnEnable()
    {
        // 엘베 표시등 켜기
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
                Debug.LogWarning("엘베 라이트 오류2");
            }
        }
        else
        {
            Debug.LogWarning("엘베 라이트 오류1");
        }
    }

    void OnTriggerEnter(Collider col) 
    {
        if (col.CompareTag("Player")) //플레이어가 엘베에 탑승했을 때
        {

            CloseDoor(); 

            Invoke("Startlift", 4.0f);
            

        }
    }

    void CloseDoor() 
    {
        leftanim.SetBool("isClose", true);
        rightanim.SetBool("isClose", true);

    }
    void OpenDoor()
    {
        abilityScheduler.enabled = true;
        leftanim.SetBool("isClose", false);
        rightanim.SetBool("isClose", false);
    }
    void Startlift()
    {
        if (check1F == false) //지상일 때를 판별
        {
            abilityScheduler.enabled = false;
            Invoke("OpenDoor", 16.0f);
            anim.SetTrigger("isDown"); 
            check1F = true;
        }
        else //지하일 때를 판별
        {
            abilityScheduler.enabled = false;
            Invoke("OpenDoor", 16.0f);
            anim.SetTrigger("isUp");
            check1F = false;
        }
    }

}
