using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandList : MonoBehaviour
{
    [SerializeField] Transform[] cardsPos;
    bool[] cardsOver;

    List<GameObject> handCardList=new List<GameObject>();
    bool clicked;
    GameObject goSelectedCard;
    void Awake()
    {
        cardsOver = new bool[cardsPos.Length];
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Vector3 GetEmptyPos()
    {
        for (int i = 0; i < cardsOver.Length; i++)
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
        handCardList.Add(addCard);
    }

    public void RemoveList(GameObject removeCard)
    {
        handCardList.Remove(removeCard);
    }

    public void SetSelectedCard(GameObject card)
    {
        goSelectedCard = card;
    }

    public void ResetCards(GameObject except)
    {
        foreach (var item in handCardList)
        {
            if (item.gameObject != except)
                item.GetComponent<HandCard>().ResetCard();
        }
    }
    public void SetClicked(bool bClicked)
    {
        clicked = bClicked;
    }

    public bool GetClicked()
    {
        return clicked;
    }
}
