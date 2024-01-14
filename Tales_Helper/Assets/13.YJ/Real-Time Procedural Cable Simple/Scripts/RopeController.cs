using UnityEngine;

public class RopeController : MonoBehaviour
{
    public Transform connectedObject; // 도르래가 연결된 오브젝트(도르래 끝 부분 등)
    public int segmentCount = 10; // 밧줄 세그먼트의 수
    public float ropeLength = 5f; // 밧줄의 초기 길이
    public float ropeWidth = 0.1f; // 밧줄의 두께

    private LineRenderer lineRenderer;
    private Vector3[] ropePositions;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        InitializeRope();
    }

    void InitializeRope()
    {
        lineRenderer.positionCount = segmentCount;

        ropePositions = new Vector3[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            ropePositions[i] = transform.position + Vector3.up * (ropeLength / segmentCount) * i;
        }
    }

    void Update()
    {
        UpdateRopePositions();
    }

    void UpdateRopePositions()
    {
        ropePositions[0] = transform.position;
        ropePositions[segmentCount - 1] = connectedObject.position;

        for (int i = 1; i < segmentCount - 1; i++)
        {
            float t = i / (float)(segmentCount - 1);
            ropePositions[i] = Vector3.Lerp(transform.position, connectedObject.position, t);
        }

        lineRenderer.SetPositions(ropePositions);
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
    }
}
