using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Kodama : MonsterControllerFactory
{
    public Kodama(string Name, Stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
    {
        base.Name = Name;
        base.baseStats = baseStats;
        base.Controllable = Controllable;
    }
}

sealed class SingleChefKodama : Kodama
{
    private static SingleChefKodama chefKodamaInstance;
    private bool isLeader;

    private SingleChefKodama(string Name, Stat baseStats, bool Controllable, string AnimatorP, bool isLeader) : base(Name, baseStats, Controllable, AnimatorP)
    {

        this.isLeader = isLeader;
    }

    public static SingleChefKodama ChefKodamaInstance
    {
        get
        {
            if (chefKodamaInstance == null)
            {
                chefKodamaInstance = new SingleChefKodama(LanguageDao.GetLanguage("chefkuku", GameManager.language), new Stat { AGI = 6, END = 5, LUC = 35, STR = 4 }, false, "~Kuku", true);
            }
            return chefKodamaInstance;
        }
    }
}