using System.Collections;
using System.IO;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    #region 싱글톤
    // --- 싱글톤으로 선언 --- //
    private static ChapterManager instance;
    public static ChapterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ChapterManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
    #endregion

    // --- 게임 데이터 파일이름 설정 ("원하는 이름(영문).json") --- //
    string GameDataFileName = "GameData.json";

    // --- 저장용 클래스 변수 --- //
    public Data data = new Data();

    public int SelectChapter = 1234;

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
        if (chapterNum <= data.isChapterCleared.Length)
            data.isChapterCleared[chapterNum] = true;
        else
            return;

        SaveGameData();
    }

    public void LockChapter(int chapterNum)
    {
        if (chapterNum <= data.isChapterCleared.Length)
            data.isChapterCleared[chapterNum] = false;
        else
            return;

        SaveGameData();
    }

}