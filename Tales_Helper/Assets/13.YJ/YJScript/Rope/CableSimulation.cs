using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CableSimulation : MonoBehaviour
{
    public Transform startPoint; // 전선의 시작 지점 (콘센트)
    public Transform endPoint;   // 전선의 끝 지점 (토스트기)
    public float curveHeight = 0.5f; // 곡선의 휘어짐 높이
    public int resolution = 20;  // 곡선의 세분화 정도

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawCable();
    }

    void DrawCable()
    {
        // 곡선을 따라갈 포인트 계산
        Vector3[] points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1); // 0 ~ 1 사이의 비율
            points[i] = GetBezierPoint(t, startPoint.position, GetControlPoint(), endPoint.position);
        }

        // Line Renderer 업데이트
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    Vector3 GetControlPoint()
    {
        // 중간 제어점을 계산 (시작과 끝 사이의 중간에서 위로 올라감)
        Vector3 midpoint = (startPoint.position + endPoint.position) / 2;
        return midpoint + Vector3.up * curveHeight;
    }

    Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 2차 베지어 곡선 공식: (1-t)² * p0 + 2 * (1-t) * t * p1 + t² * p2
        return Mathf.Pow(1 - t, 2) * p0 +
               2 * (1 - t) * t * p1 +
               Mathf.Pow(t, 2) * p2;
    }
}
