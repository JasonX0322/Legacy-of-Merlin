using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardIntro : MonoBehaviour
{
    [SerializeField] Text txt;
    // Start is called before the first frame update
    void Start()
    {
        //SkillIntro si = new SkillIntro();
        //si.skillName = "Test";
        //si.skillEffect = "testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest";
        //SkillIntro[] a = new SkillIntro[1];
        //a[0] = si;
        //initIntro(a);
    }

    public void initIntro(List<HandCard.SkillParam> skillIntro)
    {
        string str = "";
        for (int i = 0; i < skillIntro.Count; i++)
        {
            str += "<size=40>";
            str += skillIntro[i].name;
            str += "\n";
            str += "</size>";
            str += "<size=20>";
            str += skillIntro[i].effectIntro;
            str += "\n";
            str += "</size>";
        }
        //string s = "";
        //s += "<size=40>";
        //s += "改变字体颜色";
        //s += "</size>";
        txt.text = str;
    }
}
