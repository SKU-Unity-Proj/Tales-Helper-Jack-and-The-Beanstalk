using UnityEngine;
using System.Collections.Generic;

public class kitchenGiantManager : MonoBehaviour
{
    public List<kitchenGiant> giants;
    public List<Transform> cleaningPositions;

    void Start()
    {
        // 초기 청소 위치 할당
        for (int i = 0; i < giants.Count; i++)
        {
            giants[i].SetCleaningPosition(cleaningPositions[i % cleaningPositions.Count]);
        }
    }
}
