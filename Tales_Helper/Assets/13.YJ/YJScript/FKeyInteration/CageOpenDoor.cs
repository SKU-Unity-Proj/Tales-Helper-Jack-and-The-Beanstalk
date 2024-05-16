using UnityEngine;
using static IFKeyInteractable;

public class CageOpenDoor : MonoBehaviour, IFInteractable
{
    public float canDistance = 3f; // 상호작용 가능한 거리

    private Animator anim;
    public Animator lockAnim;

    public GameObject flashLight;
    public GameObject duck;

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

    public void Interact(float distance)
    {
        if(distance < canDistance)
        {
            if (isLocked)
            {
                anim.CrossFadeInFixedTime("CageOpen", 0f);
                isPlay = true;
                isLocked = false;

                flashLight.SetActive(false);
                duck.layer = 9;
            }
        }
    }
}
