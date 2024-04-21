using UnityEngine;
using TMPro;

public class WeightCalculate : MonoBehaviour
{
    public float totalWeight = 0f;
    public float answerWeight = 15f;
    private float targetWeight = 0f;
    private float weightChangeSpeed = 6f;

    public TextMeshProUGUI weightText; //���Ը� ǥ���� Text

    private void Update()
    {
        if (totalWeight < targetWeight)
        {
            totalWeight += weightChangeSpeed * Time.deltaTime;
            if (totalWeight > targetWeight)
                totalWeight = targetWeight;
        }
        else if (totalWeight > targetWeight)
        {
            totalWeight -= weightChangeSpeed * Time.deltaTime;
            if (totalWeight < targetWeight)
                totalWeight = targetWeight;
        }

        weightText.text = totalWeight.ToString("F2"); // �Ҽ��� 2�ڸ����� ����

        AnswerWeight();
    }

    void AnswerWeight()
    {
        if(totalWeight == answerWeight)
        {
            Debug.Log("suc");
        }
    }

    public void AddWeight(float objWeight)
    {
        targetWeight += objWeight;
    }

    public void RemoveWeight(float objWeight)
    {
        targetWeight -= objWeight;
    }
}
