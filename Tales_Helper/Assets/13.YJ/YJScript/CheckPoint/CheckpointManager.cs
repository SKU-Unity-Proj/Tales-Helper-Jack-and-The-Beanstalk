using System.Collections.Generic;
using UnityEngine;
//Vector3(-12.1000004,-3.79999995,101.900002)
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public Transform player;
    public int currentEpisodeNum = 0;
    
    public Vector3[] startPositions;

    void Awake()
    {
        // 싱글턴 패턴을 사용하여 인스턴스를 관리
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트가 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 새로운 오브젝트를 파괴
        }
    }

    private void Start()
    {
        if (currentEpisodeNum > 0 && currentEpisodeNum < startPositions.Length)
        {
            // 플레이어의 위치를 현재 에피소드에 해당하는 위치로 이동
            Invoke("PlayerPositionSetting", 1f);
                
        }
        else
        {
            Debug.LogError("현재 에피소드 번호가 유효한 범위를 벗어났습니다.");
        }

        EpisodeSetting(currentEpisodeNum);
    }

    public void ClearEpisodeUpdate(int clearEpisode)
    {
        if(currentEpisodeNum < clearEpisode)
            currentEpisodeNum = clearEpisode;
    }

    void EpisodeSetting(int episode)
    {
        switch (episode)
        {
            case 1:
                Debug.LogError("Episode 1 설정 적용");
                SetEpisode1Settings();
                break;

            case 2:
                Debug.LogError("Episode 2 설정 적용");
                SetEpisode2Settings();
                break;

            case 3:
                Debug.LogError("Episode 3 설정 적용");
                SetEpisode3Settings();
                break;

            default:
                Debug.LogError("알 수 없는 Episode 설정");
                break;
        }
    }

    void SetEpisode1Settings()
    {
        GameObject episodeObject_Door = GameObject.Find("Door_Episode");
        episodeObject_Door.SetActive(false);

        GameObject episodeObject_Giant = GameObject.Find("LivingGiant");
        Debug.Log(episodeObject_Giant);
        episodeObject_Giant.SetActive(false);
    }

    void SetEpisode2Settings()
    {
        GameObject episodeObject_Mouse = GameObject.Find("Mouse");
        episodeObject_Mouse.SetActive(false);
    }

    void SetEpisode3Settings()
    {
        Liftmove episodeObject_Elevator = GameObject.Find("lift").GetComponent<Liftmove>();
        episodeObject_Elevator.enabled = true;
    }

    void PlayerPositionSetting()
    {
        player = GameObject.Find("CS Character Controller").GetComponent<Transform>();
        player.localPosition = startPositions[currentEpisodeNum];
    }
}
