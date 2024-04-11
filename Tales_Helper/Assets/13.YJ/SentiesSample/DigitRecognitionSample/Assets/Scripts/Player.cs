using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �÷��̾ ó���մϴ�.
 *  - �÷��̾ ��ü�� �� �� �ֽ��ϴ�.
 *  - �÷��̾ Rigidbody�� �浹�ϸ� �ణ�� �ӵ��� �����մϴ�.
 */
public class Player : MonoBehaviour
{
    // �� ��ũ��Ʈ�� ĳ���Ͱ� ������ ��� RigidBody�� �о���ϴ�.
    float pushPower = 2.0f; // �б� ����

    // ĳ���� ��Ʈ�ѷ��� �浹���� �� ȣ��Ǵ� �Լ�
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody; // �浹�� ��ü�� Rigidbody�� �����ɴϴ�.
        if (body == null || body.isKinematic)
        {
            return; // Rigidbody�� ���ų� Kinematic�̸� �Լ��� �����մϴ�.
        }

        // �Ʒ��� Ǫ������ �ʵ��� �մϴ�.
        if (hit.moveDirection.y < -0.3)
        {
            return; // Ǫ�� ������ �Ʒ������� ���� ��� �Լ��� �����մϴ�.
        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z); // �б� ������ �������� �����մϴ�.
        body.velocity = pushDir * pushPower; // Rigidbody�� �б⸦ �����Ͽ� �ӵ��� �����մϴ�.
    }
}
