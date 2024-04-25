using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public GameObject lightGO;
    private bool isOn = false; 

    void Start()
    {
        if(lightGO == null)
        {
            lightGO = GameObject.Find("ZombieStopLight"); //캐릭터 모델 안에 있음
        }

        //lightGO.SetActive(isOn);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            isOn = !isOn;

            if (isOn)
                lightGO.SetActive(true);
            else
                lightGO.SetActive(false);
        }
    }
}
