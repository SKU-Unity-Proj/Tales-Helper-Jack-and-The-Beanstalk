using UnityEngine;
using System.Collections.Generic;

public class QuestPathGuide : MonoBehaviour
{
    public QuestManager questManager; // 퀘스트 매니저 참조
    public Transform player; // 플레이어 위치
    public LineRenderer lineRenderer; // 경로를 표시할 LineRenderer
    public LayerMask groundLayer; // 땅 레이어 마스크

    [Range(0.5f, 5f)]
    public float pointInterval = 1.0f; // 경로 점 간격
    public float arrivalThreshold = 1.0f; // 도착 거리 임계값
    public float effectSpeed = 2.0f; // 이펙트 속도

    public GameObject effectPrefab; // 이펙트 프리팹
    private GameObject effectInstance; // 이펙트 인스턴스

    private List<Vector3> pathPoints = new List<Vector3>();
    private List<Vector3> smoothPathPoints = new List<Vector3>(); // 곡선 보간 포인트
    private float effectDistance = 0f; // 이펙트가 경로에서 이동한 거리

    private bool isPathActive = true; // 라인랜더러 활성 상태

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 색상 설정
        lineRenderer.startColor = Color.yellow; // 시작 색상
        lineRenderer.endColor = Color.yellow;   // 끝 색상

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
            Debug.LogWarning("[QuestPathRenderer] 현재 퀘스트 목표를 찾을 수 없습니다.");
            return;
        }

        if (Vector3.Distance(player.position, target.position) < arrivalThreshold)
        {
            if (questManager.questId == 40 && isPathActive)
            {
                Debug.Log("[QuestPathRenderer] '콩 심기' 목표에 도착. LineRenderer 비활성화.");
                lineRenderer.enabled = false;
                if (effectInstance != null) effectInstance.SetActive(false);
                isPathActive = false;
                return;
            }
        }
        else if (!isPathActive)
        {
            Debug.Log("[QuestPathRenderer] 새로운 목표가 설정됨. LineRenderer 활성화.");
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
            Debug.LogWarning("[QuestPathRenderer] 이펙트가 이동할 경로 포인트가 부족합니다.");
            return;
        }

        // 이펙트 이동 거리 업데이트
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

                // 이펙트 방향 설정
                Vector3 direction = (nextPoint - currentPoint).normalized;
                if (direction != Vector3.zero)
                {
                    effectInstance.transform.rotation = Quaternion.LookRotation(direction);
                }
                return;
            }

            coveredDistance += segmentLength;
        }

        // 마지막 포인트에 도달한 경우
        effectInstance.transform.position = smoothPathPoints[smoothPathPoints.Count - 1];

        // 이펙트 이동 거리 초기화
        effectDistance = 0f;
        Debug.Log("[QuestPathRenderer] 이펙트가 마지막 포인트에 도달했습니다.");
    }


    Transform GetCurrentQuestTarget()
    {
        int questId = questManager.questId;
        int actionIndex = questManager.questActionIndex;

        //Debug.Log($"[QuestPathRenderer] 현재 퀘스트 ID: {questId}, ActionIndex: {actionIndex}");

        switch (questId)
        {
            case 10:
                if (actionIndex == 0) return questManager.questObject[0]?.transform; // 엄마 느낌표
                if (actionIndex == 1) return questManager.questObject[2]?.transform; // 상인 느낌표
                break;

            case 20:
                if (actionIndex == 0) return questManager.questObject[5]?.transform; // 콩주머니
                break;

            case 30:
                if (actionIndex == 0) return questManager.questObject[0]?.transform; // 엄마 느낌표
                break;

            case 40: // 콩 심기 단계
                if (questManager.plantingPoint != null)
                {
                    Debug.Log("[QuestPathRenderer] '콩 심기' 목표 지점으로 이동 중...");
                    return questManager.plantingPoint;
                }
                break;
        }

        Debug.LogWarning("[QuestPathRenderer] 유효한 목표 지점이 없습니다.");
        return null;
    }
}
