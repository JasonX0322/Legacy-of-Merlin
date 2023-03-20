using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandCard : MonoBehaviour
{
    EventTrigger et;
    Material mat;
    Image img;
    bool clicked;
    bool isPlayer;

    [SerializeField] HandList myHandList;
    [SerializeField] Army myArmy;

    //属性
    string index = "-1";
    public struct CardParam
    {
        public string name;
        public int month;
        public string skillIndex;
        public string frontImg;
        public string backImg;
    }

    public struct SkillParam
    {
        public string name;
        public bool needToSelect;
        public bool addArmy;
        public bool finishAction;
    }

    CardParam myCardParam;
    SkillParam mySkillParam;

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
        Texture texFront = Resources.Load<Texture>(myCardParam.frontImg);
        Texture texBack = Resources.Load<Texture>(myCardParam.backImg);
        mat.SetTexture("_Front", texFront);
        mat.SetTexture("_Back", texBack);
    }

    public void InitCard(string idIndex)
    {
        index = idIndex;
        myCardParam = ReadCSV.i.GetCardParam(index);
        mySkillParam = ReadCSV.i.GetSkillParam(myCardParam.skillIndex);

    }

    public void MoveTo(Vector3 position,Vector3 scale,bool rotate)
    {
        transform.DOMove(position, 1);
        if(rotate)
        {
            transform.DORotate(new Vector3(0, 180, 0), 1.0f);
        }
    }

    // Pointer Click事件响应函数
    void OnPointerClickDelegate(PointerEventData data)
    {
        if (isPlayer)
        {
            Debug.Log("Pointer Click");
            if(!clicked)
            {
                myHandList.SetSelectedCard(this.gameObject);
                myHandList.ResetCards(this.gameObject);
                clicked = true;
                GroundCardList.i.FindSameMonthGround(myCardParam.month);
                transform.DOScale(Vector3.one * 1.2f, 0.1f);
                mat.SetColor("_OutlineColor", Color.yellow);
                mat.SetFloat("_OutlineAlpha", 1);
                myHandList.SetClicked(true);

                if(mySkillParam.needToSelect)
                {
                    mat.SetColor("_OutlineColor", Color.red);
                    myArmy.StartSelect();
                    myArmy.SelectSolider(this.gameObject);
                }

            }
            else
            {
                myHandList.SetSelectedCard(null);
                ResetCard();
                clicked = false;
                myHandList.SetClicked(false);
            }
        }
    }

    // Pointer Enter事件响应函数
    void OnPointerEnterDelegate(PointerEventData data)
    {
        if (!CheckIfReact())
            return;
        if (isPlayer)
        {
            Debug.Log("Pointer Enter");
            GroundCardList.i.FindSameMonthGround(myCardParam.month);
            transform.DOScale(Vector3.one * 1.2f, 0.1f);
            mat.SetColor("_OutlineColor", Color.yellow);
            mat.SetFloat("_OutlineAlpha", 1);
        }
    }

    // Pointer Exit事件响应函数
    void OnPointerExitDelegate(PointerEventData data)
    {
        if (!CheckIfReact())
            return;
        if (isPlayer)
        {
            Debug.Log("Pointer Exit");
            if (!clicked)
            {
                transform.DOScale(Vector3.one * 1, 0.1f);
                mat.SetColor("_OutlineColor", Color.yellow);
                mat.SetFloat("_OutlineAlpha", 0);
                GroundCardList.i.ResetCards();
            }
        }
    }

    public void SetisPlayer(bool bIsPlayer)
    {
        isPlayer = bIsPlayer;
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

    public int GetMonth()
    {
        return myCardParam.month;
    }

}
