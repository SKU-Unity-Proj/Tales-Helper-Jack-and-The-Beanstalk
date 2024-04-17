using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellarFollowCam : MonoBehaviour
{

    public Transform player; // 플레이어 Transform
    public float yOffset = 0f; // X축 오프셋

    private Vector3 followPosition;

    void Update()
    {
        // 플레이어의 Y, Z 위치를 따라가되, X 위치는 고정
        followPosition.x = player.position.x;
        followPosition.y = yOffset;
        followPosition.z = player.position.z;

        transform.position = followPosition;
    }
}
