using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class UIManagerT : MonoBehaviour
{
    public TalkManager talkManager;
    public QuestManager questManager;
    public GameObject talkPanel;
    public Image portraitImg;
    public Text talkText;
    public GameObject scanObject;
    public bool isAction;
    public int talkIndex;
    private bool npcActivated;
    public float radius = 3;

    [SerializeField]
    private LayerMask layerMask;

    public TextMeshProUGUI showQuest;

    private void Update()
    {
        CheckNPC();
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
        string talkData = talkManager.GetTalk(id + questTalkIndex, talkIndex);

        //End Talk
        if (talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            Debug.Log(questManager.CheckQuest(id));
            showQuest.text = questManager.CheckQuest(id);
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius, layerMask);

            foreach (Collider col in colliders)
            {
                scanObject = col.gameObject;

                if (scanObject.gameObject.tag == "NPC")
                    npcActivated = true;

                if (npcActivated)
                {
                    Action();
                }
            }  
        }
    }
}