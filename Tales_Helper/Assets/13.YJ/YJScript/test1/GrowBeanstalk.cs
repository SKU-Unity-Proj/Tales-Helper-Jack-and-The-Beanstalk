using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GrowBeanstalk : MonoBehaviour  //BeanStalk ������ �޸� ������Ʈ�� ����
{
    public float maxSize;
    public float growRate;
    private float scale = 1f;
    public GameObject dust;
    public GameObject inventory; //�κ��丮�� main UI �ֱ�
    public GameObject inventoryOption; //�κ��丮�� slot Options �ֱ�

    public PlayableDirector playableDirector;

    void Start()
    {
        InventoryOff();
        PlantBean();
        StartCoroutine(GrowBean());
        Invoke("CameraShakeStart", 11f);
    }
    /*
    void Update()
    {
        //�� �ڶ�� dust ����
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
        yield return new WaitForSeconds(11f);

        dust.SetActive(true);

        while (scale < maxSize)
        {
            this.transform.localScale = Vector3.one * scale;
            scale += growRate * Time.deltaTime;

            if (scale >= maxSize)
            {
                dust.SetActive(false);
                break;
            }

            yield return null; // �� �����Ӿ� ���
        }
    }

    void CameraShakeStart()
    {
        Debug.Log("CameraShake Play");

        CameraShakeManager.Instance.SetShakeDegree(2f,2f);
        CameraShakeManager.Instance.SetShakeTime(6.4f);
    }

    void InventoryOff()
    {
        inventory.SetActive(false);
        inventoryOption.SetActive(false);
    }
}
