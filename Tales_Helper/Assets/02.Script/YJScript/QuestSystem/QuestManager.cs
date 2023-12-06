using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex = 0; //����Ʈ ��ȭ ����
    public GameObject[] questObject;
    public GameObject cow;

    Dictionary<int, QuestData> questList;

    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    //����Ʈ�� ���� ������Ʈ ����
    void ControlObject()
    {
        switch (questId)
        {
            case 10:
                if(questActionIndex == 1)
                {
                    questObject[0].SetActive(false); //1000 ����ǥ ����
                    questObject[1].SetActive(true); //2000 ����
                    cow.gameObject.GetComponent<FollowCow>().enabled = true;
                }
                if (questActionIndex == 2)
                {
                    questObject[1].SetActive(false); //2000 ����
                    //GameObject.Find("GameManager").GetComponent<UIManager>().Action();
                    cow.SetActive(false); //�� ����
                    questObject[0].SetActive(true); //1000 ����ǥ ����
                }
                break;
            case 20:
                if (questActionIndex == 0)
                {
                    questObject[2].SetActive(true); //Crouch rock ����
                    questObject[0].SetActive(false); //1000 ����ǥ ����
                }
                break;
        }
    }

    // Update is called once per frame
    void GenerateData()
    {
        questList.Add(10, new QuestData("������ �� �ȱ�"
                                        , new int[] { 1000, 2000 }));

        questList.Add(20, new QuestData("�� ������ ��ȭ�ϱ�"
                                        , new int[] { 2000, 3000 }));

        questList.Add(30, new QuestData("����Ʈ Ŭ����"
                                        , new int[] { 0 }));
    }

    public int GetQuestTalkIndex(int id)
    {
        return questId + questActionIndex;
    }

    public string CheckQuest(int id)
    {
        //������ �°� ��ȭ ���� ���� ��ȭ ���� �ø���
        if (id == questList[questId].npcId[questActionIndex])
            questActionIndex++;

        ControlObject();

        //���� ����Ʈ Ȯ��
        if (questActionIndex == questList[questId].npcId.Length)
            NextQuest();
        //���� ����Ʈ ���
        return questList[questId].questName;
    }

    void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;
    }

    
}
