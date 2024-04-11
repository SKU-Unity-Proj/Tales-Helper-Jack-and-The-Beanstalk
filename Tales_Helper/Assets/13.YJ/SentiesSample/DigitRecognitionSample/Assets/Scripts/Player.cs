using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어를 처리합니다.
 *  - 플레이어가 물체를 밀 수 있습니다.
 *  - 플레이어가 Rigidbody에 충돌하면 약간의 속도를 제공합니다.
 */
public class Player : MonoBehaviour
{
    // 이 스크립트는 캐릭터가 접촉한 모든 RigidBody를 밀어냅니다.
    float pushPower = 2.0f; // 밀기 강도

    // 캐릭터 컨트롤러가 충돌했을 때 호출되는 함수
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody; // 충돌한 객체의 Rigidbody를 가져옵니다.
        if (body == null || body.isKinematic)
        {
            return; // Rigidbody가 없거나 Kinematic이면 함수를 종료합니다.
        }

        // 아래로 푸쉬하지 않도록 합니다.
        if (hit.moveDirection.y < -0.3)
        {
            return; // 푸쉬 방향이 아래쪽으로 가는 경우 함수를 종료합니다.
        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z); // 밀기 방향을 수평으로 설정합니다.
        body.velocity = pushDir * pushPower; // Rigidbody에 밀기를 적용하여 속도를 변경합니다.
    }
}
