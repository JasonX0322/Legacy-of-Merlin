using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] HandList myHandList;
    [SerializeField] string enemyName;
    GameObject[] handCardArray;

    public static EnemyAI i;

    public struct AICard
    {
        public string cardIndex;
        public int cardPriority;
    }
    AICard[] aiCardArray;

    void Awake()
    {
        i = this;
    }

    public void InitEnemyAI()
    {
        aiCardArray = ReadCSV.i.GetEnemyAI(enemyName);
    }

    public string[] GetEnemyCardsIndex()
    {
        string[] cards = new string[aiCardArray.Length];
        for(int i=0;i!=aiCardArray.Length;i++)
        {
            cards[i]=aiCardArray[i].cardIndex;
        }
        return cards;
    }

    public int[] GetEnemyCardsPriority()
    {
        int[] cards = new int[aiCardArray.Length];
        for (int i = 0; i != aiCardArray.Length; i++)
        {
            cards[i] = aiCardArray[i].cardPriority;
        }
        return cards;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SelectMinPriority()
    {
        handCardArray = myHandList.GetHandList();
        int min = 9999;
        int minIndex = -1;
        for (int i = 0; i < handCardArray.Length; i++)
        {
            int n = FindPriority(handCardArray[i].GetComponent<HandCard>().GetCardParam().index);
            if (n < min)
            {
                int month = handCardArray[i].GetComponent<HandCard>().GetCardParam().month;
                if (!GroundCardList.i.FindSameMonthGround(month))
                    continue;
                min = n;
                minIndex = i;
            }
        }
        if (minIndex == -1)
            minIndex = 0;
        handCardArray[minIndex].GetComponent<HandCard>().AI_SelectEvent();

    }

    public int FindPriority(string index)
    {
        for (int i = 0; i != handCardArray.Length; i++)
        {
            if (index == aiCardArray[i].cardIndex)
            {
                return aiCardArray[i].cardPriority;
            }
        }
        Debug.LogError("AI没有找到卡");
        return -1;
    }

    public string GetName()
    {
        return enemyName;
    }
}
