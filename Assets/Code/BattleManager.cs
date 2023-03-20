using System;
using System.Collections;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    [SerializeField] phase phaseNow; 

    [SerializeField] HandDeck playerHandDeck;
    [SerializeField] HandDeck enemyHandDeck;

    [SerializeField] HandCard[] playerHandCard;
    [SerializeField] HandCard[] enemyHandCard;

    //TODO
    [SerializeField] string[] tempPlayerCardIndex;
    [SerializeField] string[] tempEnemyCardIndex;

    public static BattleManager i;

    bool isPlayerAction;

    public enum phase
    {
        Dealing,
        Action,
        Settle,
        Selecting,
        Anim
    }

    void Awake()
    {
        i = this;
    }

    void Start()
    {
        //读表
        ReadCSV.i.StartReadCSV();
        //初始化手牌
        for(int i=0;i!=playerHandCard.Length;i++)
        {
            playerHandCard[i].InitCard(tempPlayerCardIndex[i]);
        }
        for (int i = 0; i != enemyHandCard.Length; i++)
        {
            enemyHandCard[i].InitCard(tempEnemyCardIndex[i]);
        }
        //开始发牌
        StartPhase_Dealing();
    }

    public void StartPhase_Dealing()
    {
        phaseNow = phase.Dealing;
        GroundCardDeck.i.DealCards(8);
        playerHandDeck.DealCards(8);
        enemyHandDeck.DealCards(8);
    }

    public void StartPhase_Action(bool isPlayer)
    {
        phaseNow = phase.Action;
        isPlayerAction = isPlayer;
    }

    public void StartPhase_Settle()
    {
        if (phaseNow == phase.Settle)
            return;

        phaseNow = phase.Settle;
        
    }

    public void StartPhase_Select()
    {
        phaseNow=phase.Selecting;
    }

    public void CancelPhase_Select()
    {
        phaseNow = phase.Action;
    }

    public void GameOver(bool isPlayerWin)
    {
        if (isPlayerWin)
            Debug.Log("PlayerWin");
        else
            Debug.Log("PlayerLose");
    }

    public phase GetPhaseNow()
    {
        return phaseNow;
    }

    public bool IsPlayerAction()
    {
        return isPlayerAction;
    }

}
