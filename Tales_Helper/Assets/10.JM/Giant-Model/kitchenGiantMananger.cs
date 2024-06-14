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
            //giants[i].index = i; // �� ���ο� �ε��� �Ҵ�
            //û����ġ
            giants[i].SetCleaningPosition(cleaningPositions[i % cleaningPositions.Count]);
            giants[i].manager = this;  // �� ���ο� �޴��� ������ ����
        }
    }

    // û�� ��ġ�� ���� ������ �������� �޼���
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
