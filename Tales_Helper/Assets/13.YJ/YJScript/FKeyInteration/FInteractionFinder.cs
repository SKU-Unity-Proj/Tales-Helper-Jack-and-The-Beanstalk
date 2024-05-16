using UnityEngine;
using static IFKeyInteractable;

public class FInteractionFinder : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű
    public float interactionRange = 3f; // ��ȣ�ۿ� ����

    public GameObject[] interactables;
    public GameObject currentInteractable; // ���� ��ȣ�ۿ� ������ ������Ʈ�� ������ ����
    private float currentInteractableDistance; // ��ȣ�ۿ� ������Ʈ���� �Ÿ�

    private void Awake()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            FindNearestInteractable(); // ���� ����� ��ȣ�ۿ� ������ ������Ʈ�� ã��
            if (currentInteractable != null)
            {
                InteractWithCurrent();
            }
        }
    }

    void FindNearestInteractable()
    {
        //GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        float minDistance = Mathf.Infinity; // �ּ� �Ÿ��� ���Ѵ�� �ʱ�ȭ
        currentInteractable = null;

        foreach (GameObject interactable in interactables) // ��� ��ȣ�ۿ� ������ ������Ʈ�� ��ȸ�ϸ鼭
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position); // ���� ������Ʈ���� �Ÿ��� ���

            if (distance < minDistance && distance <= interactionRange) // ���� �� �Ÿ��� �ּ� �Ÿ����� �۰� ��ȣ�ۿ� ���� ���� �ִٸ�
            {
                minDistance = distance; // �ּ� �Ÿ��� ������Ʈ
                currentInteractable = interactable; // ���� ��ȣ�ۿ� ������ ������Ʈ�� ������Ʈ
                currentInteractableDistance = distance; // ���� ������Ʈ�� �Ÿ� ����
            }
        }
    }

    void InteractWithCurrent() //��ȣ�ۿ� ȣ�� �Լ�
    {
        IFInteractable interactable = currentInteractable.GetComponent<IFInteractable>();

        if (interactable != null)
        {
            interactable.Interact(currentInteractableDistance); // ��ȣ�ۿ� �Լ� ȣ��
            Debug.Log(interactable);
        }
    }
}
