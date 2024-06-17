using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public RoomDataList roomDataList; // �� ������ ����Ʈ
    private int currentRoomIndex = 0; // ���� �� �ε���
    private bool conditionMet = false; // ������ �����Ǿ����� ����

    private PlayableDirector playableDirector; // Ÿ�Ӷ��� ��� ���� PlayableDirector

    void Start()
    {
        LoadProgress(); // ���� ��Ȳ �ε�
        LoadCurrentRoom(); // ���� �� �ε�
    }

    void Update()
    {
        CheckCondition(); // ���� Ȯ��
    }

    public void LoadCurrentRoom()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex]; // ���� �� ������ �ε�
        GameObject player = GameObject.FindWithTag("Player"); // �÷��̾� ��ü ã��
        if (player != null)
        {
            player.transform.position = currentRoom.playerStartPosition; // �÷��̾� ��ġ ����
        }
        conditionMet = false; // ���� �ʱ�ȭ

        // PlayableDirector ���� (Ÿ�Ӷ����� �ִ� ���)
        if (currentRoom.completionCondition == "pushLever") // ������ ������ ���� ���
        {
            playableDirector = GameObject.FindObjectOfType<PlayableDirector>(); // PlayableDirector ã��
            if (playableDirector != null)
            {
                playableDirector.stopped += OnPlayableDirectorStopped; // Ÿ�Ӷ����� ���� �� �̺�Ʈ �߰�
            }
        }
    }

    public void CompleteRoom()
    {
        currentRoomIndex++; // ���� �� �ε��� ����
        if (currentRoomIndex < roomDataList.rooms.Count) // ���� ���� ���� �ִ� ���
        {
            SaveProgress(); // ���� ��Ȳ ����
            LoadCurrentRoom(); // ���� �� �ε�
        }
        else
        {
            Debug.Log("��� ���� Ŭ�����߽��ϴ�!"); // ��� ���� Ŭ������ ��� �޽��� ���
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentRoomIndex", currentRoomIndex); // ���� �� �ε��� ����
        PlayerPrefs.Save(); // ����
    }

    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey("CurrentRoomIndex")) // ����� ���� ��Ȳ�� �ִ� ���
        {
            currentRoomIndex = PlayerPrefs.GetInt("CurrentRoomIndex"); // ���� ��Ȳ �ε�
        }
    }

    private void CheckCondition()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex]; // ���� �� ������ �ε�
        if (!conditionMet)
        {
            switch (currentRoom.completionCondition) // ���� ���� �Ϸ� ���� Ȯ��
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
        if (playableDirector != null && director == playableDirector) // Ÿ�Ӷ����� ������ ��
        {
            conditionMet = true; // ���� ����
            CompleteRoom(); // �� �Ϸ� ó��
        }
    }

    private void OnDestroy()
    {
        if (playableDirector != null) // PlayableDirector�� �����Ǿ� �ִ� ���
        {
            playableDirector.stopped -= OnPlayableDirectorStopped; // �̺�Ʈ ����
        }
    }

    public void ResetPlayerPosition()
    {
        RoomData currentRoom = roomDataList.rooms[currentRoomIndex]; // ���� �� ������ �ε�
        GameObject player = GameObject.FindWithTag("Player"); // �÷��̾� ��ü ã��
        if (player != null)
        {
            player.transform.position = currentRoom.playerStartPosition; // �÷��̾� ��ġ �缳��
        }
    }
}
