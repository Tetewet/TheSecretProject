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
        Name.text = a.Name.ToUpper() + "\n" + LanguageDao.GetLanguage("the", GameManager.language).ToUpper() + " " + a.Class.type.ToString().ToUpper(); ;
        Stats.text = "STATS"
            + "\n" + LanguageDao.GetLanguage("statstr", GameManager.language) + " " + a.GetStats.STR
              + "\n" + LanguageDao.GetLanguage("statend", GameManager.language) + " " + a.GetStats.END
                + "\n" + LanguageDao.GetLanguage("statagi", GameManager.language) + " " + a.GetStats.AGI
                  + "\n" + LanguageDao.GetLanguage("statint", GameManager.language) + " " + a.GetStats.INT
                    + "\n" + LanguageDao.GetLanguage("statwis", GameManager.language) + " " + a.GetStats.WIS
                      + "\n" + LanguageDao.GetLanguage("statluc", GameManager.language) + " " + a.GetStats.LUC;
        Lvl.text = LanguageDao.GetLanguage("lvl", GameManager.language) + " " + a.GetLevel
            + "\n" + LanguageDao.GetLanguage("exp", GameManager.language) + ": " + a.GetEXP.ToString("0000") +
            "\n" + LanguageDao.GetLanguage("nextxp", GameManager.language) + ": " + a.RequiredEXP.ToString("0000");
        Desc.text = a.Description;
        stats2.text = LanguageDao.GetLanguage("def", GameManager.language) + ": " + a.GetStats.PhysDEF  + " " 
            + LanguageDao.GetLanguage("mdef", GameManager.language) + ": " + a.GetStats.MagDEF 
            + " " + LanguageDao.GetLanguage("walk", GameManager.language) + ":" + (a.GetStats.MaximumSP * a.GetStats.AGI)  
            + "\n" + LanguageDao.GetLanguage("statcrit", GameManager.language) + ":" + a.GetStats.CriticalHitPercentage.ToString("00.0")
        + "% " + "" + LanguageDao.GetLanguage("speed", GameManager.language) + ": " + (a.GetStats.Priority + a.GetStats.AGI)  + " " 
        + LanguageDao.GetLanguage("sp", GameManager.language) + ": " + a.GetStats.MaximumSP;

    }
}
