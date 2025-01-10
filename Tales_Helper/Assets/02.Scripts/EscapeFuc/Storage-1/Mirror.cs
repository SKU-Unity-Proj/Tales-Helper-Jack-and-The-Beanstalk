using UnityEngine;

public class Mirror : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // 거울 앞면을 파란색으로 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // 거울 뒷면을 빨간색으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.forward * 2f);
    }

}
