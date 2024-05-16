using UnityEngine;
using static IFKeyInteractable;

public class FInteractionFinder : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.F;
    public float interactionRange = 3f;

    public GameObject currentInteractable;

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            FindNearestInteractable();
            if (currentInteractable != null)
            {
                InteractWithCurrent();
            }
        }
    }

    void FindNearestInteractable()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        float minDistance = Mathf.Infinity;
        currentInteractable = null;

        foreach (GameObject interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (distance < minDistance && distance <= interactionRange)
            {
                minDistance = distance;
                currentInteractable = interactable;
            }
        }
    }

    void InteractWithCurrent()
    {
        IFInteractable interactable = currentInteractable.GetComponent<IFInteractable>();

        if (interactable != null)
        {
            interactable.Interact();
            Debug.Log(interactable);
        }
    }
}
