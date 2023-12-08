using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Crawl_Raser : MonoBehaviour
{
    public Transform startPoint;        // 시작 지점
    public Transform endPoint;          // 끝 지점
    private LineRenderer lineRenderer;

    void Update()
    {
        DrawStraightLine();
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void DrawStraightLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
