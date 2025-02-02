using UnityEngine;
using TMPro;

public class WeightCalculate : MonoBehaviour
{
    public float totalWeight = 0f;
    public float answerWeight = 15f;
    private float targetWeight = 0f;
    private float weightChangeSpeed = 6f;

    public TextMeshProUGUI weightText; //무게를 표시할 Text

    public Animator puppetAnim;
    public BoxCollider bedroomLock;

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

        weightText.text = totalWeight.ToString("F2"); // 소수점 2자리까지 연동

        AnswerWeight();
    }

    void AnswerWeight()
    {
        if(totalWeight == answerWeight)
        {
            puppetAnim.SetTrigger("Fall");

            // 침실로 가는 자물쇠 활성화

            bedroomLock.enabled = true;
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
