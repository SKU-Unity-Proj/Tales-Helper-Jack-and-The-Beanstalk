using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex = 0; //����Ʈ ��ȭ ����
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

    //�����־�� �� ������Ʈ
    private void Start()
    {
        questObject[5].SetActive(false);
        questObject[6].SetActive(false);
        questObject[7].SetActive(false);
    }
    //����Ʈ�� ���� ������Ʈ ����
    void ControlObject()
    {
        switch (questId)
        {
            //questObject[] = 0(���� ����ǥ) 1(����) 2(���� ����ǥ) 3(������ ��) 4(��Ÿ��) 5(���ָӴ�) 6(�κ��� ���� ���ָӴ�) 7(���λ� �� ��)
            case 10:
                if(questActionIndex == 1) //�������� �ɺθ� ���� ����
                {
                    questObject[0].SetActive(false); //1000 ����ǥ X
                    questObject[2].SetActive(true); //2000 ����ǥ O
                    cow.gameObject.GetComponent<FollowCow>().enabled = true;
                }
                if (questActionIndex == 2) //���λ�� ��ȭ ����
                {
                    questObject[1].gameObject.layer = 0; //���� ���̾� X
                    questObject[2].SetActive(false); //2000 ����ǥ X
                    cow.SetActive(false); //�� X
                    questObject[7].SetActive(true); // ���λ� �� O 
                    questObject[5].SetActive(true); //���ָӴ� O
                }
                break;

            case 20:
                if (questActionIndex == 1) //���ָӴ� ������ ��
                {
                    audioSource.Play();
                    questObject[6].SetActive(true); //�κ��� ���� ���ָӴ� O
                    questObject[5].SetActive(false); //���ָӴ� X
                    questObject[1].gameObject.layer = 8; //���� ���̾� O
                    questObject[0].SetActive(true); //1000 ����ǥ O
                }
                break;

            case 30:
                if (questActionIndex == 1) //������ ��ȭ ����
                {
                    StartCoroutine("ShowBridge");
                    StartCoroutine("ShakeCamera");
                    questObject[1].SetActive(false); //���� X
                    questObject[0].SetActive(false); //1000 ����ǥ X
                    questObject[3].SetActive(true); //Crouch rock O
                    questObject[4].SetActive(false); //��Ÿ�� X
                }
                break;
        }
    }

    // Update is called once per frame
    void GenerateData()
    {
        questList.Add(10, new QuestData("������ �� �ȱ�"
                                        , new int[] { 1000, 2000 }));

        questList.Add(20, new QuestData("�� �ޱ�"
                                        , new int[] { 5000 }));

        questList.Add(30, new QuestData("������ ��ȭ�ϱ�"
                                        , new int[] { 1000 }));

        questList.Add(40, new QuestData("�� �ɱ�"
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
