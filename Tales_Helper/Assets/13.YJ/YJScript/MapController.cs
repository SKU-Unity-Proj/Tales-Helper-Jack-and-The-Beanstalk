using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public GameObject mapUI;
    public Image mapSignature;
    public Image[] miniMap;
    public Sprite[] miniMapType;
    public Sprite[] mapState;
    private int currentIndex = 0;

    #region 싱글톤 패턴
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
        CurrentRoom();
    }

    public void ChangeImage(int newIndex)
    {
        currentIndex = newIndex;


    }

    void CurrentRoom() // 현재 위치한 방의 색상 변경
    {
        mapSignature.sprite = mapState[currentIndex];
    }

    void OpenCloseMap()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if (mapUI.activeSelf == false)
            {
                mapUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                mapUI.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

}
