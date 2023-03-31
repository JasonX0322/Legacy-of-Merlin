using System;
using System.Collections;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    [SerializeField] phase phaseNow;

    [SerializeField] HandDeck playerHandDeck;
    [SerializeField] HandDeck enemyHandDeck;

    [SerializeField] HandCard[] playerHandCardinDeck;
    [SerializeField] HandCard[] enemyHandCardinDeck;

    [SerializeField] HandList playerHandList;
    [SerializeField] HandList enemyHandList;

    [SerializeField] GroundCardList groundCardList;

    [SerializeField] Army playerArmy;
    [SerializeField] Army enemyArmy;

    //TODO
    string[] tempPlayerCardIndex;
    string[] tempEnemyCardIndex;

    public static BattleManager i;

    public enum phase
    {
        Dealing,
        PlayerAction,
        PlayerSettle,
        PlayerPause,
        AiAction,
        AiPause,
        AiSettle
    }

    void Awake()
    {
        i = this;
    }

    void Start()
    {
        //读表
        ReadCSV.i.ReadCardandSkill();
        //AI初始化
        EnemyAI.i.InitEnemyAI();
        //TODO
        tempPlayerCardIndex = new string[30];
        tempEnemyCardIndex = EnemyAI.i.GetEnemyCardsIndex();
        for (int i = 0; i < tempPlayerCardIndex.Length; i++)
        {
            int index = 1001 + i;
            string strIndex = index.ToString();
            tempPlayerCardIndex[i] = strIndex;
        }
        //初始化手牌
        for (int i = 0; i != playerHandCardinDeck.Length; i++)
        {
            playerHandCardinDeck[i].InitCard(tempPlayerCardIndex[i]);
        }
        for (int i = 0; i != enemyHandCardinDeck.Length; i++)
        {
            enemyHandCardinDeck[i].InitCard(tempEnemyCardIndex[i]);
        }
        //开始发牌
        StartPhase_Dealing();
        //StartPhase_PlayerAction();
    }

    public void StartPhase_Dealing()
    {
        phaseNow = phase.Dealing;
        GroundCardDeck.i.DealCards();
        playerHandDeck.DealCards();
        enemyHandDeck.DealCards();

        this.Invoke("StartPhase_PlayerAction", 5);
    }

    public void StartPhase_PlayerAction()
    {
        phaseNow = phase.PlayerAction;
        Debug.Log("Player行动");
        //行动
        playerHandList.SetClickable(true);
        playerHandList.SetInteractable(true);
    }

    public void StartPhase_PlayerSettle()
    {
        phaseNow = phase.PlayerSettle;
        Debug.Log("Player结算");
        //补牌
        GroundCardDeck.i.DealCards();
        playerHandDeck.DealCards();
        enemyHandDeck.DealCards();

        //锁定
        playerHandList.SetClickable(false);

        //攻击
        enemyArmy.GetAttack(playerArmy.GetAtk());

        //攻击后
        if (CheckGameOver())
        {
            //TODO
        }
        else
        {
            Invoke("StartPhase_EnemyAction", 2);
        }
    }

    public void StartPhase_EnemyAction()
    {
        phaseNow = phase.AiAction;
        Debug.Log("Enemy行动");
        EnemyAI.i.SelectMinPriority();
    }

    public void StartPhase_EnemySettle()
    {
        phaseNow = phase.AiSettle;
        Debug.Log("Enemy结算");
        //补牌
        GroundCardDeck.i.DealCards();
        playerHandDeck.DealCards();
        enemyHandDeck.DealCards();

        //攻击
        playerArmy.GetAttack(enemyArmy.GetAtk());
        if (CheckGameOver())
        {
            //TODO
        }
        else
        {
            StartPhase_PlayerAction();
        }
    }

    public bool CheckGameOver()
    {
        if(enemyArmy.GetHealth()<=0)
        {
            Debug.Log("胜利");
            return true;
        }
        if(playerArmy.GetHealth()<=0)
        {
            Debug.Log("失败");
            return true;
        }
        return false;
    }

    public void StartPhase_SelectGroundCard()
    {
        phaseNow=phase.PlayerPause;

        groundCardList.SetClickable(true);
        groundCardList.SetInteractable(true);
    }

    public void StartPhase_SelectArmySoldier()
    {
        phaseNow = phase.PlayerPause;

        enemyArmy.SetInteractable(true);
        enemyArmy.SetClickable(true);
    }

    public void StartPhase_AiPause()
    {
        phaseNow = phase.AiPause;
    }

    public void StartPhase_PlayerPause()
    {
        phaseNow = phase.PlayerPause;
    }

    public void CancelPhase_Select()
    {
        if (phaseNow == phase.AiPause)
            phaseNow = phase.AiAction;
        else
        {
            phaseNow = phase.PlayerAction;
            groundCardList.SetClickable(false);
            groundCardList.SetInteractable(false);
            enemyArmy.SetInteractable(false);
            enemyArmy.SetClickable(false);
        }
    }

    public void ResumePhase_Action()
    {
        if (phaseNow == phase.AiPause)
            phaseNow = phase.AiAction;
        else
        {
            phaseNow = phase.PlayerAction;
            groundCardList.SetClickable(false);
            groundCardList.SetInteractable(false);
            enemyArmy.SetInteractable(false);
            enemyArmy.SetClickable(false);
        }
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
        return (phaseNow == phase.PlayerAction);
    }

    public bool IsAiAction()
    {
        return (phaseNow == phase.AiAction);
    }
}
