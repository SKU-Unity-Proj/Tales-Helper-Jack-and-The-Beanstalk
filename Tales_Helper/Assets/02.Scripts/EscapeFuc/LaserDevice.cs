using UnityEngine;

public class LaserDevice : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserLine; // �������� �ð�ȭ�ϴ� LineRenderer
    [SerializeField] private LayerMask reflectionMask; // �ݻ� ������ ���̾�
    [SerializeField] private LayerMask targetMask; // Ÿ�� ���� ���̾�
    [SerializeField] private Color laserColor = Color.red; // ������ ����
    [SerializeField] private float laserRange = 50f; // ������ �ִ� �Ÿ�

    public Color LaserColor => laserColor; // �ܺο��� ������ ���� Ȯ�� ����
    public bool IsHittingTarget { get; private set; } // Ÿ�ٿ� ��Ҵ��� Ȯ��
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

        while (Physics.Raycast(laserOrigin, laserDirection, out RaycastHit hit, laserRange, reflectionMask | targetMask))
        {
            laserLine.positionCount += 1;
            laserLine.SetPosition(laserLine.positionCount - 1, hit.point);

            // ���� ������Ʈ �ݺ� ���� ����
            if (hit.collider.gameObject == lastHitObject)
            {
                Debug.Log("Laser ignored duplicate reflection.");
                break;
            }

            lastHitObject = hit.collider.gameObject; // �浹�� ������Ʈ ����

            // Ÿ�� ������ ��Ҵ��� Ȯ��
            if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
            {
                var target = hit.collider.GetComponent<LaserTarget>();
                if (target != null)
                {
                    target.CheckLaserHit(this); // Ÿ�ٿ� ������ ���� ����
                }

                Debug.Log("Laser hit target.");
                IsHittingTarget = true;
                return;
            }

            // �ݻ� ó��
            if (((1 << hit.collider.gameObject.layer) & reflectionMask) != 0)
            {
                Debug.Log("Laser reflected.");
                laserOrigin = hit.point + hit.normal * 0.01f; // ������ �߰�
                laserDirection = Vector3.Reflect(laserDirection, hit.normal); // �ݻ� ��� ���
                continue;
            }

            // ���� ó��: reflectionMask�� targetMask�� ���Ե��� ���� ��ü
            Debug.Log($"Laser blocked by: {hit.collider.gameObject.name}");
            break; // ������ �ߴ�
        }

        // Ÿ�ٿ� ���� ������ ���� �ʱ�ȭ
        IsHittingTarget = false;
        laserLine.positionCount += 1;
        laserLine.SetPosition(laserLine.positionCount - 1, laserOrigin + laserDirection * laserRange);
    }

}
