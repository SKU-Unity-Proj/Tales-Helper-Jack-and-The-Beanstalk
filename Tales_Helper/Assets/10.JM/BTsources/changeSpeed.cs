using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeSpeed : MonoBehaviour
{
    public cellerGiant tracerange;

    public float increaseRange = 15f; // 플레이어가 들어왔을 때의 스피드

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tracerange.Speed = increaseRange;
        }
    }

}
