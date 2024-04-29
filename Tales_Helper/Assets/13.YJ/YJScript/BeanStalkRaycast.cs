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
        // 위치를 기준으로 레이 발사
        Ray ray = new Ray(transform.position+transform.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            climbBeanStalk.isClimb = true;

            //Quaternion targetRotation = Quaternion.LookRotation(hit.normal, transform.up);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1f);

            // 캐릭터의 전방 벡터를 레이가 검출한 콜라이더의 방향으로 설정
            transform.forward = -hit.normal;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position+transform.up, transform.forward * raycastDistance);
    }
}
