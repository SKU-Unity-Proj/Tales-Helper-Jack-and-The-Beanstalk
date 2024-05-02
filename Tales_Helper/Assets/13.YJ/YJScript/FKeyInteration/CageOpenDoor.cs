using UnityEngine;
using static IFKeyInteractable;

public class CageOpenDoor : MonoBehaviour, IFInteractable
{
    private Animator anim;
    public Animator lockAnim;

    private bool isPlay = false;
    private bool isLocked = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(!isPlay)
            if (lockAnim.GetCurrentAnimatorStateInfo(0).IsName("Lock_Unlock"))
            {
                Debug.Log("unlock");
                isPlay = true;
                isLocked = true;
            } 
    }

    public void Interact()
    {
        Debug.Log("Open");
        if (isLocked)
        {
            anim.CrossFadeInFixedTime("CageOpen", 0f);
            isPlay = true;
            isLocked = false;
        }
    }
}
