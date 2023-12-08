using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardGuide : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public TMP_Text tmp_Text;
    public float fadeTime = 2f;
    public GameObject[] TriggerBox;
    //public static float indexText = 0;


    void OnTriggerEnter(Collider col)
    {
        //indexText = indexText + 1;
        if (this.gameObject.name == "F_Trigger")
        {
            textMesh.text = "<color=red>F</color> 대화 및 상호작용";
            StartCoroutine(FadeFullAlpha());

            Invoke("FTrigger", 4.1f);
        }

        else if (this.gameObject.name == "X_Trigger")
        {
            textMesh.text = "<color=red>X</color> 구르기";
            StartCoroutine(FadeFullAlpha());

            Invoke("XTrigger",4.1f);
        }
        
        else if (this.gameObject.name =="C_Trigger")
        {
            textMesh.text = "<color=red>C</color> 앉기";
            StartCoroutine(FadeFullAlpha());

            Invoke("CTrigger", 4.1f);
        }
        
        else if (this.gameObject.name == "Z_Trigger")
        {
            textMesh.text = "<color=red>Z</color> 엎드리기";
            StartCoroutine(FadeFullAlpha());

            Invoke("ZTrigger", 4.1f);
        }
        
    }

    void XTrigger()
    {
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);
        TriggerBox[0].SetActive(false);
    }
    void CTrigger()
    {
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);
        TriggerBox[1].SetActive(false);
    }
    void ZTrigger()
    {
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);
        TriggerBox[2].SetActive(false);
    }
    void FTrigger()
    {
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);
        TriggerBox[3].SetActive(false);
    }




    IEnumerator FadeFullAlpha() // 알파값 0에서 1로 전환
    {
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);
        while (textMesh.color.a < 1.0f)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a + (Time.deltaTime / fadeTime));
            yield return null;
        }
        StartCoroutine(FadeZero());
    }

    IEnumerator FadeZero()  // 알파값 1에서 0으로 전환
    {
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1);
        while (textMesh.color.a > 0.0f)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a - (Time.deltaTime / fadeTime));
            yield return null;
        }
    }
}