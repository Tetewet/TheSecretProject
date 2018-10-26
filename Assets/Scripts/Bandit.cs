using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class Bandit : MonsterControllerFactory
    {
        public Bandit(string Name, stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
        {
            base.Name = Name;
            base.baseStats = baseStats;
            base.Controllable = Controllable;
        }
        
    }
}
