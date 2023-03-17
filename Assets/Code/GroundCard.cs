using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GroundCard : MonoBehaviour
{
    [SerializeField] int monthIndex;
    [SerializeField] Texture cardFront;
    [SerializeField] Texture cardBack;
    EventTrigger et;
    Material mat;
    [SerializeField] Army enemyArmy;
    [SerializeField] Army playerArmy;

    bool clickable;
    void Awake()
    {
        et = gameObject.AddComponent<EventTrigger>();
        mat = new Material(Shader.Find("Card"));
        GetComponent<Image>().material = mat;
    }
    // Start is called before the first frame update
    void Start()
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

        mat.SetTexture("_Front",cardFront);
        mat.SetTexture("_Back",cardBack);
    }
    public void MoveTo(Vector3 position, Vector3 scale, bool rotate = false)
    {
        transform.DOMove(position, 1);
        if (rotate)
        {
            transform.DORotate(new Vector3(0, 180, 0), 1.0f);
        }
    }

    public void ResetCard()
    {
        mat.SetFloat("_OutlineAlpha", 0);
        clickable = false;
    }

    public int GetMonth()
    {
        return monthIndex;
    }

    public void HighlightCard()
    {
        mat.SetFloat("_OutlineAlpha", 1);
    }
    // Pointer Click事件响应函数
    void OnPointerClickDelegate(PointerEventData data)
    {
        if (clickable && BattleManager.i.GetPhaseNow() == BattleManager.phase.Selecting)
        {
            playerArmy.SelectSolider(this.gameObject);
            playerArmy.FinishSelect();
        }
    }

    // Pointer Enter事件响应函数
    void OnPointerEnterDelegate(PointerEventData data)
    {
        if (clickable)
            transform.DOScale(Vector3.one * 1.2f, 0.1f);
    }

    // Pointer Exit事件响应函数
    void OnPointerExitDelegate(PointerEventData data)
    {
        if (clickable)
            transform.DOScale(Vector3.one, 0.1f);
    }

    public void SetClickable(bool bClickable)
    {
        clickable = bClickable;
    }
}
