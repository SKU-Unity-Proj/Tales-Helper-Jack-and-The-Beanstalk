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
        talkData.Add(2000, new string[] { "������ ������ ������ ���� �� �ٲ��ָ�.:1" });
        talkData.Add(3000, new string[] { "�� ���� ���� ������ �ǰڴ�." });
        talkData.Add(5000, new string[] { "���� �����!" });

        //Quest Talk ((��ȭ+����Ʈ) ���� + ������)
        talkData.Add(10 + 1000, new string[] 
            {"��, ��� �ִٰ� ���� ���°Ŵ�.:0",
            "���� ���� ���� ������ ������ ������ ���� �� �Ҹ� �Ȱ� ����.:0",
            "������ ������ ������ ��� �� ���󰡸� ��.:0",
            "���� �ʰ� �;� ��.:0"});
        
        talkData.Add(11 + 2000, new string[]
            {"�̺���. �ҳ�:0",
            "������ ���� ���� ��� ������ �� �� ���ܴ�.:0",
            "�Ҹ� �ȷ� ���� ����ε� ������ ���� �ʰڳ�?:0",
            "������ �Ҹ� �ָ� �� ������ ���� �ָ�:0"});

        talkData.Add(30 + 1000, new string[]
            {"��! ��� �Ҹ� �̱� ���̶� �ٲ���� ��ϴ�!:0",
            "���� �ٸ� �ǳʿ� ������ ��:0"});



        //ǥ�� �̹���
        portraitData.Add(1000 + 0, portraitArr[0]);
        portraitData.Add(2000 + 0, portraitArr[1]);
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
