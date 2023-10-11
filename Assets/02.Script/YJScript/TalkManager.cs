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
        talkData.Add(1000, new string[] { "안녕?:0", "난 잭이야:1" });
        talkData.Add(2000, new string[] { "넌 누구야:3" });

        //Quest Talk ((대화+퀘스트) 순서 + 누군지)
        talkData.Add(10 + 1000, new string[] 
            {"안녕. 난 잭이야:0",
            "지금 어머니가 아프셔서 이 소를 마을에 팔고 와야 하는데 도와줄 수 있어?:1",
            "고마워:0"});
        talkData.Add(11 + 2000, new string[]
            {"이보게. 소년:1",
            "소를 팔러 가는 모양인데 나한테 팔지 않겠나?:2",
            "나에게 소를 주면 이 마법의 콩을 주마:0"});
        talkData.Add(20 + 1000, new string[]
            {"음... 고민되는데:0",
            "너가 결정해줘:2"});



        //표정 이미지
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
