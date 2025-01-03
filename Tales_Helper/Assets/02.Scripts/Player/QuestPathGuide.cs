using UnityEngine;
using System.Collections.Generic;

public class QuestPathGuide : MonoBehaviour
{
    public QuestManager questManager; // ����Ʈ �Ŵ��� ����
    public Transform player; // �÷��̾� ��ġ
    public LineRenderer lineRenderer; // ��θ� ǥ���� LineRenderer
    public LayerMask groundLayer; // �� ���̾� ����ũ

    [Range(0.5f, 5f)]
    public float pointInterval = 1.0f; // ��� �� ����
    public float arrivalThreshold = 1.0f; // ���� �Ÿ� �Ӱ谪
    public float effectSpeed = 2.0f; // ����Ʈ �ӵ�

    public GameObject effectPrefab; // ����Ʈ ������
    private GameObject effectInstance; // ����Ʈ �ν��Ͻ�

    private List<Vector3> pathPoints = new List<Vector3>();
    private List<Vector3> smoothPathPoints = new List<Vector3>(); // � ���� ����Ʈ
    private float effectDistance = 0f; // ����Ʈ�� ��ο��� �̵��� �Ÿ�

    private bool isPathActive = true; // ���η����� Ȱ�� ����

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // ���� ����
        lineRenderer.startColor = Color.yellow; // ���� ����
        lineRenderer.endColor = Color.yellow;   // �� ����

        if (effectPrefab != null)
        {
            effectInstance = Instantiate(effectPrefab);
        }
    }

    void Update()
    {
        UpdatePathToQuestTarget();
        UpdateEffectPosition();
    }

    void UpdatePathToQuestTarget()
    {
        Transform target = GetCurrentQuestTarget();
        if (target == null)
        {
            Debug.LogWarning("[QuestPathRenderer] ���� ����Ʈ ��ǥ�� ã�� �� �����ϴ�.");
            return;
        }

        if (Vector3.Distance(player.position, target.position) < arrivalThreshold)
        {
            if (questManager.questId == 40 && isPathActive)
            {
                Debug.Log("[QuestPathRenderer] '�� �ɱ�' ��ǥ�� ����. LineRenderer ��Ȱ��ȭ.");
                lineRenderer.enabled = false;
                if (effectInstance != null) effectInstance.SetActive(false);
                isPathActive = false;
                return;
            }
        }
        else if (!isPathActive)
        {
            Debug.Log("[QuestPathRenderer] ���ο� ��ǥ�� ������. LineRenderer Ȱ��ȭ.");
            lineRenderer.enabled = true;
            if (effectInstance != null) effectInstance.SetActive(true);
            isPathActive = true;
        }

        pathPoints.Clear();
        smoothPathPoints.Clear();

        Vector3 start = player.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        int pointCount = Mathf.CeilToInt(distance / pointInterval);

        for (int i = 0; i <= pointCount; i++)
        {
            Vector3 point = Vector3.Lerp(start, end, i / (float)pointCount);

            if (Physics.Raycast(point + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, groundLayer))
            {
                pathPoints.Add(hit.point);
            }
            else
            {
                pathPoints.Add(point);
            }
        }

        GenerateSmoothPath();

        lineRenderer.positionCount = smoothPathPoints.Count;
        lineRenderer.SetPositions(smoothPathPoints.ToArray());

    }

    void GenerateSmoothPath()
    {
        smoothPathPoints.Clear();
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 p0 = (i == 0) ? pathPoints[i] : pathPoints[i - 1];
            Vector3 p1 = pathPoints[i];
            Vector3 p2 = pathPoints[i + 1];
            Vector3 p3 = (i + 2 < pathPoints.Count) ? pathPoints[i + 2] : pathPoints[i + 1];

            for (float t = 0; t <= 1; t += 0.1f)
            {
                Vector3 point = CatmullRom(p0, p1, p2, p3, t);
                smoothPathPoints.Add(point);
            }
        }
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    void UpdateEffectPosition()
    {
        if (effectInstance == null || smoothPathPoints.Count < 2)
        {
            Debug.LogWarning("[QuestPathRenderer] ����Ʈ�� �̵��� ��� ����Ʈ�� �����մϴ�.");
            return;
        }

        // ����Ʈ �̵� �Ÿ� ������Ʈ
        effectDistance += Time.deltaTime * effectSpeed;

        float coveredDistance = 0f;

        for (int i = 0; i < smoothPathPoints.Count - 1; i++)
        {
            Vector3 currentPoint = smoothPathPoints[i];
            Vector3 nextPoint = smoothPathPoints[i + 1];
            float segmentLength = Vector3.Distance(currentPoint, nextPoint);

            if (effectDistance <= coveredDistance + segmentLength)
            {
                float lerpFactor = (effectDistance - coveredDistance) / segmentLength;
                effectInstance.transform.position = Vector3.Lerp(currentPoint, nextPoint, lerpFactor);

                // ����Ʈ ���� ����
                Vector3 direction = (nextPoint - currentPoint).normalized;
                if (direction != Vector3.zero)
                {
                    effectInstance.transform.rotation = Quaternion.LookRotation(direction);
                }
                return;
            }

            coveredDistance += segmentLength;
        }

        // ������ ����Ʈ�� ������ ���
        effectInstance.transform.position = smoothPathPoints[smoothPathPoints.Count - 1];

        // ����Ʈ �̵� �Ÿ� �ʱ�ȭ
        effectDistance = 0f;
        Debug.Log("[QuestPathRenderer] ����Ʈ�� ������ ����Ʈ�� �����߽��ϴ�.");
    }


    Transform GetCurrentQuestTarget()
    {
        int questId = questManager.questId;
        int actionIndex = questManager.questActionIndex;

        //Debug.Log($"[QuestPathRenderer] ���� ����Ʈ ID: {questId}, ActionIndex: {actionIndex}");

        switch (questId)
        {
            case 10:
                if (actionIndex == 0) return questManager.questObject[0]?.transform; // ���� ����ǥ
                if (actionIndex == 1) return questManager.questObject[2]?.transform; // ���� ����ǥ
                break;

            case 20:
                if (actionIndex == 0) return questManager.questObject[5]?.transform; // ���ָӴ�
                break;

            case 30:
                if (actionIndex == 0) return questManager.questObject[0]?.transform; // ���� ����ǥ
                break;

            case 40: // �� �ɱ� �ܰ�
                if (questManager.plantingPoint != null)
                {
                    Debug.Log("[QuestPathRenderer] '�� �ɱ�' ��ǥ �������� �̵� ��...");
                    return questManager.plantingPoint;
                }
                break;
        }

        Debug.LogWarning("[QuestPathRenderer] ��ȿ�� ��ǥ ������ �����ϴ�.");
        return null;
    }
}
