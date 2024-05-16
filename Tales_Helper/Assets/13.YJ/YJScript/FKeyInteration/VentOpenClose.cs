using UnityEngine;
using static IFKeyInteractable;

public class VentOpenClose : MonoBehaviour, IFInteractable
{
    public float canDistance = 5f; // 상호작용 가능한 거리

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact(float distance)
    {
        if (distance < canDistance)
        {
            if (anim != null)
            {
                if (!anim.GetBool("isOpen"))
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
}