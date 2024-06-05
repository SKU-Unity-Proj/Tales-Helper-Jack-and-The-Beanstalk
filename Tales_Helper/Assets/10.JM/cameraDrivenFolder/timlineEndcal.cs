using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using System.Collections.Generic;
using DiasGames.Controller;

public class timelineEndcal : MonoBehaviour
{
    [SerializeField] private List<PlayableDirector> directors;
    [SerializeField] private CinemachineVirtualCamera timelineCam;
    [SerializeField] private GameObject player;

    private CSPlayerController playerMovement;

    void Start()
    {
        if (player != null)
        {
            playerMovement = player.GetComponent<CSPlayerController>();
        }

        foreach (var director in directors)
        {
            if (director != null)
            {
                director.stopped += OnTimelineStopped;
                director.played += OnTimelinePlayed;
            }
        }
    }

    private void OnTimelinePlayed(PlayableDirector aDirector)
    {
        if (directors.Contains(aDirector))
        {
            Debug.Log("타임라인 재생이 시작되었습니다.");
            DisablePlayerMovement();
        }
    }

    private void OnTimelineStopped(PlayableDirector aDirector)
    {
        if (directors.Contains(aDirector))
        {
            Debug.Log("타임라인 재생이 끝났습니다.");
            TimeLineCamPriority();
            EnablePlayerMovement();
        }
    }

    void TimeLineCamPriority()
    {
        timelineCam.Priority = 0;
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
        foreach (var director in directors)
        {
            if (director != null)
            {
                director.stopped -= OnTimelineStopped;
                director.played -= OnTimelinePlayed;
            }
        }
    }
}
