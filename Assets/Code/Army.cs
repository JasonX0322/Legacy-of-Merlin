using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Army : MonoBehaviour
{
    public struct team
    {
        public int monthIndex;
        public int posIndex;
        public List<GameObject> soliders;
    }

    List<team> teamList = new List<team>();
    public Transform[] pos;
    public List<GameObject> tempSoliders = new List<GameObject>();
    [SerializeField] Text txtAtk;
    int nAtk;
    [SerializeField] Text txtHealth;
    int nHealth;

    bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartSelect()
    {
        BattleManager.i.StartPhase_Select();
    }

    public void SelectSolider(GameObject go)
    {
        tempSoliders.Add(go);
    }

    public void FinishSelect()
    {
        //TODO
        //BattleManager.i.StartPhase_Anim();
        AddSoldiers();
        tempSoliders.Clear();
    }

    public void CancelAelect()
    {
        BattleManager.i.CancelPhase_Select();
        tempSoliders.Clear();
    }

    void AddSoldiers()
    {
        int cardMonth = 0;
        if (tempSoliders[0].GetComponent<HandCard>() != null)
            cardMonth = tempSoliders[0].GetComponent<HandCard>().GetMonth();
        else if(tempSoliders[0].GetComponent<GroundCard>() != null)
            cardMonth = tempSoliders[0].GetComponent<GroundCard>().GetMonth();

        if (cardMonth == 0)
            Debug.LogWarning("错误的卡牌进入计算");

        //如果已存在
        foreach (var team in teamList)
        {
            if (team.monthIndex == cardMonth)
            {
                tempSoliders.Reverse();

                for(int i=0;i!=tempSoliders.Count;i++)
                {
                    team.soliders.Insert(0, tempSoliders[i]);
                    tempSoliders[i].transform.SetParent(pos[team.posIndex]);
                }
                RearrangeCards(team.soliders,team.posIndex);
                return;
            }
        }
        //如果不存在
        team newTeam = new team();
        newTeam.monthIndex = cardMonth;
        newTeam.posIndex=teamList.Count;
        newTeam.soliders = new List<GameObject>();
        tempSoliders.Reverse();
        for (int i = 0; i != tempSoliders.Count; i++)
        {
            newTeam.soliders.Insert(0, tempSoliders[i]);
            tempSoliders[i].transform.SetParent(pos[newTeam.posIndex]);
        }
        RearrangeCards(newTeam.soliders, newTeam.posIndex);

        teamList.Add(newTeam);
    }

    void RearrangeCards(List<GameObject> soliders,int posIndex)
    {
        Vector3 newPos = pos[posIndex].position;
        for (int i = 0; i < soliders.Count; i++)
        {
            soliders[i].transform.SetAsFirstSibling();
            newPos.x--;
            soliders[i].transform.DOMove(pos[posIndex].position, 0.1f).OnComplete(() =>
            {
                UpdateAtk();
                BattleManager.i.StartPhase_Settle();
            });

        }
    }

    void UpdateAtk()
    {
        int newAtk = 0;
        foreach (var team in teamList)
        {
            newAtk+=team.soliders.Count;
        }
        nAtk = newAtk;
        txtAtk.text = nAtk.ToString();
    }

    public void Attack(Army otherArmy)
    {
        otherArmy.GetHurt(nAtk);
    }

    public void GetHurt(int nDamage)
    {
        int newHealth = nHealth;
        newHealth -= nDamage;
        if(newHealth < 0)
        {
            newHealth = 0;
            nHealth = newHealth;
            txtHealth.text = nHealth.ToString();
            BattleManager.i.GameOver(isPlayer);
        }
        else
        {
            nHealth = newHealth;
            txtHealth.text = nHealth.ToString();
        }
    }
}
