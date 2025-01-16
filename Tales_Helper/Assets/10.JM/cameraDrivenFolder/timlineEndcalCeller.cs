using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using DiasGames.Controller;
using System.Collections.Generic;

public class timelineEndcalCeller : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private CinemachineVirtualCamera timelineCam;
    [SerializeField] private CinemachineVirtualCamera cellertimelineCam;
    [SerializeField] private CinemachineVirtualCamera cellertimelineCam2;
    [SerializeField] private GameObject player;
    [SerializeField] private float holdDuration = 3f; // FŰ�� ������ �־�� �ϴ� �ð�

    [SerializeField] private List<CinemachineVirtualCamera> timelineCams = new List<CinemachineVirtualCamera>();

    private CSPlayerController playerMovement;
    private bool isHoldingKey = false;
    private float holdTime = 0f;

    private void Start()
    {
        if (director != null)
        {
            director.stopped += OnTimelineStopped;
            director.played += OnTimelinePlayed; // Ÿ�Ӷ��� ���� �̺�Ʈ �߰�
        }

        if (player != null)
        {
            playerMovement = player.GetComponent<CSPlayerController>();
        }
        EnablePlayerMovement();
    }

    private void Update()
    {
        if (director != null && director.state == PlayState.Playing)
        {
            if (Input.GetKey(KeyCode.F))
            {
                if (!isHoldingKey)
                {
                    isHoldingKey = true;
                    holdTime = 0f;
                }
                holdTime += Time.deltaTime;

                if (holdTime >= holdDuration)
                {
                    director.Stop();
                    ResetCameraPriorities();
                }
            }
            else
            {
                isHoldingKey = false;
                holdTime = 0f;
            }
        }
    }

    private void ResetCameraPriorities()
    {
        foreach (var cam in timelineCams)
        {
            cam.Priority = 0;
        }
    }

    private void OnTimelinePlayed(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            Debug.Log("Ÿ�Ӷ��� ����� ���۵Ǿ����ϴ�.");
            DisablePlayerMovement();
        }
    }

    private void OnTimelineStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            Debug.Log("Ÿ�Ӷ��� ����� �������ϴ�.");
            TimeLineCamPriority();
            EnablePlayerMovement();
            director.enabled = false;
        }
    }

    void TimeLineCamPriority()
    {
        timelineCam.Priority = 0;
        cellertimelineCam.Priority = 0;
        cellertimelineCam2.Priority = 0;
    }

    void DisablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    void EnablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void OnDestroy()
    {
        if (director != null)
        {
            director.stopped -= OnTimelineStopped;
            director.played -= OnTimelinePlayed; // Ÿ�Ӷ��� ���� �̺�Ʈ ����
        }
    }
}