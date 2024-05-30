using UnityEngine;

public class Lock : MonoBehaviour
{
    public Animator PadLockAnim;
    public Animator Door2Anim;
    private Animator anim;
    private Rigidbody rigid;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Lock_Unlock"))
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                rigid.useGravity = true;
                if (PadLockAnim != null)
                {
                    PadLockAnim.SetTrigger("Unlock");

                    Invoke("PlayOpenDoorAnimation", 2f);
                }
                else
                    return;
            }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Key"))
        {
            anim.SetTrigger("Unlock");
            
            collision.gameObject.SetActive(false);
        }
    }

    void PlayOpenDoorAnimation()
    {
        Door2Anim.SetTrigger("Open");
    }
}
