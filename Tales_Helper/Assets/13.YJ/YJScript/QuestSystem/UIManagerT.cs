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
    public int talkIndex;
    public GameObject scanObject;

    private bool npcActivated;
    private bool isAction;
    private float searchRadius = 3;
    private Vector3 scanPos;
    //private CSPlayerController _CSPlayerController;

    [SerializeField]
    private LayerMask layerMask;

    public TextMeshProUGUI showQuest;

    private void Awake()
    {
        //_CSPlayerController = this.gameObject.GetComponent<CSPlayerController>();
    }

    private void Update()
    {
        CheckNPC();
    }

    public void Action()
    {
        LookEachOther();
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
        CantMove();
        //End Talk
        if (talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            Debug.Log(questManager.CheckQuest(id));
            showQuest.text = questManager.CheckQuest(id);
            CanMove();
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
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, searchRadius, layerMask);

            foreach (Collider col in colliders)
            {
                scanObject = col.gameObject;

                if (scanObject.gameObject.tag == "NPC")
                    npcActivated = true;

                if (npcActivated)
                {
                    Action();
                    scanPos = col.gameObject.transform.position;
                }
            }  
        }
    }

    private void LookEachOther()
    {
        scanObject.transform.LookAt(this.transform);
        this.transform.LookAt(scanPos);
    }

    private void CantMove()
    {
        this.gameObject.GetComponent<DiasGames.Controller.CSPlayerController>().enabled = false;
        this.gameObject.GetComponent<DiasGames.AbilityScheduler>().enabled = false;
        this.gameObject.GetComponent<CharacterController>().enabled = false;
    }

    private void CanMove()
    {
        this.gameObject.GetComponent<DiasGames.Controller.CSPlayerController>().enabled = true;
        this.gameObject.GetComponent<DiasGames.AbilityScheduler>().enabled = true;
        this.gameObject.GetComponent<CharacterController>().enabled = true;
    }
}