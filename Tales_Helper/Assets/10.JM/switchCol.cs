using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.IK;

public class switchCol : MonoBehaviour
{

    // _ikScheduler의 타입에 맞게 변수를 선언하세요.
    private IKScheduler _ikScheduler;

    private void OnTriggerEnter(Collider other)
    {
        // 'switchCol' 태그를 가진 오브젝트와 부딪혔는지 확인합니다.
        if (other.gameObject.CompareTag("switchCol"))
        {
            // IK를 중지하는 함수를 호출합니다.
            _ikScheduler.StopIK(AvatarIKGoal.LeftHand);
            _ikScheduler.StopIK(AvatarIKGoal.RightHand);
        }
    }
}
