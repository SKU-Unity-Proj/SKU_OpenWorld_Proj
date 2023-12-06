using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex = 0; //����Ʈ ��ȭ ����
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

    //����Ʈ�� ���� ������Ʈ ����
    void ControlObject()
    {
        switch (questId)
        {
            //questObject[] = 0(���� ����ǥ) 1(����) 2(������ ��) 3(CTrigger) 4(ZTrigger) 5(FTrigger) 6(���� ����ǥ)
            case 10:
                if(questActionIndex == 1) //������ ��ȭ ����
                {
                    StartCoroutine("ShowBridge");
                    questObject[0].SetActive(false); //1000 ����ǥ ����
                    questObject[6].SetActive(true); //2000 ����ǥ ����
                    cow.gameObject.GetComponent<FollowCow>().enabled = true;
                }
                if (questActionIndex == 2) //���λ�� ��ȭ ����
                {
                    questObject[1].SetActive(false); //2000 ����
                    cow.SetActive(false); //�� ����
                    questObject[0].SetActive(true); //1000 ����ǥ ����
                    questObject[6].SetActive(false); //2000 ����ǥ ����
                }
                break;

            case 20:
                if (questActionIndex == 0)
                {
                    questObject[2].SetActive(true); //Crouch rock ����
                    questObject[0].SetActive(false); //1000 ����ǥ ����
                    questObject[3].SetActive(true); //CTrigger ����
                    questObject[4].SetActive(true); //ZTrigger ����
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
