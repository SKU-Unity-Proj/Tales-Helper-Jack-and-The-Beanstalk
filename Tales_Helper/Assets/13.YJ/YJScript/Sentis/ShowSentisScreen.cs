using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShowSentisScreen : MonoBehaviour
{
    private bool playerIn = false;
    private CinemachineVirtualCamera screenCam;
    public Texture2D circleCursor;
    private bool isShow = false;

    private void Start()
    {
        screenCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        playerIn = true;
    }
    private void OnTriggerExit(Collider other)
    {
        playerIn = false;
    }

    private void Update()
    {
        if (playerIn)
        {
            if(!isShow)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    screenCam.Priority = 11;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    Cursor.SetCursor(circleCursor, new Vector2(16, 16), CursorMode.Auto);
                    isShow = true;
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.F) && isShow)
            {
                screenCam.Priority = 2;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                isShow = false;
            }
        }
    }
}
