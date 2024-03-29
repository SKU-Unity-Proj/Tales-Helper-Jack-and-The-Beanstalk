using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftControl : MonoBehaviour
{
    public int touchingObjects = 0; // 현재 닿고 있는 오브젝트의 수
    public Animator anim_Box;
    public Animator anim_Bucket;

    void Update()
    {
        switch (touchingObjects)
        {
            case 0:
                anim_Box.SetInteger("haveObject", 0);
                anim_Bucket.SetInteger("haveObject", 0);
                break;
            case 1:
                anim_Box.SetInteger("haveObject", 1);
                anim_Bucket.SetInteger("haveObject", 1);
                break;
            case 2:
                anim_Box.SetInteger("haveObject", 2);
                anim_Bucket.SetInteger("haveObject", 2);
                break;
            default:
                anim_Box.SetInteger("haveObject", 2);
                anim_Bucket.SetInteger("haveObject", 2);
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        touchingObjects++;
        if (touchingObjects > 2)
            touchingObjects = 2;

        Debug.Log("ItemIn");
    }

    private void OnTriggerExit(Collider other)
    {
        touchingObjects--;

        Debug.Log("ItemOut");
    }
}
