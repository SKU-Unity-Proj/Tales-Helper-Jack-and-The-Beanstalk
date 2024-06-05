using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using DiasGames.Controller;
public class timelineEndcal : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private CinemachineVirtualCamera timelineCam;
    [SerializeField] private GameObject player;

    private CSPlayerController playerMovement;

    void Start()
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

        DisablePlayerMovement();
    }

    private void OnTimelinePlayed(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            Debug.Log("Ÿ�Ӷ��� ����� ���۵Ǿ����ϴ�.");
            
        }
    }

    private void OnTimelineStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            Debug.Log("Ÿ�Ӷ��� ����� �������ϴ�.");
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
        if (director != null)
        {
            director.stopped -= OnTimelineStopped;
            director.played -= OnTimelinePlayed; // Ÿ�Ӷ��� ���� �̺�Ʈ ����
        }
    }
}
