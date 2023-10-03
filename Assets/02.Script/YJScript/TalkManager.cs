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
        //NPC1 = 1000, Cube = 100
        //Chair = 200
        talkData.Add(1000, new string[] { "안녕?:0", "이 곳에 처음 왔구나?:1" });

        talkData.Add(100, new string[] { "평범한 의자" });
        talkData.Add(200, new string[] { "대화 실패" });

        //Quest Talk ((대화+퀘스트) 순서 + 누군지)
        talkData.Add(10 + 1000, new string[] 
            {"어서와.:0",
            "이 마을에는 전설이 있어:1",
            "오른쪽 애가 알려줄거야:2"});
        talkData.Add(11 + 2000, new string[]
            {"여어.:0",
            "전설을 듣고 싶으면 일을 하나 해줘:1",
            "내 집 근처에서 떨어트린 동전을 주워줘:2"});

        talkData.Add(20 + 1000, new string[]
            {"동전? 못 봤는데:1"});
        talkData.Add(20 + 2000, new string[]
            {"하이.:0",
            "이 마을에는 전설이 있어:1",
            "오른쪽 애가 알려줄거야:2"});
        talkData.Add(20 + 5000, new string[]
            {"근처에서 동전을 찾았다."});
        talkData.Add(21 + 2000, new string[]
            {"엇, 찾아줘서 고마워.:2"});


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
