using System;
using System.Collections;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    [SerializeField] phase phaseNow; 

    [SerializeField] HandDeck playerHandDeck;
    [SerializeField] HandDeck enemyHandDeck;

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
