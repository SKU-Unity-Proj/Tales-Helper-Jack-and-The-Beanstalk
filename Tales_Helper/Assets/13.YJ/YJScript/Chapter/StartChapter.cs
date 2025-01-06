using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChapter : MonoBehaviour
{
    public GameObject player;
    public GameObject[] gameObjects;
    public int highChapter = 0;

    private void OnEnable()
    {
        if (ChapterManager.Instance.SelectChapter == 1234) {
            ChapterManager.Instance.LoadGameData();
            GetHighestClearedChapterIndex();
            StartCoroutine(SettingPosition());
        }
        else
        {
            highChapter = ChapterManager.Instance.SelectChapter;
            StartCoroutine(SettingPosition());
        }
    }

    //위치 및 상황 세팅하기
    IEnumerator SettingPosition()
    {
        yield return new WaitForSeconds(0.1f);
        switch (highChapter)
        {
            case 0:
                player.transform.localPosition = new Vector3(-2f, 0.4f, 62.1f);
                break;
            case 1:
                player.transform.localPosition = new Vector3(-1.3f, 3.7f, 104.2f);
                gameObjects[0].SetActive(false);
                gameObjects[1].GetComponent<Lock>().SettingChapter();
                Debug.Log("SettingChapter : " + highChapter);
                break;
            case 2:
                player.transform.localPosition = new Vector3(-5.1f, 4.4f, 179f);
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
        var data = ChapterManager.Instance.data;
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

    //세이브 UI용 함수
    public void SaveBtn()
    {
        ChapterManager.Instance.SaveGameData();
    }
}
