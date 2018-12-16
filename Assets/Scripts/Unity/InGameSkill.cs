using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InGameSkill : MonoBehaviour {

 
    public Text Cost,Name;

    Skill s;
    public Skill getSkill
    {
        get { return s; }
    }
 

    public void ShowSkill(Skill c, Actor a)
    {
        s = c;
        Name.text = c.Name;
        Cost.text = "";
        if (s.SpCost > 0)
            Cost.text += "SP:" + c.SpCost.ToString("00") + "/" + a.SP ;
        if (s.MpCost > 0)
            Cost.text += " MP:" + c.MpCost.ToString() + "/" + a.MP;
        if (s.HpCost > 0)
            Cost.text += " HP:" + c.HpCost.ToString() + "/" + a.HP;

    }


}
