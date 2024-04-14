using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public GameObject mapUI; // ���� ų �� UI

    public Image mapPicture; // �� �׸�
    public Sprite[] mapPictureType; // �� �׸� ����
    public Image[] miniMap; // �̴ϸ�
    public Sprite[] miniMapType; // �̴ϸ� ����

    private int RoomNumber = 0; // ���� �� ��ȣ
    private int UnlockRoomNum = 0; // ����� �ִ� �� ��ȣ
    private int SaveRoomNumber = 0; // ���� ���� ų �� ���� ��ѹ��� �����س��ٰ� �ʱ�ȭ ��Ű�� �뵵

    #region �̱��� ����
    private static MapController instance = null;
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
    public static MapController Instance
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
    }

    public void CurrentRoom(int RoomNum) // ���� ��ġ�� ��
    {
        miniMap[RoomNum].sprite = miniMapType[1]; // �̴ϸ� ���

        mapPicture.sprite = mapPictureType[RoomNum]; // �� �̹��� ����

        miniMap[RoomNum].color = Color.green; // ��ġ�� �� �̴ϸ� ���� ����
        miniMap[RoomNumber].color = Color.white;

        RoomNumber = RoomNum; // �� ��ȣ �ٽ� ����

        if(RoomNum > UnlockRoomNum) // ���ο� ���̸�
        {
            UnlockRoomNum = RoomNum; // ����� �ִ� �� ��ȣ ������Ʈ
        }
    }

    public void NextRoomButton()
    {
        if(RoomNumber < UnlockRoomNum)
        {
            CurrentRoom(RoomNumber + 1);
        }
    }
    public void BeforeRoomButton()
    {
        if (RoomNumber > 0)
        {
            CurrentRoom(RoomNumber - 1);
        }
    }

    void OpenCloseMap() //�� ���� �ݱ�
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if (mapUI.activeSelf == false) // �ѱ�
            {
                mapUI.SetActive(true); 
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                SaveRoomNumber = RoomNumber; // ���� �� ��ȣ ����
            }
            else // ����
            {
                mapUI.SetActive(false); 
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                RoomNumber = SaveRoomNumber; // ���� ������ �ٽ� �ʱ�ȭ
            }
        }
    }

}
