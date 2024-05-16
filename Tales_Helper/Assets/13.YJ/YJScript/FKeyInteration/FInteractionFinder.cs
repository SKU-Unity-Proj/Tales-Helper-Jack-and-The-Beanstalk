using UnityEngine;
using static IFKeyInteractable;

public class FInteractionFinder : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키
    public float interactionRange = 3f; // 상호작용 범위

    public GameObject[] interactables;
    public GameObject currentInteractable; // 현재 상호작용 가능한 오브젝트를 저장할 변수
    private float currentInteractableDistance; // 상호작용 오브젝트와의 거리

    private void Awake()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            FindNearestInteractable(); // 가장 가까운 상호작용 가능한 오브젝트를 찾음
            if (currentInteractable != null)
            {
                InteractWithCurrent();
            }
        }
    }

    void FindNearestInteractable()
    {
        //GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        float minDistance = Mathf.Infinity; // 최소 거리를 무한대로 초기화
        currentInteractable = null;

        foreach (GameObject interactable in interactables) // 모든 상호작용 가능한 오브젝트를 순회하면서
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position); // 현재 오브젝트와의 거리를 계산

            if (distance < minDistance && distance <= interactionRange) // 만약 이 거리가 최소 거리보다 작고 상호작용 범위 내에 있다면
            {
                minDistance = distance; // 최소 거리를 업데이트
                currentInteractable = interactable; // 현재 상호작용 가능한 오브젝트를 업데이트
                currentInteractableDistance = distance; // 현재 오브젝트의 거리 저장
            }
        }
    }

    void InteractWithCurrent() //상호작용 호출 함수
    {
        IFInteractable interactable = currentInteractable.GetComponent<IFInteractable>();

        if (interactable != null)
        {
            interactable.Interact(currentInteractableDistance); // 상호작용 함수 호출
            Debug.Log(interactable);
        }
    }
}
