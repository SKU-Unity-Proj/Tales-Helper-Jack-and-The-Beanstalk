using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShowSentisScreen : MonoBehaviour
{
    private bool playerIn = false;
    public CinemachineVirtualCamera screenCam;
    public Texture2D circleCursor;

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
            if(Input.GetKeyDown(KeyCode.F)&& playerIn)
            {
                screenCam.Priority = 11;
                Cursor.lockState = CursorLockMode.None;
                Cursor.SetCursor(circleCursor, new Vector2(16, 16), CursorMode.Auto);
                Cursor.visible = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                screenCam.Priority = 2;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
