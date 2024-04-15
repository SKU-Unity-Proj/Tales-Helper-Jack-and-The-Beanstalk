using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entranceChandelier : MonoBehaviour
{
    public List<Transform> transformList = new List<Transform>();

    void Start()
    {
        // 모든 트랜스폼의 메쉬 콜라이더를 활성화
        ActivateMeshColliders();
    }

    // 메쉬 콜라이더를 활성화하는 메소드
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
