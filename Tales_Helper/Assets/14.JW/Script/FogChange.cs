using UnityEngine;
using System.Collections;

public class FogChange: MonoBehaviour
{
    public GameObject smokeObject;

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 "Fog" 태그를 가지고 있는지 확인
        if (other.CompareTag("Fog"))
        {
            // "smoke" 오브젝트가 존재하고 활성화되어 있다면 비활성화
            if (smokeObject != null && smokeObject.activeSelf)
            {
                smokeObject.SetActive(false);
            }
        }
    }
}
