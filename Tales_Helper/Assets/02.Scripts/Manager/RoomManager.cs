using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public RoomDataList roomDataList; // 방 데이터 리스트
    private int currentRoomIndex = 0;
    private bool conditionMet = false; // 조건이 만족되었는지 여부

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
        conditionMet = false; // 조건 초기화

        // PlayableDirector 설정 (타임라인이 있는 경우)
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
            Debug.Log("모든 방을 클리어했습니다!");
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
                    // 타임라인 실행 여부는 OnPlayableDirectorStopped 이벤트에서 처리
                    break;
                case "collectItem":
                    // 아이템 수집 조건을 체크
                    break;
                // 추가 조건은 여기서 처리
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
