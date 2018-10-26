using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InGameSkill : MonoBehaviour {


    public Text Cost,Name;

    Skill s;
    public Skill UseSKill
    {
        get { return s; }
    }

    public void ShowSkill(Skill c)
    {
        s = c;
        Name.text = c.Name;
        Cost.text = "sp:" + c.SpCost.ToString("00") + "mp:" + c.MpCost.ToString(); 
    }


}
