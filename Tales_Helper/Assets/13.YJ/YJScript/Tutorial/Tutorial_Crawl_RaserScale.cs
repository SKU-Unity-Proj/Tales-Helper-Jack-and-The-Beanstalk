using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Crawl_RaserScale : MonoBehaviour
{
    void OnTriggerStay(Collider col)
    {
        if (col.tag == ("TutorialCrawl"))
        {
            transform.localScale = new Vector3(0.68f, 1f, 1f);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == ("TutorialCrawl"))
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
