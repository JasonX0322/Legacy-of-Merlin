using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDeck : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] HandList myHandList;
    List<GameObject> groundedCardList = new List<GameObject>();
    int cardPointer;

    void Awake()
    {
        GetChildCards();
        cardPointer = groundedCardList.Count - 1;
        Rearrange();

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void GetChildCards()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            groundedCardList.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).GetComponent<HandCard>().SetisPlayer(isPlayer);
        }
    }

    void Rearrange()
    {
        //打乱
        System.Random rand = new System.Random();
        groundedCardList.Sort((a, b) => rand.Next(-1, 2));
        foreach (var item in groundedCardList)
        {
            item.transform.SetAsFirstSibling();
        }
        //位置
        Vector3 arrangePoint = Vector3.zero;
        for (int i = 0; i < groundedCardList.Count; i++)
        {
            groundedCardList[i].transform.localPosition = arrangePoint;
            arrangePoint += new Vector3(-1, 1, 0);
        }
    }

    public void DealCards(int count)
    {
        if (cardPointer < 0)
        {
            Debug.LogWarning("无牌可发");
            return;
        }
        StartCoroutine(ienuDealCards(count));
    }

    IEnumerator ienuDealCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 targetPos = myHandList.GetEmptyPos();
            groundedCardList[cardPointer].GetComponent<HandCard>().MoveTo(targetPos, Vector3.one, isPlayer);
            if (isPlayer)
                myHandList.AddList(groundedCardList[cardPointer]);
            cardPointer--;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
