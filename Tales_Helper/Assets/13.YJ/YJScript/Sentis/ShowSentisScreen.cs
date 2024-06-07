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
        if (other.CompareTag("Player")) // �÷��̾����� Ȯ�� (�ʿ��� ���)
        {
            playerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾����� Ȯ�� (�ʿ��� ���)
        {
            playerIn = false;
            HideScreen(); // Ʈ���� �ڽ��� ����� �� �ʱ�ȭ
        }
    }

    private void Update()
    {
        if (playerIn)
        {
            if (!isShow)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    ShowScreen();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    HideScreen();
                }
            }
        }
    }

    private void ShowScreen()
    {
        screenCam.Priority = 11;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Cursor.SetCursor(circleCursor, new Vector2(16, 16), CursorMode.Auto);
        isShow = true;
    }

    private void HideScreen()
    {
        screenCam.Priority = 2;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isShow = false;
    }
}
