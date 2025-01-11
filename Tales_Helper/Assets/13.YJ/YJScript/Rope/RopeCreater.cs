using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    public GameObject segmentPrefab; // 밧줄 세그먼트 프리팹
    public int segmentCount = 10;    // 세그먼트 개수
    public float segmentSpacing = 0.5f; // 세그먼트 간 간격
    public Transform startPoint;    // 시작점

    void Start()
    {
        GameObject previousSegment = null;

        for (int i = 0; i < segmentCount; i++)
        {
            // 세그먼트 생성
            GameObject segment = Instantiate(segmentPrefab, startPoint.position + Vector3.down * segmentSpacing * i, Quaternion.identity);

            // Rigidbody 추가
            Rigidbody rb = segment.AddComponent<Rigidbody>();

            rb.mass = 1.0f;

            // 스프링 조인트 연결
            SpringJoint joint = segment.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = new Vector3(0, -segmentSpacing, 0);
            joint.spring = 50f;
            joint.damper = 5f;

            // 이전 세그먼트와 연결
            if (previousSegment != null)
            {
                joint.connectedBody = previousSegment.GetComponent<Rigidbody>();
            }

            previousSegment = segment;
        }
    }
}
