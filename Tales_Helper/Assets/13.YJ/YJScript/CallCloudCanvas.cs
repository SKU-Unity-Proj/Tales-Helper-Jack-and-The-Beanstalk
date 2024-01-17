using UnityEngine;

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
