using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public Transform player;
    public int currentEpisodeNum = 0;
    
    public Vector3[] startPositions;
    public GameObject[] episodeObjects;

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
            player.position = startPositions[currentEpisodeNum];
        }
        else
        {
            Debug.LogError("현재 에피소드 번호가 유효한 범위를 벗어났습니다.");
        }

        // 오브젝트 세팅
        for(int i = 0; i< currentEpisodeNum; i++)
        {
            EpisodeSetting(i);
        }
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
                Debug.Log("Episode 1 설정 적용");
                SetEpisode1Settings();
                break;

            case 2:
                Debug.Log("Episode 2 설정 적용");
                SetEpisode2Settings();
                break;

            case 3:
                Debug.Log("Episode 3 설정 적용");
                SetEpisode3Settings();
                break;

            default:
                Debug.Log("알 수 없는 Episode 설정");
                break;
        }
    }

    void SetEpisode1Settings()
    {
        episodeObjects[0].SetActive(false);
        episodeObjects[1].SetActive(false);
    }

    void SetEpisode2Settings()
    {

    }

    void SetEpisode3Settings()
    {

    }
}
