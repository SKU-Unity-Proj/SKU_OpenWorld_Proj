using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex = 0; //퀘스트 대화 순서
    public GameObject[] questObject;
    public GameObject cow;

    Dictionary<int, QuestData> questList;

    public CinemachineVirtualCamera bridgeCam;
    public CinemachineVirtualCamera mainCam;

    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    //퀘스트에 따른 오브젝트 생성
    void ControlObject()
    {
        switch (questId)
        {
            //questObject[] = 0(엄마 느낌표) 1(상인) 2(무너진 돌) 3(CTrigger) 4(ZTrigger) 5(FTrigger) 6(상인 느낌표)
            case 10:
                if(questActionIndex == 1) //엄마와 대화 이후
                {
                    StartCoroutine("ShowBridge");
                    questObject[0].SetActive(false); //1000 느낌표 꺼짐
                    questObject[6].SetActive(true); //2000 느낌표 켜짐
                    cow.gameObject.GetComponent<FollowCow>().enabled = true;
                }
                if (questActionIndex == 2) //보부상과 대화 이후
                {
                    questObject[1].SetActive(false); //2000 꺼짐
                    cow.SetActive(false); //소 꺼짐
                    questObject[0].SetActive(true); //1000 느낌표 켜짐
                    questObject[6].SetActive(false); //2000 느낌표 꺼짐
                }
                break;

            case 20:
                if (questActionIndex == 0)
                {
                    questObject[2].SetActive(true); //Crouch rock 켜짐
                    questObject[0].SetActive(false); //1000 느낌표 꺼짐
                    questObject[3].SetActive(true); //CTrigger 켜짐
                    questObject[4].SetActive(true); //ZTrigger 켜짐
                }
                break;
        }
    }

    // Update is called once per frame
    void GenerateData()
    {
        questList.Add(10, new QuestData("마을에 소 팔기"
                                        , new int[] { 1000, 2000 }));

        questList.Add(20, new QuestData("잭 엄마와 대화하기"
                                        , new int[] { 2000, 3000 }));

        questList.Add(30, new QuestData("퀘스트 클리어"
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
}
