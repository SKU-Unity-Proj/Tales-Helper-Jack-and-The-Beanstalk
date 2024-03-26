using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.IK;

public class switchCol : MonoBehaviour
{

    // _ikScheduler�� Ÿ�Կ� �°� ������ �����ϼ���.
    private IKScheduler _ikScheduler;

    private void OnTriggerEnter(Collider other)
    {
        // 'switchCol' �±׸� ���� ������Ʈ�� �ε������� Ȯ���մϴ�.
        if (other.gameObject.CompareTag("switchCol"))
        {
            // IK�� �����ϴ� �Լ��� ȣ���մϴ�.
            _ikScheduler.StopIK(AvatarIKGoal.LeftHand);
            _ikScheduler.StopIK(AvatarIKGoal.RightHand);
        }
    }
}
