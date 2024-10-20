using UnityEngine;

public class ObjectWeight : MonoBehaviour
{
    public float objWeight = 0;
    public WeightCalculate weightCalculate;

    private void Start()
    {
        if (weightCalculate == null)
        {
            weightCalculate = FindObjectOfType<WeightCalculate>();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("AddWeight"))
        {
            weightCalculate.AddWeight(objWeight);
        }

        if (collision.gameObject.CompareTag("RemoveWeight"))
        {
            weightCalculate.RemoveWeight(objWeight);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("AddWeight"))
        {
            weightCalculate.RemoveWeight(objWeight);
        }

        if (collision.gameObject.CompareTag("RemoveWeight"))
        {
            weightCalculate.AddWeight(objWeight);
        }
    }
}
