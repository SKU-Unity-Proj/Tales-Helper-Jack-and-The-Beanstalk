using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCam : MonoBehaviour
{

    public Transform player; // �÷��̾� Transform
    public float xOffset = 0f; // X�� ������
    public float yOffset = 0f; // Y�� ������

    private Vector3 followPosition;

    void Update()
    {
        // �÷��̾��� Z ��ġ�� ���󰡵�, X�� Y ��ġ�� ����
        followPosition.x = xOffset; // X���� ���� ������ ���
        followPosition.y = yOffset; // Y���� ���� ������ ���
        followPosition.z = player.position.z;

        transform.position = followPosition;
    }
}
