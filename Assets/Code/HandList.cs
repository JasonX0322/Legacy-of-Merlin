using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandList : MonoBehaviour
{
    [SerializeField] Transform[] cardsPos;
    bool[] cardsOver = new bool[8];

    GameObject[] handCardArray=new GameObject[8];
    bool clicked;
    GameObject goSelectedCard;
    [SerializeField] bool isPlayer;
    void Awake()
    {

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
                return cardsPos[i].position;
            }
        }
        Debug.LogWarning("没有空位发牌");
        return Vector3.zero;
    }

    public void AddCard(GameObject addCard)
    {
        //Debug.Log(addCard.name);
        //handCardArray.Add(addCard);
        for (int i = 0; i < handCardArray.Length; i++)
        {
            if (handCardArray[i]==null)
            {
                Vector3 targetPos = GetEmptyPos();
                addCard.GetComponent<HandCard>().MoveTo(targetPos, Vector3.one, isPlayer);
                handCardArray[i] = addCard;
                cardsOver[i] = true;
                break;
            }
        }
    }

    public void RemoveList(GameObject removeCard)
    {
        for (int i = 0; i < handCardArray.Length; i++)
        {
            if (handCardArray[i] == removeCard)
            {
                handCardArray[i] = null;
                cardsOver[i] = false;
                return;
            }
        }
    }

    public void SetSelectedCard(GameObject card)
    {
        goSelectedCard = card;
    }

    public void ResetCards(GameObject except)
    {
        foreach (var item in handCardArray)
        {
            if (item.gameObject != except)
                item.GetComponent<HandCard>().ResetCard();
        }
    }
    public void SetClicked(bool bClicked)
    {
        Debug.Log("牌库状态" + bClicked);
        clicked = bClicked;
    }

    public bool GetClicked()
    {
        return clicked;
    }

    public void ResetCards()
    {
        clicked = false;
        foreach (var item in handCardArray)
        {
            if (item != null)
                item.GetComponent<HandCard>().ResetCard();
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

    public GameObject[] GetHandList()
    {
        return handCardArray;
    }

    public void SetClickable(bool b)
    {
        foreach(var item in handCardArray)
        {
            if (item != null)
                item.GetComponent<Card>().SetClickable(b);
        }
    }

    public void SetInteractable(bool b)
    {
        foreach (var item in handCardArray)
        {
            if (item != null)
                item.GetComponent<Card>().SetInteractable(b);
        }
    }

}
