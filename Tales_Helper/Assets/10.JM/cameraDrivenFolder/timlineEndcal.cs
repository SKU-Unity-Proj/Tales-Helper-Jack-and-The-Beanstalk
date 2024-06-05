using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class timelineEndcal : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private CinemachineVirtualCamera timelineCam;

    void Start()
    {
        if (director != null)
        {
            director.stopped += OnTimelineStopped;
        }
    }

    private void OnTimelineStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            Debug.Log("타임라인 재생이 끝났습니다.");
            TimeLineCamPriority();
        }
    }

    void TimeLineCamPriority()
    {
        timelineCam.Priority = 0;
    }

    private void OnDestroy()
    {
        if (director != null)
        {
            director.stopped -= OnTimelineStopped;
        }
    } 
}
