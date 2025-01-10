using UnityEngine;

public class ValveInteraction : MonoBehaviour
{
    public float interactionRadius = 2f; // ��ȣ�ۿ� �ݰ�
    public float interactionOffset = 1.5f; // �÷��̾�κ��� ���� �Ÿ�
    public float heightOffset = 1f; // �÷��̾� ���� ���� �̵��� ����
    public LayerMask valveLayerMask; // ���꿡 �ش��ϴ� ���̾� ����ũ
    public LayerMask windowLayerMask; // â���� �ش��ϴ� ���̾� ����ũ

    private ValveController currentValve; // ���� ������ ����
    private WindowController currentWindow; // ���� ������ â��

    private void Update()
    {
        DetectObjects();

        // EŰ�� ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentValve != null)
            {
                currentValve.Interact();
            }
            else if (currentWindow != null)
            {
                currentWindow.RotateSymbol(90f); // â�� ���� 90�� ȸ��
            }
        }
    }

    private void DetectObjects()
    {
        // �÷��̾� ���ʰ� ���̸� ������ ��ȣ�ۿ� ���� ��ġ ���
        Vector3 interactionCenter = transform.position + transform.forward * interactionOffset + Vector3.up * heightOffset;

        // OverlapSphere�� ���� ����
        Collider[] valveColliders = Physics.OverlapSphere(interactionCenter, interactionRadius, valveLayerMask);
        currentValve = valveColliders.Length > 0 ? valveColliders[0].GetComponent<ValveController>() : null;

        // OverlapSphere�� â�� ����
        Collider[] windowColliders = Physics.OverlapSphere(interactionCenter, interactionRadius, windowLayerMask);
        currentWindow = windowColliders.Length > 0 ? windowColliders[0].GetComponent<WindowController>() : null;

        // ����� �α�
        if (currentValve != null)
        {
            Debug.Log($"���� ������ ����: {currentValve.name}");
        }
        else if (currentWindow != null)
        {
            Debug.Log($"���� ������ â��: {currentWindow.name}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ��ȣ�ۿ� �ݰ� �ð�ȭ
        Gizmos.color = Color.yellow;

        // �÷��̾� ���ʰ� ������ ��ȣ�ۿ� ���� ǥ��
        Vector3 interactionCenter = transform.position + transform.forward * interactionOffset + Vector3.up * heightOffset;
        Gizmos.DrawWireSphere(interactionCenter, interactionRadius);
    }
}
