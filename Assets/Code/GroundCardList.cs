using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCardList : MonoBehaviour
{
    [SerializeField] Transform[] cardsPos;
    bool[] cardsOver;
    public static GroundCardList i;
    List<GameObject> groundCardList = new List<GameObject>();

    void Awake()
    {
        i = this;
        cardsOver = new bool[cardsPos.Length];
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public Vector3 GetEmptyPos()
    {
        for(int i=0; i<cardsOver.Length; i++)
        {
            if (!cardsOver[i])
            {
                cardsOver[i] = true;
                return cardsPos[i].position;
            }
        }
        Debug.LogWarning("没有空位发牌");
        return Vector3.zero;
    }
    public void AddList(GameObject addCard)
    {
        Debug.Log(addCard.name);
        groundCardList.Add(addCard);
    }

    public void RemoveList(GameObject removeCard)
    {
        groundCardList.Remove(removeCard);
    }

    public void ResetCards()
    {
        foreach (var item in groundCardList)
        {
            item.GetComponent<GroundCard>().ResetCard();
        }
    }

    public void FindSameMonthGround(int month)
    {
        foreach (var item in groundCardList)
        {
            item.GetComponent<GroundCard>().SetClickable(false);
            int groundMonth = item.GetComponent<GroundCard>().GetMonth();
            if (groundMonth == month)
            {
                item.GetComponent<GroundCard>().HighlightCard();
            }
        }
    }

    public void StartSelect()
    {
        foreach(var item in groundCardList)
        {
            item.GetComponent<GroundCard>().SetClickable(true);
        }
    }

}
