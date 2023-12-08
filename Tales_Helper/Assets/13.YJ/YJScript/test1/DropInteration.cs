using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInteration : MonoBehaviour
{
    private Animator anim;
    private Rigidbody playerRigidbody;
    private float radius = 1;

    public float pullForce = 5f;


    void Start()
    {
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        PullObj();
    }

    void PullObj()
    {
        if (Input.GetKey(KeyCode.F)) 
        {
            Collider[] colliders =
                        Physics.OverlapSphere(this.transform.position, radius);

            foreach (Collider col in colliders)
            {
                if (col.gameObject.tag == "PullObject")
                {
                    anim.SetBool("PullObject", true);

                    Vector3 PullDirection = (transform.forward).normalized;
                    playerRigidbody.AddForce(PullDirection * -pullForce);

                    Rigidbody PullRigid = col.GetComponent<Rigidbody>();
                    PullRigid.AddForce(PullDirection * -pullForce,ForceMode.Force);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            anim.SetBool("PullObject", false);
        }
    }
}
