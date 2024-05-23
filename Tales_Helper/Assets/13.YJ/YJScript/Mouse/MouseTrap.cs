using UnityEngine;

public class MouseTrap : MonoBehaviour
{
    public GameObject giant;
    public GameObject mouse;
    public GameObject cheese;
    public Transform targetPos;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == cheese)
        {
            other.transform.position = targetPos.position;
            Rigidbody cheeseRigid = other.GetComponent<Rigidbody>();
            cheeseRigid.angularVelocity = Vector3.zero;
            cheeseRigid.velocity = Vector3.zero;

            mouse.GetComponent<MouseController>().MoveToTarget(targetPos);
        }

        if (other.gameObject == mouse)
        {
            anim.SetTrigger("isCatch");
            mouse.GetComponent<MouseController>().Die();
            cheese.SetActive(false);
            giant.GetComponent<RodolfoFSM>().ChangeState(RodolfoFSM.State.MOVE);
        }
    }
}
