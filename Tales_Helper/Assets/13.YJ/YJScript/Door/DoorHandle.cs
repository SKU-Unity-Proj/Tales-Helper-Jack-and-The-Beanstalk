using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorHandle : MonoBehaviour
{   
    public Animator doorAnim;
    private DroppedObject manager;
    private NavMeshObstacle navMeshObstacle;
    [SerializeField] Transform doorPos;

    private void Start()
    {
        // DroppedObjectManager�� �ν��Ͻ��� ã�Ƽ� �Ҵ�
        manager = DroppedObject.Instance;
        navMeshObstacle = GetComponent<NavMeshObstacle>();

        // �Ŵ����� �������� ���� ���, �α� �޽��� ��� �Ǵ� ���� ó��
        if (manager == null)
        {
            Debug.LogError("DroppedObjectManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            // �ʿ��� ���, ���⿡ �߰����� ���� ó�� �ڵ带 �߰�
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            doorAnim.SetTrigger("Grab");

            // ���� ���� �� NavMesh Obstacle ��Ȱ��ȭ
            navMeshObstacle.enabled = false;

            this.GetComponent<BoxCollider>().enabled = false;
            doorPos.GetComponent<BoxCollider>().enabled = true;

            // �Ŵ��� �ν��Ͻ��� �����ϴ� ��쿡�� �迭�� ����
            if (manager != null)
            {
                manager.AddDroppedObject(this.gameObject);
            }
        }
    }
}
