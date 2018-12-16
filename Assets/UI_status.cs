using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_status : MonoBehaviour {

    public GameObject main;
    public Text Name, Stats, Lvl, Desc,stats2;
    
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
        stats2.text = "DEF: " + a.GetStats.PhysDEF  + " MDEF: " + a.GetStats.MagDEF 
            + " WLK:" + (a.GetStats.MaximumSP * a.GetStats.AGI)  + "\nCRIT:" + a.GetStats.CriticalHitPercentage.ToString("00.0")
        + "% SPD: "+ (a.GetStats.Priority + a.GetStats.AGI)  + " SP: "+ a.GetStats.MaximumSP;

    }
}
