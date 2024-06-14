using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GrowBeanstalk : MonoBehaviour  //BeanStalk 하위에 달린 오브젝트에 부착
{
    public float maxSize;
    public float growRate;
    private float scale = 1f;
    //public GameObject dust;
    public GameObject inventory; //인벤토리에 main UI 넣기
    public GameObject inventoryOption; //인벤토리에 slot Options 넣기

    public PlayableDirector playableDirector;
    public GameObject useBeanGuideText;

    void Start()
    {
        useBeanGuideText.SetActive(false);

        InventoryOff();
        PlantBean();
        StartCoroutine(GrowBean());
        Invoke("CameraShakeStart", 11f);
    }
    /*
    void Update()
    {
        //다 자라면 dust 끄기
        if(dust.activeSelf)
        {
            if(scale >= maxSize) 
            {
                dust.SetActive(false);
            }
        }
    }
    */
    void PlantBean()
    {
        Debug.Log("Timeline Play");
        playableDirector.Play();
    }

    IEnumerator GrowBean()
    {
        yield return new WaitForSeconds(13f);

        //dust.SetActive(true);

        while (scale < maxSize)
        {
            this.transform.localScale = Vector3.one * scale;
            scale += growRate * Time.deltaTime;

            if (scale >= maxSize)
            {
                //dust.SetActive(false);
                break;
            }

            yield return null; // 한 프레임씩 대기
        }
    }

    void CameraShakeStart()
    {
        Debug.Log("CameraShake Play");

        CameraShakeManager.Instance.SetShakeDegree(1.5f,1.5f);
        CameraShakeManager.Instance.SetShakeTime(8.8f);
    }

    void InventoryOff()
    {
        inventory.SetActive(false);
        inventoryOption.SetActive(false);
    }
}
