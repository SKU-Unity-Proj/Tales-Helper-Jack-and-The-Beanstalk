using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeSpeed : MonoBehaviour
{
    public cellerGiant tracerange;

    public float increaseRange = 20f; // �÷��̾ ������ ���� ���ǵ�

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tracerange.Speed = increaseRange;
        }
    }

}
