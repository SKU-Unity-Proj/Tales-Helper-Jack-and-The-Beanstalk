using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellarFollowCam : MonoBehaviour
{

    public Transform player; // �÷��̾� Transform
    public float yOffset = 0f; // X�� ������

    private Vector3 followPosition;

    void Update()
    {
        // �÷��̾��� Y, Z ��ġ�� ���󰡵�, X ��ġ�� ����
        followPosition.x = player.position.x;
        followPosition.y = yOffset;
        followPosition.z = player.position.z;

        transform.position = followPosition;
    }
}
