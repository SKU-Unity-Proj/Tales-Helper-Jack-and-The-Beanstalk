using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSCameraDriven : MonoBehaviour
{
    private Collider currentZone; // ���� �÷��̾ �ִ� Zone
    private CameraTriggerManager cameraTriggerManager;

    void Start()
    {
        // CameraTriggerManager �ν��Ͻ��� ã�Ƽ� cameraTriggerManager ������ �Ҵ�
        // �̸� ����, �� ��ũ��Ʈ�� ī�޶� �� ������ ������ �� ����
        cameraTriggerManager = FindObjectOfType<CameraTriggerManager>();
    }

    private void FixedUpdate()
    {
        foreach (var zone in cameraTriggerManager.cameraZones)
        {
            Collider zoneCollider = zone.zoneObject.GetComponent<Collider>();

            if (zoneCollider.bounds.Contains(this.transform.position))
            {
                // �÷��̾ ���ο� ���� ������ ���
                if (currentZone != zoneCollider)
                {
                    EnterZone(zoneCollider);
                    currentZone = zoneCollider;
                }
            }
            else if (currentZone == zoneCollider)
            {
                // �÷��̾ ���� ������ ��� ���
                ExitZone(zoneCollider);
                currentZone = null;
            }
        }
    }

    private void EnterZone(Collider zone)
    {
        var cameraZone = cameraTriggerManager.GetCameraZoneFromCollider(zone);
        if (cameraZone != null)
        {
            cameraZone.camera.Priority = 11; // ī�޶� �켱���� ����
            Debug.Log($"Entered Zone: {zone.name}");
        }
    }

    private void ExitZone(Collider zone)
    {
        var cameraZone = cameraTriggerManager.GetCameraZoneFromCollider(zone);
        if (cameraZone != null)
        {
            cameraZone.camera.Priority = 0; // ī�޶� �켱���� �ʱ�ȭ
            Debug.Log($"Exited Zone: {zone.name}");
        }
    }
}