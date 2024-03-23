using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCam : MonoBehaviour
{

    public Transform player; // 플레이어 Transform

    void Update()
    {
        // 빈 오브젝트는 위치를 변경하지 않고, 오직 플레이어를 바라봅니다.
        transform.LookAt(player);
    }
}
