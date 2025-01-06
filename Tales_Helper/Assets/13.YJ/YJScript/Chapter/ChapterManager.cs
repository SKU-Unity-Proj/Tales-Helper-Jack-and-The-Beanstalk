using System.Collections;
using System.IO;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    static GameObject container;

    public GameObject player;
    public GameObject[] gameObjects;
    int highChapter = 0;

    #region 싱글톤
    // ---싱글톤으로 선언--- //
    static ChapterManager instance;
    public static ChapterManager Instance
    {
        get
        {
            if (!instance)
            {
                container = new GameObject();
                container.name = "DataManager";
                instance = container.AddComponent(typeof(ChapterManager)) as ChapterManager;
                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }
    #endregion

    // --- 게임 데이터 파일이름 설정 ("원하는 이름(영문).json") --- //
    string GameDataFileName = "GameData.json";

    // --- 저장용 클래스 변수 --- //
    public Data data = new Data();

    private void Start()
    {
        LoadGameData(); // 게임 시작 시 저장된 데이터 불러오기
    }

    // 불러오기
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            // 저장된 파일 읽어오고 Json을 클래스 형식으로 전환해서 할당
            string FromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(FromJsonData);
            print("불러오기 완료");
        }

        GetHighestClearedChapterIndex();
        StartCoroutine(SettingPosition());
    }


    // 저장하기
    public void SaveGameData()
    {
        // 클래스를 Json 형식으로 전환 (true : 가독성 좋게 작성)
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, ToJsonData);

        // 올바르게 저장됐는지 확인 (자유롭게 변형)
        print("저장 완료");

        for (int i = 0; i < data.isChapterCleared.Length; i++)
        {
            print($"{i}번 챕터 잠금 해제 여부 : " + data.isChapterCleared[i]);
        }
    }

    public void UnlockChapter(int chapterNum)
    {
        data.isChapterCleared[chapterNum] = true;

        SaveGameData();
    }

    //위치 및 상황 세팅하기
    IEnumerator SettingPosition()
    {
        yield return new WaitForSeconds(0.1f);
        switch (highChapter)
        {
            case 0:
                player.transform.localPosition = new Vector3(-1.3f, 3.7f, 104.2f);
                gameObjects[0].SetActive(false);
                gameObjects[1].GetComponent<Lock>().SettingChapter();
                Debug.Log("SettingChapter : " + highChapter);
                break;
            case 1:
                player.transform.localPosition = new Vector3(-5.1f, 4.4f, 179f);
                Debug.Log("SettingChapter : " + highChapter);
                break;
            case 2:
                player.transform.localPosition = new Vector3(0, 0, 0);
                Debug.Log("SettingChapter : " + highChapter);
                break;
            case 3:
                player.transform.localPosition = new Vector3(0, 0, 0);
                Debug.Log("SettingChapter : " + highChapter);
                break;
            case 4:
                player.transform.localPosition = new Vector3(0, 0, 0);
                Debug.Log("SettingChapter : " + highChapter);
                break;
            case -1:
                player.transform.localPosition = new Vector3(-2f, 0.4f, 62.1f);
                Debug.Log("SettingChapter : 처음");
                break;

        }
    }


    // 가장 높은 챕터 값 구하기
    public int GetHighestClearedChapterIndex()
    {
        // isChapterCleared 배열에서 true 값을 찾아 가장 큰 인덱스 반환
        for (int i = data.isChapterCleared.Length - 1; i >= 0; i--)
        {
            if (data.isChapterCleared[i])  // true 값인 경우
            {
                return highChapter = i; // 가장 높은 인덱스 반환
            }
        }

        return highChapter = -1;
    }

}