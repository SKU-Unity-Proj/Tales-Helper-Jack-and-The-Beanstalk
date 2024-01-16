using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject mapUI;

    void Start()
    {

    }

    void Update()
    {
        OpenMapUI();
    }

    void OpenMapUI()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if (mapUI.activeSelf == false)
            {
                mapUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                mapUI.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

}
