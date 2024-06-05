using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSCameraDriven : MonoBehaviour
{
    private CameraTriggerManager cameraTriggerManager;

    void Start()
    {
        // CameraTriggerManager �ν��Ͻ��� ã�Ƽ� cameraTriggerManager ������ �Ҵ�
        // �̸� ����, �� ��ũ��Ʈ�� ī�޶� �� ������ ������ �� ����
        cameraTriggerManager = FindObjectOfType<CameraTriggerManager>();
    }

    // ������Ʈ�� Ʈ���� �ݶ��̴��� ������ �� �ڵ����� ȣ��
    private void OnTriggerEnter(Collider other)
    {
        //ī�޶��� �±װ� ������ ����X
        if (other.CompareTag("CameraZone"))
        {
            // ���� �浹�� �ݶ��̴��� �ش��ϴ� ī�޶� ���� ������
            var cameraZone = cameraTriggerManager.GetCameraZoneFromCollider(other);

            // �̴� �ش� ī�޶� ���� �������� ��, �� ī�޶� Ȱ��ȭ�ϴ� �� ���
            if (cameraZone != null)
            {
                cameraZone.camera.Priority = 11;
            }
        }
        else
            return;
    }

    // ������Ʈ�� Ʈ���� �ݶ��̴����� ��� �� �ڵ����� ȣ��
    private void OnTriggerExit(Collider other)
    {
        //ī�޶��� �±װ� ������ ����X
        if (other.CompareTag("CameraZone"))
        {
            // ���� �浹�� �ݶ��̴��� �ش��ϴ� ī�޶� ���� ������
            var cameraZone = cameraTriggerManager.GetCameraZoneFromCollider(other);

            // �̴� �ش� ī�޶� ���� �������� ��, �� ī�޶� Ȱ��ȭ�ϴ� �� ���
            if (cameraZone != null)
            {
                cameraZone.camera.Priority = 0;
            }
        }
        else
            return;
    }
}