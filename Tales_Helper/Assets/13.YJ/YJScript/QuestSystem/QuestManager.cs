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

    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    private float ShakeDuration = 1f;          //ī�޶� ��鸲 ȿ���� ���ӵǴ� �ð�
    private float ShakeAmplitude = 3.0f;         //ī�޶� �Ķ����
    private float ShakeFrequency = 3.0f;         //ī�޶� �Ķ����
    private float ShakeElapsedTime = 0f;


    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();

        if (mainCam != null)
            virtualCameraNoise = mainCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (mainCam != null && virtualCameraNoise != null)
        {
            if (ShakeElapsedTime > 0)
            {
                virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                ShakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;
            }
        }
    }

    //����Ʈ�� ���� ������Ʈ ����
    void ControlObject()
    {
        switch (questId)
        {
            //questObject[] = 0(���� ����ǥ) 1(����) 2(������ ��) 3(CTrigger) 4(ZTrigger) 5(FTrigger) 6(���� ����ǥ) 7(��Ÿ��)
            case 10:
                if(questActionIndex == 1) //�������� �ɺθ� ���� ����
                {
                    questObject[0].SetActive(false); //1000 ����ǥ ����
                    questObject[6].SetActive(true); //2000 ����ǥ ����
                    cow.gameObject.GetComponent<FollowCow>().enabled = true;
                }
                if (questActionIndex == 2) //���λ�� ��ȭ ����
                {
                    cow.SetActive(false); //�� ����
                }
                break;

            case 20:
                if (questActionIndex == 1) //�� ���� ����
                {
                    questObject[6].SetActive(false); //2000 ����ǥ ����
                    questObject[0].SetActive(true); //1000 ����ǥ ����
                    questObject[7].SetActive(true); //���ָӴ� ����
                }
                break;

            case 30:
                if (questActionIndex == 1) //�� ���� �� ������ ��ȭ ����
                {
                    StartCoroutine("ShowBridge");
                    StartCoroutine("ShakeCamera");
                    questObject[1].SetActive(false); //���� ����
                    questObject[0].SetActive(false); //1000 ����ǥ ����
                    questObject[2].SetActive(true); //Crouch rock ����
                    questObject[7].SetActive(false); //��Ÿ�� ����
                    questObject[3].SetActive(true); //CTrigger ����
                    questObject[4].SetActive(true); //ZTrigger ����
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

        questList.Add(20, new QuestData("�� �ޱ�"
                                        , new int[] { 2000 }));

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
        ShakeElapsedTime = ShakeDuration;
    }
}
