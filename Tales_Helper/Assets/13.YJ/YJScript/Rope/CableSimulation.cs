using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CableSimulation : MonoBehaviour
{
    public Transform startPoint; // ������ ���� ���� (�ܼ�Ʈ)
    public Transform endPoint;   // ������ �� ���� (�佺Ʈ��)
    public float curveHeight = 0.5f; // ��� �־��� ����
    public int resolution = 20;  // ��� ����ȭ ����

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
        // ��� ���� ����Ʈ ���
        Vector3[] points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1); // 0 ~ 1 ������ ����
            points[i] = GetBezierPoint(t, startPoint.position, GetControlPoint(), endPoint.position);
        }

        // Line Renderer ������Ʈ
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    Vector3 GetControlPoint()
    {
        // �߰� �������� ��� (���۰� �� ������ �߰����� ���� �ö�)
        Vector3 midpoint = (startPoint.position + endPoint.position) / 2;
        return midpoint + Vector3.up * curveHeight;
    }

    Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 2�� ������ � ����: (1-t)�� * p0 + 2 * (1-t) * t * p1 + t�� * p2
        return Mathf.Pow(1 - t, 2) * p0 +
               2 * (1 - t) * t * p1 +
               Mathf.Pow(t, 2) * p2;
    }
}
