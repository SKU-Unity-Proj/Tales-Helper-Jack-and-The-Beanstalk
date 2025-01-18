using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftControl : MonoBehaviour
{
    public Animator anim_Box;
    public Animator anim_Bucket;

    private void OnTriggerEnter(Collider other)
    {
        anim_Bucket.SetBool("Weight", true);
        anim_Box.SetBool("Weight", true);
    }

    private void OnTriggerExit(Collider other)
    {
        anim_Bucket.SetBool("Weight", false);
        anim_Box.SetBool("Weight", false);
    }
}
