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
        // �̱��� ������ ����Ͽ� �ν��Ͻ��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� ���ο� ������Ʈ�� �ı�
        }
    }

    private void Start()
    {
        if (currentEpisodeNum > 0 && currentEpisodeNum < startPositions.Length)
        {
            // �÷��̾��� ��ġ�� ���� ���Ǽҵ忡 �ش��ϴ� ��ġ�� �̵�
            player.position = startPositions[currentEpisodeNum];
        }
        else
        {
            Debug.LogError("���� ���Ǽҵ� ��ȣ�� ��ȿ�� ������ ������ϴ�.");
        }

        // ������Ʈ ����
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
                Debug.Log("Episode 1 ���� ����");
                SetEpisode1Settings();
                break;

            case 2:
                Debug.Log("Episode 2 ���� ����");
                SetEpisode2Settings();
                break;

            case 3:
                Debug.Log("Episode 3 ���� ����");
                SetEpisode3Settings();
                break;

            default:
                Debug.Log("�� �� ���� Episode ����");
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
