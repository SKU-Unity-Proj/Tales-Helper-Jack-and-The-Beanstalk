using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCam : MonoBehaviour
{

    public Transform player; // �÷��̾� Transform
    public float xOffset = 0f; // X�� ������

    private Vector3 followPosition;

    void Update()
    {
        // �÷��̾��� Y, Z ��ġ�� ���󰡵�, X ��ġ�� ����
        followPosition.x = xOffset; // X���� ���� ������ ���
        followPosition.y = player.position.y;
        followPosition.z = player.position.z;

        transform.position = followPosition;
    }
}
