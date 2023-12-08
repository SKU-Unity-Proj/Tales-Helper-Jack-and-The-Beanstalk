using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    Dictionary<int, Sprite> portraitData;

    public Sprite[] portraitArr;

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    void GenerateData()
    {
        //NPC1 = 1000, NPC2 = 2000
        talkData.Add(1000, new string[] { "�� ����:1" });
        talkData.Add(2000, new string[] { "������ ������ ������ ���� �ٲ��ָ�:3" });

        //Quest Talk ((��ȭ+����Ʈ) ���� + ������)
        talkData.Add(10 + 1000, new string[] 
            {"��, ��� �ִٰ� ���� ���°Ŵ�.:0",
            "���� ���� ���� ������ ������ ������ ���� �� �Ҹ� �Ȱ� ����.:1",
            "���� �ʰ� �;� ��.:0"});
        
        talkData.Add(11 + 2000, new string[]
            {"�̺���. �ҳ�:1",
            "�Ҹ� �ȷ� ���� ����ε� ������ ���� �ʰڳ�?:2",
            "������ �Ҹ� �ָ� �� ������ ���� �ָ�:0"});

        talkData.Add(20 + 2000, new string[]
            {"���� �޾Ҵ�.:0"});

        talkData.Add(30 + 1000, new string[]
            {"��! ��� �Ҹ� �̱� ���̶� �ٲ���� ��ϴ�!:3",
            "���� �ٸ� �ǳʿ� ������ ��:3"});



        //ǥ�� �̹���
        portraitData.Add(1000 + 0, portraitArr[4]);
        portraitData.Add(1000 + 1, portraitArr[5]);
        portraitData.Add(1000 + 2, portraitArr[6]);
        portraitData.Add(1000 + 3, portraitArr[7]);
        portraitData.Add(2000 + 0, portraitArr[0]);
        portraitData.Add(2000 + 1, portraitArr[1]);
        portraitData.Add(2000 + 2, portraitArr[2]);
        portraitData.Add(2000 + 3, portraitArr[3]);
    }

    public string GetTalk(int id,int talkIndex)
    {
        //ContainsKey() = Dictionary�� key�� �����ϴ��� �˻�
        if (!talkData.ContainsKey(id))
        {
            //�ش� ����Ʈ ���� ���� ��簡 ���� ��.
            //����Ʈ �� ó�� ��縦 ������ ��.
            //����Ʈ �� ó�� ��縶�� ���� ��.
            //�⺻ ��縦 ������ �´�.
            if (!talkData.ContainsKey(id - id % 10))
                return GetTalk(id - id % 100, talkIndex);
            else
                return GetTalk(id - id % 10, talkIndex);
        }

        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
