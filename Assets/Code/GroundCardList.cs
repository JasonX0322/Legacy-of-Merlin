using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCardList : MonoBehaviour
{
    [SerializeField] Transform[] cardsPos;
    bool[] cardsOver = new bool[8];
    public static GroundCardList i;
    GameObject[] groundCardArray = new GameObject[8];

    void Awake()
    {
        i = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public Vector3 GetEmptyPos()
    {
        for (int i = 0; i < cardsOver.Length; i++)
        {
            if (!cardsOver[i])
            {
                //Debug.Log("发牌至" + i.ToString());
                return cardsPos[i].position;
            }
        }
        Debug.LogWarning("没有空位发牌");
        return Vector3.zero;
    }
    public void AddList(GameObject addCard)
    {
        //Debug.Log(addCard.name);
        //groundCardArray.Add(addCard);
        for (int i = 0; i < groundCardArray.Length; i++)
        {
            if (groundCardArray[i] == null)
            {
                Vector3 targetPos = GetEmptyPos();
                addCard.transform.SetParent(this.transform);
                addCard.transform.SetAsLastSibling();
                addCard.GetComponent<GroundCard>().MoveTo(targetPos, Vector3.one);
                groundCardArray[i] = addCard;
                cardsOver[i] = true;
                break;
            }
        }
    }

    public void RemoveFromList(GameObject removeCard)
    {
        for (int i = 0; i < groundCardArray.Length; i++)
        {
            if (groundCardArray[i] == removeCard)
            {
                groundCardArray[i] = null;
                cardsOver[i] = false;
                Debug.Log("从场牌删除" + removeCard.name);
                return;
            }
        }
    }

    public void ResetCards()
    {
        foreach (var item in groundCardArray)
        {
            if (item != null)
                item.GetComponent<GroundCard>().ResetCard();
        }
    }

    public bool FindSameMonthGround(int month)
    {
        bool haveSameMonth = false;
        SetClickableForList(false);
        foreach (var item in groundCardArray)
        {
            if (item == null)
                continue;
            int groundMonth = item.GetComponent<GroundCard>().GetMonth();
            if (groundMonth == month)
            {
                item.GetComponent<GroundCard>().HighlightCard();
                haveSameMonth = true;
            }
        }
        return haveSameMonth;
    }

    public void SetClickableForList(bool b)
    {
        foreach (var item in groundCardArray)
        {
            if (item != null)
                item.GetComponent<GroundCard>().SetClickable(b);
        }
    }

    public int GetEmptyBlockCount()
    {
        int count = 0;
        foreach (var item in cardsOver)
        {
            if (!item)
                count++;
        }
        return count;
    }

    public void AI_SelectMonth(int month)
    {
        for (int i = 0; i < groundCardArray.Length; i++)
        {
            if (groundCardArray[i].GetComponent<GroundCard>().GetMonth() == month)
            {
                groundCardArray[i].GetComponent<GroundCard>().AI_SelectEvent();
                return;
            }
        }
    }
    public void SetClickable(bool b)
    {
        foreach (var item in groundCardArray)
        {
            if (item != null)
                item.GetComponent<Card>().SetClickable(b);
        }
    }

    public void SetInteractable(bool b)
    {
        foreach (var item in groundCardArray)
        {
            if (item != null)
                item.GetComponent<Card>().SetInteractable(b);
        }
    }

}
