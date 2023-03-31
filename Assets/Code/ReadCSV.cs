using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static HandCard;
using static EnemyAI;
using System.Reflection;

public class ReadCSV : MonoBehaviour
{
    public static ReadCSV i;
    List<string[]> card;
    List<string[]> skill;


    void Awake()
    {
        i = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void ReadCardandSkill()
    {
        card = Read("card");
        skill = Read("skill");
    }

    public CardParam GetCardParam(string index)
    {
        //Debug.Log(index);
        int cardIndex = -1;
        CardParam cardParam = new CardParam();
        for (int i = 0; i != card.Count; i++)
        {
            if (card[i][0] == index)
            {
                cardIndex = i;
                break;
            }
        }
        if (cardIndex == -1)
            Debug.LogWarning("没有找到牌");
        cardParam.index = card[cardIndex][0];
        cardParam.name = card[cardIndex][1];
        cardParam.month = int.Parse(card[cardIndex][2]);
        cardParam.skillIndex = card[cardIndex][3].Split(',');
        cardParam.frontImg = card[cardIndex][4];
        cardParam.backImg = card[cardIndex][5];
        return cardParam;
    }

    public SkillParam GetSkillParam(string index)
    {
        int skillIndex = -1;
        SkillParam skillParam = new SkillParam();
        for (int i = 0; i != skill.Count; i++)
        {
            if (skill[i][0] == index)
            {
                skillIndex = i;
                break;
            }
        }
        if (skillIndex == -1)
            Debug.LogWarning("没有找到技能");
        skillParam.index = skill[skillIndex][0];
        skillParam.name = skill[skillIndex][1];
        skillParam.effectIntro = skill[skillIndex][2];
        skillParam.selectGroundCard = TurnToTureFalse(skill[skillIndex][3]);
        skillParam.selectArmySoldier = TurnToTureFalse(skill[skillIndex][4]);
        skillParam.addArmy = TurnToTureFalse(skill[skillIndex][5]);
        skillParam.finishAction = TurnToTureFalse(skill[skillIndex][6]);
        return skillParam;

    }

    public AICard[] GetEnemyAI(string name)
    {
        List<string[]> aiCardList = Read(name);
        AICard[] aiCardArray = new AICard[aiCardList.Count];
        for(int i=0;i!=aiCardList.Count;i++)
        {
            AICard aiCard = new AICard();
            aiCard.cardIndex = aiCardList[i][0];
            aiCard.cardPriority = int.Parse(aiCardList[i][1]);
            aiCardArray[i] = aiCard;
        }
        return aiCardArray;
    }

    bool TurnToTureFalse(string str)
    {
        if (str == "0")
            return false;
        else
            return true;
    }


    public List<string[]> Read(string name)
    {
        string path = Application.streamingAssetsPath + "/" + name + ".csv";
        List<string[]> csvData = new List<string[]>();
        StreamReader sr = new StreamReader(path);
        string line;

        bool firstLine=true;

        while ((line = sr.ReadLine()) != null)
        {
            if(firstLine)
            {
                firstLine = false;
                continue;
            }
            string[] rowData = line.Split(',');
            csvData.Add(rowData);
            //Debug.Log(rowData[0]);
        }
        sr.Close();
        return csvData;
    }
}
