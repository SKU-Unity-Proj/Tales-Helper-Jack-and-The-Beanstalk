using UnityEngine;

public class Mirror : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // �ſ� �ո��� �Ķ������� ǥ��
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // �ſ� �޸��� ���������� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.forward * 2f);
    }

}
