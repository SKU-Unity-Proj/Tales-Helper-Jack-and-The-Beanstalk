using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowBeanstalk : MonoBehaviour
{
    public float maxSize;
    public float growRate;
    public float scale;
    public GameObject dust; 

    void start()
    {
        Invoke("DestroyDust", 8f);
    }

    void Update()
    {
        if (scale < maxSize)
        {
            this.transform.localScale = Vector3.one * scale;
            scale += growRate * Time.deltaTime;
        }
    }

    void DestroyDust()
    {
        dust.SetActive(false);
    }
}
