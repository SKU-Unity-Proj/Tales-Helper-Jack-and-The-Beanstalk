using UnityEngine;
using System.Collections;

public class FogChange: MonoBehaviour
{
    public GameObject smokeObject;

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� "Fog" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Fog"))
        {
            // "smoke" ������Ʈ�� �����ϰ� Ȱ��ȭ�Ǿ� �ִٸ� ��Ȱ��ȭ
            if (smokeObject != null && smokeObject.activeSelf)
            {
                smokeObject.SetActive(false);
            }
        }
    }
}
