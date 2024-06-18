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
            Invoke("PlayerPositionSetting", 1f);
                
        }
        else
        {
            Debug.LogError("���� ���Ǽҵ� ��ȣ�� ��ȿ�� ������ ������ϴ�.");
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
                Debug.LogError("Episode 1 ���� ����");
                SetEpisode1Settings();
                break;

            case 2:
                Debug.LogError("Episode 2 ���� ����");
                SetEpisode2Settings();
                break;

            case 3:
                Debug.LogError("Episode 3 ���� ����");
                SetEpisode3Settings();
                break;

            default:
                Debug.LogError("�� �� ���� Episode ����");
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
