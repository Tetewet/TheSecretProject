using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Bandit : MonsterControllerFactory
{
    public Bandit(string Name, Stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
    {
        base.Name = Name;
        base.baseStats = baseStats;
        base.Controllable = Controllable;
    }

}

sealed class SingleChefBandit : Bandit
{
    private static SingleChefBandit chefBanditInstance;
    private bool isLeader;

    private SingleChefBandit(string Name, Stat baseStats, bool Controllable, string AnimatorP, bool isLeader) : base(Name, baseStats, Controllable, AnimatorP)
    {

        this.isLeader = isLeader;
    }

    public static SingleChefBandit ChefBanditInstance
    {
        get
        {
            if (chefBanditInstance == null)
            {
                chefBanditInstance = new SingleChefBandit(LanguageDao.GetLanguage("chefbandit", ref GameManager.language), new Stat { AGI = 6, END = 5, LUC = 35, STR = 4 }, false, "~Kuku", true);
            }
            return chefBanditInstance;
        }
    }
}