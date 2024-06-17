using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public RoomDataList roomDataList; // 방 데이터 리스트
    private int currentRoomIndex = 0; // 현재 방 인덱스
    private bool conditionMet = false; // 조건이 만족되었는지 여부

    private PlayableDirector playableDirector; // 타임라인 제어를 위한 PlayableDirector

    void Start()
    {
        LoadProgress(); // 진행 상황 로드
        LoadCurrentRoom(); // 현재 방 로드
    }

    void Update()
    {
        CheckCondition(); // 조건 확인
    }

    public void LoadCurrentRoom()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex]; // 현재 방 데이터 로드
        GameObject player = GameObject.FindWithTag("Player"); // 플레이어 객체 찾기
        if (player != null)
        {
            player.transform.position = currentRoom.playerStartPosition; // 플레이어 위치 설정
        }
        conditionMet = false; // 조건 초기화

        // PlayableDirector 설정 (타임라인이 있는 경우)
        if (currentRoom.completionCondition == "pushLever") // 조건이 레버를 당기는 경우
        {
            playableDirector = GameObject.FindObjectOfType<PlayableDirector>(); // PlayableDirector 찾기
            if (playableDirector != null)
            {
                playableDirector.stopped += OnPlayableDirectorStopped; // 타임라인이 멈출 때 이벤트 추가
            }
        }
    }

    public void CompleteRoom()
    {
        currentRoomIndex++; // 현재 방 인덱스 증가
        if (currentRoomIndex < roomDataList.rooms.Count) // 아직 남은 방이 있는 경우
        {
            SaveProgress(); // 진행 상황 저장
            LoadCurrentRoom(); // 다음 방 로드
        }
        else
        {
            Debug.Log("모든 방을 클리어했습니다!"); // 모든 방을 클리어한 경우 메시지 출력
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentRoomIndex", currentRoomIndex); // 현재 방 인덱스 저장
        PlayerPrefs.Save(); // 저장
    }

    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey("CurrentRoomIndex")) // 저장된 진행 상황이 있는 경우
        {
            currentRoomIndex = PlayerPrefs.GetInt("CurrentRoomIndex"); // 진행 상황 로드
        }
    }

    private void CheckCondition()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex]; // 현재 방 데이터 로드
        if (!conditionMet)
        {
            switch (currentRoom.completionCondition) // 현재 방의 완료 조건 확인
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
        if (playableDirector != null && director == playableDirector) // 타임라인이 멈췄을 때
        {
            conditionMet = true; // 조건 충족
            CompleteRoom(); // 방 완료 처리
        }
    }

    private void OnDestroy()
    {
        if (playableDirector != null) // PlayableDirector가 설정되어 있는 경우
        {
            playableDirector.stopped -= OnPlayableDirectorStopped; // 이벤트 해제
        }
    }

    public void ResetPlayerPosition()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex]; // 현재 방 데이터 로드
        GameObject player = GameObject.FindWithTag("Player"); // 플레이어 객체 찾기
        if (player != null)
        {
            player.transform.position = currentRoom.playerStartPosition; // 플레이어 위치 재설정
        }
    }
}
