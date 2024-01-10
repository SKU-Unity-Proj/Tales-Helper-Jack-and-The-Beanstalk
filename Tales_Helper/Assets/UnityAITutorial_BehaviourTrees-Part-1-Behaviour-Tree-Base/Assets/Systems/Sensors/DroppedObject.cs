using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public Transform giantTransform; // ������ Transform
    public float raycastDistance = 50f; // ����ĳ��Ʈ �Ÿ�
    public Color raycastDebugColor = Color.red; // ����ĳ��Ʈ ����� ���� ����

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Giant"))
        {
            Vector3 directionToGiant = (giantTransform.position - transform.position).normalized;
            RaycastHit hit;

            // ����ĳ��Ʈ ����� �� �׸���
            Debug.DrawLine(transform.position, transform.position + directionToGiant * raycastDistance, raycastDebugColor, 2f);

            if (Physics.Raycast(transform.position, directionToGiant, out hit, raycastDistance))
            {
                if (hit.transform == giantTransform)
                {
                    // ���ο��� �������� ����� ����
                    NotifyGiant(hit.transform.gameObject);
                }
            }
        }
    }

    void NotifyGiant(GameObject giant)
    {
       // giant.GetComponent<CharacterAgent>().MoveToAndInteract(transform.position);
    }
}

