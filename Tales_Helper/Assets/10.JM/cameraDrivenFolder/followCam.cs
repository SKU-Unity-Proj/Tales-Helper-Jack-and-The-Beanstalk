using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCam : MonoBehaviour
{

    public Transform player; // 플레이어 Transform
    public float xOffset = 0f; // X축 오프셋
    public float yOffset = 0f; // Y축 오프셋

    private Vector3 followPosition;

    void Update()
    {
        // 플레이어의 Z 위치를 따라가되, X와 Y 위치는 고정
        followPosition.x = xOffset; // X축은 고정 오프셋 사용
        followPosition.y = yOffset; // Y축은 고정 오프셋 사용
        followPosition.z = player.position.z;

        transform.position = followPosition;
    }
}
