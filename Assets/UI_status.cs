using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_status : MonoBehaviour {

    public GameObject main;
    public Text Name, Stats, Lvl, Desc;
    
    public void GetInfo(Actor a)
    {
        main.SetActive(true);
        Name.text = a.Name.ToUpper() + "\nTHE " + a.Class.type.ToString().ToUpper(); ;
        Stats.text = "STATS"
            + "\nSTR " + a.GetStats.STR
                + "\nEND " + a.GetStats.END
                  + "\nAGI " + a.GetStats.AGI
                    + "\nINT " + a.GetStats.INT
                      + "\nWIS " + a.GetStats.WIS
                        + "\nLUC " + a.GetStats.LUC;
        Lvl.text = "LVL " + a.GetLevel
            + "\nEXP: " + a.GetEXP.ToString("0000") +
            "\nNEXT: " + a.RequiredEXP.ToString("0000");
        Desc.text = a.Description;

    }
}
