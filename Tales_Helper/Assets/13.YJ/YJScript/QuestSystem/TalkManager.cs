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
        talkData.Add(1000, new string[] { "잭 엄마:1" });
        talkData.Add(2000, new string[] { "괜찮은 물건이 있으면 내가 또 바꿔주마.:1" });
        talkData.Add(3000, new string[] { "이 곳에 콩을 심으면 되겠다." });
        talkData.Add(5000, new string[] { "콩을 얻었다!" });

        //Quest Talk ((대화+퀘스트) 순서 + 누군지)
        talkData.Add(10 + 1000, new string[] 
            {"잭, 어디에 있다가 이제 오는거니.:0",
            "당장 내일 먹을 음식이 없으니 마을에 가서 이 소를 팔고 오렴.:0",
            "마을은 나가서 오른쪽 길로 쭉 따라가면 돼.:0",
            "늦지 않게 와야 해.:0"});
        
        talkData.Add(11 + 2000, new string[]
            {"이보게. 소년:0",
            "지금은 길이 물에 잠겨 마을에 갈 수 없단다.:0",
            "소를 팔러 가는 모양인데 나한테 팔지 않겠나?:0",
            "나에게 소를 주면 이 마법의 콩을 주마:0"});

        talkData.Add(30 + 1000, new string[]
            {"잭! 비싼 소를 이깟 콩이랑 바꿔오면 어떡하니!:0",
            "저기 다리 건너에 버리고 와:0"});



        //표정 이미지
        portraitData.Add(1000 + 0, portraitArr[0]);
        portraitData.Add(2000 + 0, portraitArr[1]);
    }

    public string GetTalk(int id,int talkIndex)
    {
        //ContainsKey() = Dictionary에 key가 존재하는지 검사
        if (!talkData.ContainsKey(id))
        {
            //해당 퀘스트 진행 순서 대사가 없을 때.
            //퀘스트 맨 처음 대사를 가지고 옮.
            //퀘스트 맨 처음 대사마저 없을 때.
            //기본 대사를 가지고 온다.
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
