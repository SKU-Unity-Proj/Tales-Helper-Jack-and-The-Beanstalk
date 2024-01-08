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
        if (Input.GetKeyDown(KeyCode.E) && isInside) // E Ű�� ���������� �۵�
        {
            StartCoroutine(ElevatorSequence());
        }
    }

    IEnumerator ElevatorSequence()
    {
        isMoving = true;
        //player.GetComponent<PlayerMovement>().enabled = false; // �÷��̾� ������ ����
        elevatorAnimator.SetTrigger("Animation_door_L"); // �� ���� �ִϸ��̼�
        elevatorAnimator.SetTrigger("Animation_door_R"); // �� ���� �ִϸ��̼�
        yield return new WaitForSeconds(1); // �ִϸ��̼� �ð���ŭ ���
        elevatorAnimator.SetTrigger("Animation_lift"); // ���������� �̵� �ִϸ��̼�
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) // �÷��̾ ���������� �ȿ� ������ ��
        {
            isInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) // �÷��̾ ���������͸� ������ ��
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
        elevatorAnimator.SetTrigger("Animation_door_L"); // �� ���� �ִϸ��̼�
        elevatorAnimator.SetTrigger("Animation_door_R"); // �� ���� �ִϸ��̼�
        yield return new WaitForSeconds(1); // �ִϸ��̼� �ð���ŭ ���
        //isMoving = false;
        //player.GetComponent<PlayerMovement>().enabled = true; // �÷��̾� ������ ���
    }
}