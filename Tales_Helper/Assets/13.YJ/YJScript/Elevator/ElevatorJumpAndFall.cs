using UnityEngine;

public class ElevatorJumpAndFall : MonoBehaviour
{
    public Animator elevatorAnim;
    private int jumpCount = 0;

    private void Update()
    {
        if (jumpCount == 1)
            elevatorAnim.SetBool("isJump",true);
        else if (jumpCount == 2)
            elevatorAnim.SetBool("isJump", false);
        else if (jumpCount == 3)
        {
            elevatorAnim.SetBool("isJump", true);
            //this.gameObject.SetActive(false);
        }  
    }

    void OnTriggerEnter(Collider col)
    {
        Invoke("JumpCountUp", 0.38f);
    }

    void JumpCountUp()
    {
        jumpCount++;
    }
}
