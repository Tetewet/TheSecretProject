using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class Kuku : MonsterControllerFactory
    {
        public Kuku(string Name, Stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
        {
            base.Name = Name;
            base.baseStats = baseStats;
            base.Controllable = Controllable;
        }

    }
}
