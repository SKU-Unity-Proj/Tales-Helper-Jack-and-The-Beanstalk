using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    public GameObject segmentPrefab; // ���� ���׸�Ʈ ������
    public int segmentCount = 10;    // ���׸�Ʈ ����
    public float segmentSpacing = 0.5f; // ���׸�Ʈ �� ����
    public Transform startPoint;    // ������

    void Start()
    {
        GameObject previousSegment = null;

        for (int i = 0; i < segmentCount; i++)
        {
            // ���׸�Ʈ ����
            GameObject segment = Instantiate(segmentPrefab, startPoint.position + Vector3.down * segmentSpacing * i, Quaternion.identity);

            // Rigidbody �߰�
            Rigidbody rb = segment.AddComponent<Rigidbody>();

            rb.mass = 1.0f;

            // ������ ����Ʈ ����
            SpringJoint joint = segment.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = new Vector3(0, -segmentSpacing, 0);
            joint.spring = 50f;
            joint.damper = 5f;

            // ���� ���׸�Ʈ�� ����
            if (previousSegment != null)
            {
                joint.connectedBody = previousSegment.GetComponent<Rigidbody>();
            }

            previousSegment = segment;
        }
    }
}
