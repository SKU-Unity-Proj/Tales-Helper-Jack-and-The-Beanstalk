using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCam : MonoBehaviour
{

    public Transform player; // �÷��̾� Transform

    void Update()
    {
        // �� ������Ʈ�� ��ġ�� �������� �ʰ�, ���� �÷��̾ �ٶ󺾴ϴ�.
        transform.LookAt(player);
    }
}
