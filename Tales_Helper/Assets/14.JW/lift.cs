/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class lift : MonoBehaviour
{
    public PlayableDirector timeline;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            timeline.Play();
        }
    }
}*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lift : MonoBehaviour
{
    public Animator elevatorAnimator;
    public GameObject player;
    private bool isInside = false;
    private bool isMoving = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isInside) // E 키로 엘레베이터 작동
        {
            StartCoroutine(ElevatorSequence());
        }
    }

    IEnumerator ElevatorSequence()
    {
        isMoving = true;
        //player.GetComponent<PlayerMovement>().enabled = false; // 플레이어 움직임 제한
        elevatorAnimator.SetTrigger("Animation_door_L"); // 문 열기 애니메이션
        elevatorAnimator.SetTrigger("Animation_door_R"); // 문 열기 애니메이션
        yield return new WaitForSeconds(1); // 애니메이션 시간만큼 대기
        elevatorAnimator.SetTrigger("Animation_lift"); // 엘레베이터 이동 애니메이션
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) // 플레이어가 엘레베이터 안에 들어왔을 때
        {
            isInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) // 플레이어가 엘레베이터를 나갔을 때
        {
            isInside = false;
        }
    }

    public void ElevatorStop()
    {
        StartCoroutine(StopSequence());
    }

    IEnumerator StopSequence()
    {
        elevatorAnimator.SetTrigger("Animation_door_L"); // 문 열기 애니메이션
        elevatorAnimator.SetTrigger("Animation_door_R"); // 문 열기 애니메이션
        yield return new WaitForSeconds(1); // 애니메이션 시간만큼 대기
        //isMoving = false;
        //player.GetComponent<PlayerMovement>().enabled = true; // 플레이어 움직임 허용
    }
}