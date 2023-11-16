using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class UIManager : MonoBehaviour
{
    public TalkManager talkManager;
    public QuestManager questManager;
    public GameObject talkPanel;
    public Image portraitImg;
    public Text talkText;
    public GameObject scanObject;
    public bool isAction;
    public int talkIndex;
    private Ray ray;
    private RaycastHit hit;
    private bool npcActivated;

    [SerializeField]
    private LayerMask layerMask;
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera npcCam;

    [SerializeField]
    private Transform rayPos;

    private void Update()
    {
        ray = new Ray(rayPos.transform.position, rayPos.transform.forward);

        CheckNPC();

        if (isAction == true)
            if (Input.GetKeyDown(KeyCode.Space))
                Action();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (npcActivated)
            {
                Action();
                CheckNPC();
            }
        } 
    }

    public void Action()
    {
        //Get Current Object
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id, objData.isNpc);

        talkPanel.SetActive(isAction);
    }
    
    void Talk(int id, bool isNpc)
    {
        //Set Talk Data
        int questTalkIndex = questManager.GetQuestTalkIndex(id);
        string talkData = talkManager.GetTalk(id+questTalkIndex, talkIndex);

        //End Talk
        if(talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            Debug.Log(questManager.CheckQuest(id));
            return;
        }

        //Continue Talk
        if (isNpc)
        {
            talkText.text = talkData.Split(':')[0];

            portraitImg.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(':')[1]));
            portraitImg.color = new Color(1, 1, 1, 1);
        }
        else
        {
            talkText.text = talkData;

            portraitImg.color = new Color(1, 1, 1, 0);
        }

        isAction = true;
        talkIndex++;
    }

    private void CheckNPC()
    {
        if (Physics.Raycast(ray, out hit, 5f, layerMask))
        {
            Debug.DrawRay(rayPos.transform.position, rayPos.transform.forward * 5f, Color.red);
            scanObject = hit.collider.gameObject;
            npcActivated = true;
        }
        else
            npcActivated = false;
    }
}