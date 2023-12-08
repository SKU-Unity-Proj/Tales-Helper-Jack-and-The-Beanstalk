using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [SerializeField] Image image;

    void Start()
    {
        StartCoroutine(FadeOut(1.5f, image));
    }

    public IEnumerator FadeOut(float f, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);

        while (i.color.a > 0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / f));
            yield return null;
        }

        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
    }

    public IEnumerator FadeIn(float f, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);

        while (i.color.a < 0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / f));
            yield return null;
        }

        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
    }

}
