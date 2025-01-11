using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform[] ropeSegments;

    void Update()
    {
        lineRenderer.positionCount = ropeSegments.Length;

        for (int i = 0; i < ropeSegments.Length; i++)
        {
            lineRenderer.SetPosition(i, ropeSegments[i].position);
        }
    }
}
