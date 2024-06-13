using UnityEngine;
using System.Collections.Generic;

public class kitchenGiantManager : MonoBehaviour
{
    public List<kitchenGiant> giants;
    public List<Transform> cleaningPositions;

    void Start()
    {
        // �ʱ� û�� ��ġ �Ҵ�
        for (int i = 0; i < giants.Count; i++)
        {
            giants[i].SetCleaningPosition(cleaningPositions[i % cleaningPositions.Count]);
        }
    }
}
