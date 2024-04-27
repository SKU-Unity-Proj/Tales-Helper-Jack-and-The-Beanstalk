using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapController_Backup : MonoBehaviour
{
    public GameObject mapUI; // 껏다 킬 맵 UI

    public Image mapPicture; // 방 그림
    public Sprite[] mapPictureType; // 방 그림 종류
    public Image[] miniMap; // 미니맵
    public Sprite[] miniMapType; // 미니맵 종류
    public TextMeshProUGUI mapText; // 방 이름
    private string[] mapTextType = { "1. Entrance", "2. LivingRoom", "3. DinningRoom", "4. Kitchen", "5. Storage" };

    public int RoomNumber = 0; // 현재 방 번호
    private int UnlockRoomNum = 0; // 언락된 최대 방 번호
    private int SaveRoomNumber = 0; // 맵을 껏다 킬 때 현재 룸넘버를 저장해놨다가 초기화 시키는 용도

    #region 싱글톤 패턴
    private static MapController_Backup instance = null;
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    public static MapController_Backup Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion

    void Update()
    {
        OpenCloseMap();
        LinkRoomNumber();
    }

    public void CurrentRoom(int RoomNum) // 현재 위치한 방
    {
        miniMap[RoomNum].sprite = miniMapType[1]; // 미니맵 언락

        RoomNumber = RoomNum; // 방 번호 다시 저장

        if (RoomNum > UnlockRoomNum) // 새로운 방이면
        {
            UnlockRoomNum = RoomNum; // 언락된 최대 방 번호 업데이트
        }
    }

    private void LinkRoomNumber()
    {
        mapPicture.sprite = mapPictureType[RoomNumber]; // 맵 이미지 변경
        mapText.text = mapTextType[RoomNumber]; // 맵 이름 변경
        miniMap[RoomNumber].color = Color.green; // 위치한 방 미니맵 색상 변경
    }

    public void ColorChange(int RoomNum)
    {
        miniMap[RoomNum].color = Color.white;
    }

    public void NextRoomButton()
    {
        if (RoomNumber < UnlockRoomNum)
        {
            RoomNumber++;
            ColorChange(RoomNumber - 1);
        }
    }
    public void BeforeRoomButton()
    {
        if (RoomNumber > 0)
        {
            RoomNumber--;
            ColorChange(RoomNumber + 1);
        }
    }

    void OpenCloseMap() //맵 열고 닫기
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (mapUI.activeSelf == false) // 켜기
            {
                mapUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                SaveRoomNumber = RoomNumber; // 현재 방 번호 저장
            }
            else // 끄기
            {
                mapUI.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                RoomNumber = SaveRoomNumber; // 현재 방으로 다시 초기화
            }
        }
    }
}
