using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public RoomDataList roomDataList; // �� ������ ����Ʈ
    private int currentRoomIndex = 0;
    private bool conditionMet = false; // ������ �����Ǿ����� ����

    private PlayableDirector playableDirector;

    void Start()
    {
        LoadProgress();
        LoadCurrentRoom();
    }

    void Update()
    {
        CheckCondition();
    }

    public void LoadCurrentRoom()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex];
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentRoom.playerStartPosition;
        }
        conditionMet = false; // ���� �ʱ�ȭ

        // PlayableDirector ���� (Ÿ�Ӷ����� �ִ� ���)
        if (currentRoom.completionCondition == "pushLever")
        {
            playableDirector = GameObject.FindObjectOfType<PlayableDirector>();
            if (playableDirector != null)
            {
                playableDirector.stopped += OnPlayableDirectorStopped;
            }
        }
    }

    public void CompleteRoom()
    {
        currentRoomIndex++;
        if (currentRoomIndex < roomDataList.rooms.Count)
        {
            SaveProgress();
            LoadCurrentRoom();
        }
        else
        {
            Debug.Log("��� ���� Ŭ�����߽��ϴ�!");
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentRoomIndex", currentRoomIndex);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey("CurrentRoomIndex"))
        {
            currentRoomIndex = PlayerPrefs.GetInt("CurrentRoomIndex");
        }
    }

    private void CheckCondition()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex];
        if (!conditionMet)
        {
            switch (currentRoom.completionCondition)
            {
                case "pushLever":
                    // Ÿ�Ӷ��� ���� ���δ� OnPlayableDirectorStopped �̺�Ʈ���� ó��
                    break;
                case "collectItem":
                    // ������ ���� ������ üũ
                    break;
                // �߰� ������ ���⼭ ó��
                default:
                    break;
            }
        }
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if (playableDirector != null && director == playableDirector)
        {
            conditionMet = true;
            CompleteRoom();
        }
    }

    private void OnDestroy()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;
        }
    }
    public void ResetPlayerPosition()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex];
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentRoom.playerStartPosition;
        }
    }
}
