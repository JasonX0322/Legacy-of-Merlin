using DG.Tweening;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandCard : Card
{
    EventTrigger et;
    Material mat;
    Image img;
    bool clicked;
    bool isPlayer;

    [SerializeField] HandList myHandList;
    [SerializeField] Army myArmy;
    [SerializeField] Army enemyArmy;

    //属性
    [SerializeField] string index = "-1";
    [SerializeField] Object objIntro;
    [SerializeField] Transform myCardGrave;
    GameObject goIntro;
    public struct CardParam
    {
        public string index;
        public string name;
        public int month;
        public string[] skillIndex;
        public string frontImg;
        public string backImg;
    }

    public struct SkillParam
    {
        public string index;
        public string name;
        public string effectIntro;
        public bool selectGroundCard;
        public bool selectArmySoldier;
        public bool addArmy;
        public bool finishAction;
    }

    CardParam myCardParam;
    //SkillParam mySkillParam;
    List<SkillParam> mySkillList=new List<SkillParam>();

    void Awake()
    {
        et = gameObject.AddComponent<EventTrigger>();// 添加Pointer Click事件
        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        et.triggers.Add(clickEntry);

        // 添加Pointer Enter事件
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
        et.triggers.Add(enterEntry);

        // 添加Pointer Exit事件
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
        et.triggers.Add(exitEntry);

        mat = new Material(Shader.Find("Card"));

        img = GetComponent<Image>();
        img.material = mat;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitCard(string idIndex)
    {
        index = idIndex;
        myCardParam = ReadCSV.i.GetCardParam(index);

        foreach (var param in myCardParam.skillIndex)
        {
            SkillParam newSkill;
            newSkill=ReadCSV.i.GetSkillParam(param);
            mySkillList.Add(newSkill);
        }

        Texture texFront = Resources.Load<Texture>("FrontImg/" + myCardParam.frontImg);
        Texture texBack = Resources.Load<Texture>("BackImg/" + myCardParam.backImg);
        mat.SetTexture("_Front", texFront);
        mat.SetTexture("_Back", texBack);

        if (isPlayer)
            GenerateIntro();
    }

    public void MoveTo(Vector3 position,Vector3 scale,bool rotateToFront)
    {
        transform.DOMove(position, 1);
        if(rotateToFront)
        {
            transform.DORotate(new Vector3(0, 0, 0), 1.0f);
        }
    }

    // Pointer Click事件响应函数
    void OnPointerClickDelegate(PointerEventData data)
    {
        if (!clickable)
            return;
        //Debug.Log("Pointer Click");
        if (!clicked)
        {
            if (isSoldier)
            {
                SoldierClickEvent();
                return;
            }
            StartCoroutine(ienuPlayerSelectEvent());
        }
        else
        {
            myHandList.SetSelectedCard(null);
            ResetCard();
            GroundCardList.i.ResetCards();
            clicked = false;
            myHandList.SetClicked(false);
        }
    }

    void SoldierClickEvent()
    {
        enemyArmy.AddSoldier(this.gameObject);
        myArmy.RemoveFromArmy(this.gameObject);
        enemyArmy.SelectOver();
    }

    IEnumerator ienuPlayerSelectEvent()
    {
        myHandList.SetSelectedCard(this.gameObject);
        myHandList.ResetCards(this.gameObject);
        clicked = true;
        GroundCardList.i.FindSameMonthGround(myCardParam.month);
        transform.DOScale(Vector3.one * 1.2f, 0.1f);
        mat.SetColor("_OutlineColor", Color.yellow);
        mat.SetFloat("_OutlineAlpha", 1);
        myHandList.SetClicked(true);
        mat.SetColor("_OutlineColor", Color.red);
        mat.SetFloat("_OutlineAlpha", 1);

        Debug.Log("判定选择精灵");
        bool selectGroundCard = mySkillList.Any(x => x.selectGroundCard == true);
        if (selectGroundCard && GroundCardList.i.FindSameMonthGround(myCardParam.month))
        {
            Debug.Log("开始选择精灵");
            BattleManager.i.StartPhase_SelectGroundCard();
            while (!BattleManager.i.IsPlayerAction())
                yield return null;
        }

        Debug.Log("判定选择士兵");
        bool selectArmySoldier = mySkillList.Any(x => x.selectArmySoldier == true);
        if (selectArmySoldier && enemyArmy.FindSameMonthTeam(myCardParam.month))
        {
            Debug.Log("开始选择士兵");
            BattleManager.i.StartPhase_SelectArmySoldier();
            while (!BattleManager.i.IsPlayerAction())
                yield return null;
        }


        Debug.Log("判定入伍");
        bool addArmy = mySkillList.Any(x => x.addArmy == true);
        if (addArmy)
        {
            Debug.Log("开始入伍");
            BattleManager.i.StartPhase_PlayerPause();
            myArmy.AddSoldier(this.gameObject);
            myHandList.RemoveList(this.gameObject);
            myArmy.SelectOver();
            while (!BattleManager.i.IsPlayerAction())
                yield return null;
        }
        else
        {
            Debug.Log("销毁卡牌");
            BattleManager.i.StartPhase_PlayerPause();
            mat.DOFloat(1, "_BurnRate", 2).OnComplete(()=>
            {
                myHandList.RemoveList(this.gameObject);
                //TODO 加入坟地
                transform.position = myCardGrave.position;

                BattleManager.i.ResumePhase_Action();
            });
            while (!BattleManager.i.IsPlayerAction())
                yield return null;
        }

        Debug.Log("结算");
        myHandList.SetClicked(false);
        if (BattleManager.i.IsPlayerAction())
            BattleManager.i.StartPhase_PlayerSettle();
        else
            BattleManager.i.StartPhase_EnemySettle();
    }

    public void AI_SelectEvent()
    {
        transform.DORotate(new Vector3(0, 0, 0), 1.0f).OnComplete(() =>
        {
            StartCoroutine(ienuAIAction());
        });
    }

    IEnumerator ienuAIAction()
    {
        Debug.Log("ai判定选择精灵");
        bool selectGroundCard = mySkillList.Any(x => x.selectGroundCard == true);
        if (selectGroundCard && GroundCardList.i.FindSameMonthGround(myCardParam.month))
        {
            BattleManager.i.StartPhase_AiPause();
            GroundCardList.i.AI_SelectMonth(myCardParam.month);
            while (!BattleManager.i.IsAiAction())
                yield return null;
        }
        Debug.Log("ai判定入伍");
        bool addArmy = mySkillList.Any(x => x.addArmy == true);
        if (addArmy)
        {
            Debug.Log("入伍");
            BattleManager.i.StartPhase_AiPause();
            myArmy.AddSoldier(this.gameObject);
            myHandList.RemoveList(this.gameObject);
            myArmy.SelectOver();
            while (!BattleManager.i.IsAiAction())
                yield return null;
        }
        else
        {
            Debug.Log("销毁卡牌");
            BattleManager.i.StartPhase_AiPause();
            mat.DOFloat(1, "_BurnRate", 2).OnComplete(() =>
            {
                myHandList.RemoveList(this.gameObject);
                //TODO 加入坟地
                transform.position = myCardGrave.position;

                BattleManager.i.ResumePhase_Action();
            });
            while (!BattleManager.i.IsPlayerAction())
                yield return null;
        }

        Debug.Log("ai判定结束");
        if (BattleManager.i.IsPlayerAction())
            BattleManager.i.StartPhase_PlayerSettle();
        else
            BattleManager.i.StartPhase_EnemySettle();
    }

    // Pointer Enter事件响应函数
    void OnPointerEnterDelegate(PointerEventData data)
    {
        if (!interactable)
        {
            //Debug.Log("不可交互");
            return;
        }
        if (!CheckIfReact())
        {
            //Debug.Log("不可交互2");
            return;
        }
        //Debug.Log("Pointer Enter");
        if (goIntro != null)
            goIntro.SetActive(true);
        GroundCardList.i.FindSameMonthGround(myCardParam.month);
        transform.DOScale(Vector3.one * 1.2f, 0.1f);
        mat.SetColor("_OutlineColor", Color.yellow);
        mat.SetFloat("_OutlineAlpha", 1);
    }

    void GenerateIntro()
    {
        goIntro = Instantiate(objIntro) as GameObject;
        goIntro.transform.SetParent(transform, false);
        goIntro.transform.localPosition = Vector3.zero;
        //goIntro.transform.LookAt(Camera.main.transform);
        goIntro.GetComponent<CardIntro>().initIntro(mySkillList);
        goIntro.SetActive(false);
    }

    // Pointer Exit事件响应函数
    void OnPointerExitDelegate(PointerEventData data)
    {
        if (!interactable)
            return;
        if (!CheckIfReact())
            return;
        //Debug.Log("Pointer Exit");
        if (goIntro != null)
            goIntro.SetActive(false);
        if (!clicked)
        {
            transform.DOScale(Vector3.one * 1, 0.1f);
            mat.SetColor("_OutlineColor", Color.yellow);
            mat.SetFloat("_OutlineAlpha", 0);
            GroundCardList.i.ResetCards();
        }
    }

    public void ResetCard()
    {
        clicked = false;
        transform.localScale = Vector3.one;
        mat.SetColor("_OutlineColor", Color.yellow);
        mat.SetFloat("_OutlineAlpha", 0);
    }

    bool CheckIfReact()
    {
        if (myHandList.GetClicked() && !clicked)
        {
            return false;
        }
        else
            return true;
    }

    #region 设置与获取属性
    public void SetisPlayer(bool bIsPlayer)
    {
        isPlayer = bIsPlayer;
    }

    public CardParam GetCardParam()
    {
        return myCardParam;
    }
    #endregion

    public override void SetInteractable(bool b)
    {
        base.SetInteractable(b);
        if (goIntro != null)
            goIntro.SetActive(false);
    }
}
