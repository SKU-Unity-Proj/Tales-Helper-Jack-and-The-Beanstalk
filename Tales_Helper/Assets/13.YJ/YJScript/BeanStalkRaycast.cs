using UnityEngine;

public class BeanStalkRaycast : MonoBehaviour
{
    public LayerMask layerMask;
    public float raycastDistance = 0.5f;
    public float offsetDistance = 0.1f;

    public ClimbingBeanStalk climbBeanStalk;

    private void Start()
    {
        climbBeanStalk = GetComponent<ClimbingBeanStalk>();
    }

    void Update()
    {
        // ��ġ�� �������� ���� �߻�
        Ray ray = new Ray(transform.position+transform.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            climbBeanStalk.isClimb = true;

            //Quaternion targetRotation = Quaternion.LookRotation(hit.normal, transform.up);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1f);

            // ĳ������ ���� ���͸� ���̰� ������ �ݶ��̴��� �������� ����
            transform.forward = -hit.normal;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position+transform.up, transform.forward * raycastDistance);
    }
}
