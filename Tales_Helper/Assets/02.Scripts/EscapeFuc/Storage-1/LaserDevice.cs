using UnityEngine;

public class LaserDevice : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserLine; // �������� �ð�ȭ�ϴ� LineRenderer
    [SerializeField] private LayerMask reflectionMask; // �ݻ� ������ ���̾�
    [SerializeField] private LayerMask targetMask; // Ÿ�� ���� ���̾�
    [SerializeField] private int maxReflections = 5; // �ִ� �ݻ� Ƚ��
    [SerializeField] private Color laserColor = Color.red; // ������ ����
    [SerializeField] private float laserRange = 50f; // ������ �ִ� �Ÿ�

    public Color LaserColor => laserColor; // �ܺο��� ������ ���� Ȯ�� ����
    public bool IsHittingTarget { get; private set; } = false;// Ÿ�ٿ� ��Ҵ��� Ȯ��
    private GameObject lastHitObject; // ���������� �浹�� ������Ʈ ����

    private void Start()
    {
        // ������ ���� ����
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

        laserLine.positionCount = 1; // ������ ������
        laserLine.SetPosition(0, laserOrigin);

        lastHitObject = null; // �浹 ��� �ʱ�ȭ

        int reflections = 0;
        while (reflections < maxReflections)
        {
            if(Physics.Raycast(laserOrigin, laserDirection, out RaycastHit hit, laserRange))
            {
                laserLine.positionCount += 1;
                laserLine.SetPosition(laserLine.positionCount - 1, hit.point);

                // �ݻ� ó��
                if (((1 << hit.collider.gameObject.layer) & reflectionMask) != 0)
                {
                    // ������ �ݻ� ó��
                    Debug.Log($"Laser reflected by: {hit.collider.gameObject.name}");
                    laserOrigin = hit.point + hit.normal * 0.01f; // ������ �߰�
                    laserDirection = Vector3.Reflect(laserDirection, hit.normal); // �ݻ� ��� ���
                    reflections++;
                    continue;
                }

                // Ÿ�� ������ ��Ҵ��� Ȯ��
                if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
                {
                    var target = hit.collider.GetComponent<LaserTarget>();
                    if (target != null)
                    {
                        target.CheckLaserHit(this); // Ÿ�ٿ� ������ ���� ����
                    }

                    IsHittingTarget = true;
                    return;
                }
                // ���� ó��: reflectionMask�� targetMask�� ���Ե��� ���� ��ü
                Debug.Log($"Laser blocked by: {hit.collider.gameObject.name}");
                break; // ������ �ߴ�
            }
            IsHittingTarget = false;
            laserLine.positionCount += 1;
            laserLine.SetPosition(laserLine.positionCount - 1, laserOrigin + laserDirection * laserRange);
            break;
        }
    }
}
