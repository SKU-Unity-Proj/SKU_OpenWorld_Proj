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

    [SerializeField]
    private LayerMask layerMask;
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera npcCam;

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 50f, layerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
                scanObject = hit.collider.gameObject;
                Action();
            }
        }

        if (isAction == true)
        {
            //npcCam.MoveToTopOfPrioritySubqueue();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Action();
            }
        }
        else
        {
            //mainCam.MoveToTopOfPrioritySubqueue();
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
}