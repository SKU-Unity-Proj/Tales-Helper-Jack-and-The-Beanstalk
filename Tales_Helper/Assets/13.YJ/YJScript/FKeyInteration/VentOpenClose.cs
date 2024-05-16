using UnityEngine;
using static IFKeyInteractable;

public class VentOpenClose : MonoBehaviour, IFInteractable
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (anim != null)
        {
            if(!anim.GetBool("isOpen"))
            {
                anim.SetBool("isOpen", true);
            }
            else
            {
                anim.SetBool("isOpen", false);
            }
        }
    }
}