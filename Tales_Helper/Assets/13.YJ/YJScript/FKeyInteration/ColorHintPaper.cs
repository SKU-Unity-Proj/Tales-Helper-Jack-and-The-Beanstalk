using UnityEngine;
using static IFKeyInteractable;

public class ColorHintPaper : MonoBehaviour, IFInteractable
{
    public float canDistance = 4f; // 상호작용 가능한 거리

    public GameObject paperImage;

    public void Interact(float distance)
    {
        if (distance < canDistance)
        {
            paperImage.SetActive(!paperImage.activeSelf);
        }
        else
        {
            if(paperImage.activeSelf)
                paperImage.SetActive(false);
        }
    }
}
