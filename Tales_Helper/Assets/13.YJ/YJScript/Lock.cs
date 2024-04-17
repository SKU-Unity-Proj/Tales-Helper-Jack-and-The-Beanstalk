using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
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
                rigid.useGravity = true;
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
