using UnityEngine;

public class Lock : MonoBehaviour
{
    public Animator PadLockAnim;
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
                PadLockAnim.SetTrigger("Unlock");
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
}
