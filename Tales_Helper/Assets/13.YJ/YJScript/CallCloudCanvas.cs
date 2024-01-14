using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallCloudCanvas : MonoBehaviour
{
    public Animator anim;

    public void PlayCloudAnim()
    {
        anim.SetBool("isHide", true);
        Invoke("OpenCloudAnim", 1.8f);
    }

    void OpenCloudAnim()
    {
        anim.SetBool("isHide", false);
    }
}
