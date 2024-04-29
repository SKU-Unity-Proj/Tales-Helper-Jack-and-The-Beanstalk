using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex = 0; //퀘스트 대화 순서
    public GameObject[] questObject;
    public GameObject cow;

    Dictionary<int, QuestData> questList;

    public CinemachineVirtualCamera bridgeCam;
    public CinemachineVirtualCamera mainCam;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    //꺼져있어야 할 오브젝트
    private void Start()
    {
        questObject[5].SetActive(false);
        questObject[6].SetActive(false);
        questObject[7].SetActive(false);
    }
    //퀘스트에 따른 오브젝트 생성
    void ControlObject()
    {
        switch (questId)
        {
            //questObject[] = 0(엄마 느낌표) 1(상인) 2(상인 느낌표) 3(무너진 돌) 4(울타리) 5(콩주머니) 6(인벤에 넣을 콩주머니) 7(보부상 옆 소)
            case 10:
                if(questActionIndex == 1) //엄마에게 심부름 받은 이후
                {
                    questObject[0].SetActive(false); //1000 느낌표 X
                    questObject[2].SetActive(true); //2000 느낌표 O
                    cow.gameObject.GetComponent<FollowCow>().enabled = true;
                }
                if (questActionIndex == 2) //보부상과 대화 이후
                {
                    questObject[1].gameObject.layer = 0; //상인 레이어 X
                    questObject[2].SetActive(false); //2000 느낌표 X
                    cow.SetActive(false); //소 X
                    questObject[7].SetActive(true); // 보부상 소 O 
                    questObject[5].SetActive(true); //콩주머니 O
                }
                break;

            case 20:
                if (questActionIndex == 1) //콩주머니 가져간 후
                {
                    audioSource.Play();
                    questObject[6].SetActive(true); //인벤에 넣을 콩주머니 O
                    questObject[5].SetActive(false); //콩주머니 X
                    questObject[1].gameObject.layer = 8; //상인 레이어 O
                    questObject[0].SetActive(true); //1000 느낌표 O
                }
                break;

            case 30:
                if (questActionIndex == 1) //엄마와 대화 이후
                {
                    StartCoroutine("ShowBridge");
                    StartCoroutine("ShakeCamera");
                    questObject[1].SetActive(false); //상인 X
                    questObject[0].SetActive(false); //1000 느낌표 X
                    questObject[3].SetActive(true); //Crouch rock O
                    questObject[4].SetActive(false); //울타리 X
                }
                break;
        }
    }

    // Update is called once per frame
    void GenerateData()
    {
        questList.Add(10, new QuestData("마을에 소 팔기"
                                        , new int[] { 1000, 2000 }));

        questList.Add(20, new QuestData("콩 받기"
                                        , new int[] { 5000 }));

        questList.Add(30, new QuestData("엄마랑 대화하기"
                                        , new int[] { 1000 }));

        questList.Add(40, new QuestData("콩 심기"
                                        , new int[] { 0 }));
    }

    public int GetQuestTalkIndex(int id)
    {
        return questId + questActionIndex;
    }

    public string CheckQuest(int id)
    {
        //순서에 맞게 대화 했을 때만 대화 순서 올리기
        if (id == questList[questId].npcId[questActionIndex])
            questActionIndex++;

        ControlObject();

        //다음 퀘스트 확인
        if (questActionIndex == questList[questId].npcId.Length)
            NextQuest();
        //현재 퀘스트 출력
        return questList[questId].questName;
    }

    void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;
    }

    IEnumerator ShowBridge()
    {
        bridgeCam.MoveToTopOfPrioritySubqueue();
        bridgeCam.Priority = 11;

        yield return new WaitForSeconds(3f);

        mainCam.MoveToTopOfPrioritySubqueue();
        bridgeCam.Priority = 2;

        yield break;
    }

    IEnumerator ShakeCamera()
    {
        yield return new WaitForSeconds(6f);
        CameraShakeManager.Instance.SetShakeTime(1f);
    }
}
