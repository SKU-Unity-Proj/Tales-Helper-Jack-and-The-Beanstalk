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
            //giants[i].index = i; // 각 거인에 인덱스 할당
            //청소위치
            giants[i].SetCleaningPosition(cleaningPositions[i % cleaningPositions.Count]);
            giants[i].manager = this;  // 각 거인에 메니저 참조를 설정
        }
    }

    // 청소 위치의 방향 정보를 가져오는 메서드
    public Vector3 GetCleaningDirection(int index, int giantIndex)
    {
        Transform cleaningPos = cleaningPositions[index];
        Vector3 direction;
        switch (giantIndex)
        {
            case 0:
                direction = -cleaningPos.right;
                break;
            case 1:
                direction = cleaningPos.right;
                break;
            case 2:
                direction = cleaningPos.forward;
                break;
            default:
                direction = cleaningPos.forward;
                break;
        }
        Debug.Log("Direction for giant " + giantIndex + " at cleaning position " + index + ": " + direction);
        return direction;
    }
}
