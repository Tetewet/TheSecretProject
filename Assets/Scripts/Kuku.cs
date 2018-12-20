using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Kuku : MonsterControllerFactory
{
    public Kuku(string Name, Stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
    {
        base.Name = Name;
        base.baseStats = baseStats;
        base.Controllable = Controllable;
    }

}

sealed class SingleChefKuku : Kuku
{
    private static SingleChefKuku chefKukuInstance;
    private bool isLeader;

    private SingleChefKuku(string Name, Stat baseStats, bool Controllable, string AnimatorP, bool isLeader) : base(Name, baseStats, Controllable, AnimatorP)
    {
        
        this.isLeader = isLeader;
    }

    public static SingleChefKuku ChefKukuInstance
    {
        get
        {
            if (chefKukuInstance == null) 
            {
                 chefKukuInstance = new SingleChefKuku(LanguageDao.GetLanguage("chefkuku", ref GameManager.language), new Stat { AGI = 6, END = 5, LUC = 35, STR = 4 }, false, "~Kuku", true);
            }
            return chefKukuInstance;
        }
    }
}