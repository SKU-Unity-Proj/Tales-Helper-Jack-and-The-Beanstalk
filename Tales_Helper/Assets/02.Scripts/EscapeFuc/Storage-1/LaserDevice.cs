using UnityEngine;

public class LaserDevice : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserLine; // 레이저를 시각화하는 LineRenderer
    [SerializeField] private LayerMask reflectionMask; // 반사 가능한 레이어
    [SerializeField] private LayerMask targetMask; // 타겟 지점 레이어
    [SerializeField] private int maxReflections = 5; // 최대 반사 횟수
    [SerializeField] private Color laserColor = Color.red; // 레이저 색상
    [SerializeField] private float laserRange = 50f; // 레이저 최대 거리

    public Color LaserColor => laserColor; // 외부에서 레이저 색상 확인 가능
    public bool IsHittingTarget { get; private set; } = false;// 타겟에 닿았는지 확인
    private GameObject lastHitObject; // 마지막으로 충돌한 오브젝트 저장

    private void Start()
    {
        // 레이저 색상 설정
        laserLine.startColor = laserColor;
        laserLine.endColor = laserColor;
    }

    private void Update()
    {
        FireLaser();
    }

    private void FireLaser()
    {
        Vector3 laserOrigin = transform.position;
        Vector3 laserDirection = -transform.up;

        laserLine.positionCount = 1; // 레이저 시작점
        laserLine.SetPosition(0, laserOrigin);

        lastHitObject = null; // 충돌 기록 초기화

        int reflections = 0;
        while (reflections < maxReflections)
        {
            if(Physics.Raycast(laserOrigin, laserDirection, out RaycastHit hit, laserRange))
            {
                laserLine.positionCount += 1;
                laserLine.SetPosition(laserLine.positionCount - 1, hit.point);

                // 반사 처리
                if (((1 << hit.collider.gameObject.layer) & reflectionMask) != 0)
                {
                    // 레이저 반사 처리
                    Debug.Log($"Laser reflected by: {hit.collider.gameObject.name}");
                    laserOrigin = hit.point + hit.normal * 0.01f; // 오프셋 추가
                    laserDirection = Vector3.Reflect(laserDirection, hit.normal); // 반사 경로 계산
                    reflections++;
                    continue;
                }

                // 타겟 지점에 닿았는지 확인
                if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
                {
                    var target = hit.collider.GetComponent<LaserTarget>();
                    if (target != null)
                    {
                        target.CheckLaserHit(this); // 타겟에 레이저 색상 전달
                    }

                    IsHittingTarget = true;
                    return;
                }
                // 차단 처리: reflectionMask와 targetMask에 포함되지 않은 물체
                Debug.Log($"Laser blocked by: {hit.collider.gameObject.name}");
                break; // 레이저 중단
            }
            IsHittingTarget = false;
            laserLine.positionCount += 1;
            laserLine.SetPosition(laserLine.positionCount - 1, laserOrigin + laserDirection * laserRange);
            break;
        }
    }
}
