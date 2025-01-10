using UnityEngine;

public class ValveInteraction : MonoBehaviour
{
    public float interactionRadius = 2f; // 상호작용 반경
    public float interactionOffset = 1.5f; // 플레이어로부터 전방 거리
    public float heightOffset = 1f; // 플레이어 기준 위로 이동할 높이
    public LayerMask valveLayerMask; // 벨브에 해당하는 레이어 마스크
    public LayerMask windowLayerMask; // 창문에 해당하는 레이어 마스크

    private ValveController currentValve; // 현재 감지된 벨브
    private WindowController currentWindow; // 현재 감지된 창문

    private void Update()
    {
        DetectObjects();

        // E키로 상호작용
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentValve != null)
            {
                currentValve.Interact();
            }
            else if (currentWindow != null)
            {
                currentWindow.RotateSymbol(90f); // 창문 문양 90도 회전
            }
        }
    }

    private void DetectObjects()
    {
        // 플레이어 앞쪽과 높이를 포함한 상호작용 영역 위치 계산
        Vector3 interactionCenter = transform.position + transform.forward * interactionOffset + Vector3.up * heightOffset;

        // OverlapSphere로 벨브 감지
        Collider[] valveColliders = Physics.OverlapSphere(interactionCenter, interactionRadius, valveLayerMask);
        currentValve = valveColliders.Length > 0 ? valveColliders[0].GetComponent<ValveController>() : null;

        // OverlapSphere로 창문 감지
        Collider[] windowColliders = Physics.OverlapSphere(interactionCenter, interactionRadius, windowLayerMask);
        currentWindow = windowColliders.Length > 0 ? windowColliders[0].GetComponent<WindowController>() : null;

        // 디버그 로그
        if (currentValve != null)
        {
            Debug.Log($"현재 감지된 벨브: {currentValve.name}");
        }
        else if (currentWindow != null)
        {
            Debug.Log($"현재 감지된 창문: {currentWindow.name}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 상호작용 반경 시각화
        Gizmos.color = Color.yellow;

        // 플레이어 앞쪽과 위쪽의 상호작용 영역 표시
        Vector3 interactionCenter = transform.position + transform.forward * interactionOffset + Vector3.up * heightOffset;
        Gizmos.DrawWireSphere(interactionCenter, interactionRadius);
    }
}
