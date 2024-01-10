using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObject : MonoBehaviour
{
    public Transform giantTransform; // ������ Transform
    public LineRenderer lineRenderer; // LineRenderer ������Ʈ
    public float raycastDistance = 50f; // ����ĳ��Ʈ �Ÿ�

    public List<Transform> DroppedObjects = new List<Transform>(); // ������ ������Ʈ ����Ʈ

    private bool isRaycasting = false;
    private bool isConditionMet = false;

    void Start()
    {
        Animator anim = GetComponent<Animator>();

        // LineRenderer �ʱ�ȭ
        lineRenderer.positionCount = 2; // ���� �������� ���� ����
        lineRenderer.enabled = false; // ó������ ���� ��Ȱ��ȭ
    }

    void Update()
    {
        if (isRaycasting)
        {
            UpdateRaycastingLine();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Giant"))
        {
            RaycastHit hit;
            Vector3 directionToGiant = (giantTransform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, directionToGiant, out hit, raycastDistance))
            {
                // ����ĳ��Ʈ�� ���ο��� �����ߴ��� Ȯ��
                if (hit.transform == giantTransform)
                {
                    Debug.Log("Raycast hit the Giant.");
                    isRaycasting = true;
                    lineRenderer.enabled = true;

                    // ����ĳ��Ʈ�� ���ο��� �����ߴٸ�, ������Ʈ�� ���� ��Ͽ� �߰�
                    AddDroppedObject(transform);
                    isConditionMet = true;
                }
                else
                {
                    Debug.Log("Raycast did not hit the Giant. It hit: " + hit.transform.name);
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }

    }

    public void AddDroppedObject(Transform obj)
    {
        if (!DroppedObjects.Contains(obj))
        {
            DroppedObjects.Add(obj);
        }
    }

    void UpdateRaycastingLine()
    {
        // ���� �������� ������Ʈ�� ��ġ��, ������ ������ ��ġ�� ����
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, giantTransform.position);
    }
    public bool CheckCondition()
    {
        return isConditionMet;
    }
   
}