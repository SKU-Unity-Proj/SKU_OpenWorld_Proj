using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex; //����Ʈ ��ȭ ����
    public GameObject[] questObject;

    Dictionary<int, QuestData> questList;

    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData()
    {
        questList.Add(10, new QuestData("���� ����� ��ȭ�ϱ�"
                                        , new int[] { 1000, 2000 }));

        questList.Add(20, new QuestData("���� ã���ֱ�"
                                        , new int[] { 1000, 2000 }));

        questList.Add(20, new QuestData("����Ʈ Ŭ����"
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

    //����Ʈ�� ���� ������Ʈ ����
    void ControlObject()
    {
        switch (questId)
        {
            case 10:
                if (questActionIndex == 2)
                    questObject[0].SetActive(true);
                break;
            case 20:
                if (questActionIndex == 1)
                    questObject[0].SetActive(false);
                break;
        }
    }
}
