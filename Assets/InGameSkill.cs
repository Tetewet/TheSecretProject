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
    public void UseSkill(InGameActor Target, InGameActor Whom)
    {

    
    

    }

    public void ShowSkill(Skill c)
    {
        s = c;
        Name.text = c.Name;
        Cost.text = "sp:" + c.SpCost.ToString("00") + "mp:" + c.MpCost.ToString(); 
    }


}
