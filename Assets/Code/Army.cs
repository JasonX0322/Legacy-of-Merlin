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
    List<GameObject> tempSoliders = new List<GameObject>();
    [SerializeField] Text txtAtk;
    int nAtk;
    [SerializeField] Text txtHealth;
    int nHealth = 30;

    [SerializeField] HandList myHandList;
    [SerializeField] GroundCardList groundCardList;

    bool isPlayer;
    [SerializeField] AnimPlayer myAnim_PhysicalAttack;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void AddSoldier(GameObject go)
    {
        tempSoliders.Add(go);
    }

    public void RemoveFromArmy(GameObject go)
    {
        for(int i=0;i!=teamList.Count;i++)
        {
            for(int j = 0; j != teamList[i].soliders.Count;j++)
            {
                if (teamList[i].soliders[j] == go)
                {
                    Debug.Log("移除士兵" + go.name);
                    teamList[i].soliders.RemoveAt(j);
                    Debug.Log(teamList[i].soliders.Count);
                    RearrangeCards(teamList[i].soliders, teamList[i].posIndex);
                    return;
                }
            }
        }
        Debug.LogWarning("没找到要删除的士兵");
    }

    public void SelectOver()
    {
        Debug.Log("完成选择");
        //TODO
        //BattleManager.i.StartPhase_Anim();
        myHandList.ResetCards();
        groundCardList.ResetCards();

        AddSoldiers();
        tempSoliders.Clear();
    }

    public void CancelSelect()
    {
        BattleManager.i.CancelPhase_Select();
        tempSoliders.Clear();
    }

    void AddSoldiers()
    {
        Debug.Log("AddSoliders");
        int cardMonth = 0;
        if (tempSoliders[0].GetComponent<HandCard>() != null)
            cardMonth = tempSoliders[0].GetComponent<HandCard>().GetCardParam().month;
        else if(tempSoliders[0].GetComponent<GroundCard>() != null)
            cardMonth = tempSoliders[0].GetComponent<GroundCard>().GetMonth();

        if (cardMonth == 0)
            Debug.LogWarning("错误的卡牌进入计算");

        foreach (var item in tempSoliders)
        {
            item.GetComponent<Card>().SetAsSoldier();
        }

        //如果已存在
        foreach (var team in teamList)
        {
            if (team.monthIndex == cardMonth)
            {
                //tempSoliders.Reverse();

                for(int i=0;i!=tempSoliders.Count;i++)
                {
                    team.soliders.Insert(0, tempSoliders[i]);
                    tempSoliders[i].transform.SetParent(pos[team.posIndex]);
                    tempSoliders[i].transform.localScale = Vector3.one;
                }
                RearrangeCards(team.soliders,team.posIndex);
                Debug.Log("存在");
                return;
            }
        }
        //如果不存在
        Debug.Log("不存在");
        team newTeam = new team();
        newTeam.monthIndex = cardMonth;
        newTeam.posIndex=teamList.Count;
        newTeam.soliders = new List<GameObject>();
        //tempSoliders.Reverse();
        for (int i = 0; i != tempSoliders.Count; i++)
        {
            newTeam.soliders.Insert(0, tempSoliders[i]);
            tempSoliders[i].transform.SetParent(pos[newTeam.posIndex]);
            tempSoliders[i].transform.localScale = Vector3.one;
        }
        RearrangeCards(newTeam.soliders, newTeam.posIndex);

        teamList.Add(newTeam);
    }

    void RearrangeCards(List<GameObject> soldiers, int posIndex)
    {
        if (soldiers.Count < 1)
            return;
        Vector3 newPos = pos[posIndex].position;
        for (int i = 0; i < soldiers.Count - 1; i++)
        {
            soldiers[i].transform.SetAsLastSibling();
            newPos.x = newPos.x - 20;
            soldiers[i].transform.DOMove(newPos, 1);
        }

        soldiers[soldiers.Count - 1].transform.SetAsLastSibling();
        newPos.x = newPos.x - 20;
        soldiers[soldiers.Count - 1].transform.DOMove(newPos, 1).OnComplete(() =>
        {
            UpdateAtk();
            BattleManager.i.ResumePhase_Action();
        });
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

    public delegate void AfterAttack(int SubHealth);
    public void GetAttack(int nDamage)
    {
        AfterAttack afterAttack = new AfterAttack(SubHealth);
        myAnim_PhysicalAttack.PlayOnce(afterAttack,nDamage);
    }

    public void SubHealth(int SubHealth)
    {
        Debug.Log("攻击");
        int newHealth = nHealth;
        newHealth -= SubHealth;
        if (newHealth < 0)
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


    public int GetHealth()
    {
        return nHealth;
    }

    public int GetAtk()
    {
        return nAtk;
    }

    public void SetInteractable(bool b)
    {
        foreach (var item in teamList)
        {
            foreach (var item2 in item.soliders)
            {
                item2.GetComponent<Card>().SetInteractable(b);
            }
        }
    }
    public void SetClickable(bool b)
    {
        foreach (var item in teamList)
        {
            foreach (var item2 in item.soliders)
            {
                item2.GetComponent<Card>().SetClickable(b);
            }
        }
    }

    public bool FindSameMonthTeam(int month)
    {
        for(int i=0;i!=teamList.Count;i++)
        {
            if (teamList[i].monthIndex==month)
            {
                return true;
            }
        }
        return false;
    }
}
