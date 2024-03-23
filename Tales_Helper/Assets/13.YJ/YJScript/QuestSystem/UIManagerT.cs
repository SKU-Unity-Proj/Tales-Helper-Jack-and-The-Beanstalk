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
    public Animator talkPanel;
    public Image portraitImg;
    public Text talkText;
    public int talkIndex;
    public TextMeshProUGUI showQuest;
    public GameObject scanObject;

    private bool npcActivated;
    public bool isAction;
    private float searchRadius = 3;
    private Vector3 scanPos;
    private int Npcid = 0;

    [SerializeField]
    private LayerMask layerMask;

    private void Update()
    {
        CheckNPC();
        LookEachOther(Npcid);
    }

    public void Action()
    {
        //Get Current Object
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id, objData.isNpc);

        talkPanel.SetBool("isShow", isAction);
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
            scanObject = null;
            isAction = false;
            talkIndex = 0;
            Debug.Log(questManager.CheckQuest(id));
            showQuest.text = questManager.CheckQuest(id);
            Invoke("CanMove", 0.2f);
            return;
        }

        //Continue Talk
        if (isNpc)
        {
            talkText.text = talkData.Split(':')[0];

            talkText.gameObject.GetComponent<TextTyping>().enabled = false;
            talkText.gameObject.GetComponent<TextTyping>().enabled = true;

            portraitImg.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(':')[1]));
            portraitImg.color = new Color(1, 1, 1, 1);
        }
        else
        {
            talkText.text = talkData;

            talkText.gameObject.GetComponent<TextTyping>().enabled = false;
            talkText.gameObject.GetComponent<TextTyping>().enabled = true;

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
                }
            }  
        }
    }

    private void LookEachOther(int Npcid)
    {
        if (scanObject != null)
        {
            //NPCid값 가져오기
            Npcid = scanObject.GetComponent<ObjData>().id;

            //플레이어 바라볼 방향 구하기
            Vector3 vec = scanObject.gameObject.transform.position - transform.position;
            float lookSpeed = 2f * Time.deltaTime;

            //플레이어 회전 제외
            if(Npcid != 5000)
            {
                Quaternion q = Quaternion.LookRotation(vec);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, lookSpeed);
            }

            //NPC 회전 제외
            if (isAction && Npcid != 2000 && Npcid != 5000)
            {
                Quaternion w = Quaternion.LookRotation(-vec);
                scanObject.gameObject.transform.rotation = Quaternion.Slerp(scanObject.gameObject.transform.rotation, w, lookSpeed);
            }
        }
    }

    private void CantMove()
    {
        this.gameObject.GetComponent<DiasGames.Controller.CSPlayerController>().enabled = false;
        this.gameObject.GetComponent<DiasGames.Controller.SideCSPlayerController>().enabled = false;
        this.gameObject.GetComponent<DiasGames.AbilityScheduler>().enabled = false;
        this.gameObject.GetComponent<CharacterController>().enabled = false;
    }

    private void CanMove()
    {
        this.gameObject.GetComponent<DiasGames.Controller.CSPlayerController>().enabled = true;
        this.gameObject.GetComponent<DiasGames.Controller.SideCSPlayerController>().enabled = false;
        this.gameObject.GetComponent<DiasGames.AbilityScheduler>().enabled = true;
        this.gameObject.GetComponent<CharacterController>().enabled = true;
    }

}