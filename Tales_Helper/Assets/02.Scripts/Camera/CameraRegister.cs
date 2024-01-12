using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRegister : MonoBehaviour
{
    // ������Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �޼ҵ�
    private void OnEnable()
    {
        // �� ���� ������Ʈ�� �ִ� CinemachineVirtualCamera ������Ʈ�� CameraSwitcher�� ���
        CameraSwitcher.Register(GetComponent<CinemachineVirtualCamera>());
    }

    // ������Ʈ�� ��Ȱ��ȭ�� �� ȣ��Ǵ� �޼ҵ�
    private void OnDisable()
    {
        // �� ���� ������Ʈ�� �ִ� CinemachineVirtualCamera ������Ʈ�� CameraSwitcher���� ����
        CameraSwitcher.UnRegister(GetComponent<CinemachineVirtualCamera>());
    }
}
