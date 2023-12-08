using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VCameraManager : MonoBehaviour
{
    //pulic 메인 카메라 받아오기
    //private 바꿔줄 카메라들 담아줄 곳[]

    void Start()
    {
        //FindOfTag 로 카메라 태그 달린 애들 [] <- 여기에 전부 담기
    }

    void OnTriggerEnter(Collider other) //콜라이더 들어갈때
    {
        if (other.name == "CS Character Controller")
        {
            Debug.Log("a");
            //거실 카메라.MoveToTopOfPrioritySubqueue();
            //거실 카메라.Priority = 11; //카메라 순위 올려주기
        }

        if (this.name == "b" && other.name == "CS Character Controller")
        {
            Debug.Log("b");
            //주방 카메라.MoveToTopOfPrioritySubqueue();
            //주방 카메라.Priority = 11; 
        }
    }

    void OnTriggerExit(Collider other) //콜라이더 나갈때
    {
        if (this.name == "거실 콜라이더" && other.name == "플레이어")
        {
            //메인 카메라.MoveToTopOfPrioritySubqueue();
            //거실 카메라.Priority = 1; //카메라 순위 낮추기
        }

        if (this.name == "주방 콜라이더" && other.name == "플레이어")
        {
            //메인 카메라.MoveToTopOfPrioritySubqueue();
            //주방 카메라.Priority = 1;
        }
    }
}
