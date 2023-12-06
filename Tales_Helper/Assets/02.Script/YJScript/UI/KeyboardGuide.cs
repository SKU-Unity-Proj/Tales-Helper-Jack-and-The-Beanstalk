using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardGuide : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float fadeTime = 2f;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
            StartCoroutine(FadeFullAlpha());
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