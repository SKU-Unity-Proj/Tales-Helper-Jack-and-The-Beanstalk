using UnityEngine;

public class ElevatorJumpAndFall : MonoBehaviour
{
    private Animator elevatorAnim;

    public OriginCameraShake originCameraShake;

    private void Start()
    {
        elevatorAnim = GetComponent<Animator>();
    }

    public void JumpCountUp()
    {
        //jumpCount++;
        if (!elevatorAnim.GetBool("isJump"))
        {
            elevatorAnim.SetBool("isJump", true);
            originCameraShake.OnShakeCamera();
        }
        else
        {
            elevatorAnim.SetBool("isJump", false);
            originCameraShake.OnShakeCamera();
        }
    }
}
