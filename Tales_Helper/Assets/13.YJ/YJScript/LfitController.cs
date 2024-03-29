using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LfitController : MonoBehaviour
{
    public float maxHeight = 5f; // �ִ� ����
    public float minHeight = 1f; // ���� ���� ����
    public float midHeight; // �߰� ����

    public float movementSpeed = 1f; // �̵� �ӵ�

    public int targetHeightIndex = 0; // ��ǥ ���� �ε���
    private int touchingObjects = 0; // ���� ��� �ִ� ������Ʈ�� ��

    private void Update()
    {
        // ��ǥ ���̿� ���� ������ ó��
        float targetHeight;
        switch (targetHeightIndex)
        {
            case 0:
                targetHeight = maxHeight;
                break;
            case 1:
                targetHeight = midHeight;
                break;
            case 2:
                targetHeight = minHeight;
                break;
            default:
                targetHeight = maxHeight;
                break;
        }

        // ��ǥ ���̱��� õõ�� �̵�
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetHeight, transform.position.z), step);
    }

    private void OnCollisionEnter(Collision collision)
    {
        touchingObjects++;
        UpdateTargetHeightIndex();
    }

    private void OnCollisionExit(Collision collision)
    {
        touchingObjects--;
        UpdateTargetHeightIndex();
    }

    private void UpdateTargetHeightIndex()
    {
        if (touchingObjects == 1)
            targetHeightIndex = 1;

        else if(touchingObjects == 2)
            targetHeightIndex = 2;

        else
            targetHeightIndex = 0;
    }
}
