using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entranceChandelier : MonoBehaviour
{
    public List<Transform> transformList = new List<Transform>();

    void Start()
    {
        // ��� Ʈ�������� �޽� �ݶ��̴��� Ȱ��ȭ
        ActivateMeshColliders();
    }

    // �޽� �ݶ��̴��� Ȱ��ȭ�ϴ� �޼ҵ�
    void ActivateMeshColliders()
    {
        foreach (Transform trans in transformList)
        {
            MeshCollider meshCollider = trans.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.enabled = true;
            }
        }
    }
}
