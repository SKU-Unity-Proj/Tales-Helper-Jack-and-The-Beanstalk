using System.Collections;
using System.IO;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    static GameObject container;

    public GameObject player;
    public GameObject[] gameObjects;
    int highChapter = 0;

    #region �̱���
    // ---�̱������� ����--- //
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

    // --- ���� ������ �����̸� ���� ("���ϴ� �̸�(����).json") --- //
    string GameDataFileName = "GameData.json";

    // --- ����� Ŭ���� ���� --- //
    public Data data = new Data();

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

        GetHighestClearedChapterIndex();
        StartCoroutine(SettingPosition());
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
        data.isChapterCleared[chapterNum] = true;

        SaveGameData();
    }

    //��ġ �� ��Ȳ �����ϱ�
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
                Debug.Log("SettingChapter : ó��");
                break;

        }
    }


    // ���� ���� é�� �� ���ϱ�
    public int GetHighestClearedChapterIndex()
    {
        // isChapterCleared �迭���� true ���� ã�� ���� ū �ε��� ��ȯ
        for (int i = data.isChapterCleared.Length - 1; i >= 0; i--)
        {
            if (data.isChapterCleared[i])  // true ���� ���
            {
                return highChapter = i; // ���� ���� �ε��� ��ȯ
            }
        }

        return highChapter = -1;
    }

}