using System.Collections;
using System.IO;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    #region �̱���
    // --- �̱������� ���� --- //
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

    // --- ���� ������ �����̸� ���� ("���ϴ� �̸�(����).json") --- //
    string GameDataFileName = "GameData.json";

    // --- ����� Ŭ���� ���� --- //
    public Data data = new Data();

    public int SelectChapter = 1234;

    private void Start()
    {
        LoadGameData(); // ���� ���� �� ����� ������ �ҷ�����
    }

    // �ҷ�����
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        // ����� ������ �ִٸ�
        if (File.Exists(filePath))
        {
            // ����� ���� �о���� Json�� Ŭ���� �������� ��ȯ�ؼ� �Ҵ�
            string FromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(FromJsonData);
            print("�ҷ����� �Ϸ�");
        }
    }


    // �����ϱ�
    public void SaveGameData()
    {
        // Ŭ������ Json �������� ��ȯ (true : ������ ���� �ۼ�)
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        // �̹� ����� ������ �ִٸ� �����, ���ٸ� ���� ���� ����
        File.WriteAllText(filePath, ToJsonData);

        // �ùٸ��� ����ƴ��� Ȯ�� (�����Ӱ� ����)
        print("���� �Ϸ�");

        for (int i = 0; i < data.isChapterCleared.Length; i++)
        {
            print($"{i}�� é�� ��� ���� ���� : " + data.isChapterCleared[i]);
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