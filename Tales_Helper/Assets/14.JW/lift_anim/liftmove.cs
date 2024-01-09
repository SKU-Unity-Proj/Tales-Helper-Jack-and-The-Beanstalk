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

    private bool check1F = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        leftanim = leftdoor.GetComponent<Animator>();
        rightanim = rightdoor.GetComponent<Animator>();

    }

    // Update is called once per frame
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {

            CloseDoor();

            Invoke("Startlift", 3.0f);
            

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
        if (check1F == false)
        {
            Invoke("OpenDoor", 4.0f);
            anim.SetTrigger("isDown");
            check1F = true;
        }
        else
        {
            Invoke("OpenDoor", 4.0f);
            anim.SetTrigger("isUp");
            check1F = false;
        }
    }

}
