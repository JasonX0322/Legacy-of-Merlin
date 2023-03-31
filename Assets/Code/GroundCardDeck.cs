using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GroundCardDeck : MonoBehaviour
{
    List<GameObject> groundedCardList=new List<GameObject>();
    int cardPointer;
    public static GroundCardDeck i;
    [SerializeField] GroundCardList groundCardList;

    void Awake()
    {
        i = this;
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
        }
    }

    void Rearrange()
    {
        //打乱
        System.Random rand=new System.Random();
        groundedCardList.Sort((a, b) => rand.Next(-1, 2));
        foreach (var item in groundedCardList)
        {
            item.transform.SetAsLastSibling();
        }
        //位置
        Vector3 arrangePoint = Vector3.zero;
        for (int i = 0; i < groundedCardList.Count; i++)
        {
            groundedCardList[i].transform.localPosition = arrangePoint;
            arrangePoint += new Vector3(-1, 1, 0);
        }
    }

    public void DealCards()
    {
        StartCoroutine(ienuDealCards());
    }

    IEnumerator ienuDealCards()
    {
        int count = groundCardList.GetEmptyBlockCount();
        for (int i = 0; i < count; i++)
        {
            if (cardPointer < 0)
            {
                Debug.LogWarning("无牌可发");
                yield break;
            }
            //Vector3 targetPos=GroundCardList.i.GetEmptyPos();
            //groundedCardList[cardPointer].GetComponent<GroundCard>().MoveTo(targetPos, Vector3.one, true);
            GroundCardList.i.AddList(groundedCardList[cardPointer]);
            cardPointer--;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
