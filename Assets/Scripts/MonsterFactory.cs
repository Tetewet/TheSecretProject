using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    abstract class MonsterFactory : Actor
    {

        //public enum MonsterType
        //{
        //    goblin, orc, bandit, pirate
        //}
        //public enum MonsterClass
        //{
        //    warrior, mage, rogue
        //}

        public MonsterFactory(string Name, stat baseStats, bool Controllable) : base(Name, baseStats, Controllable)
        {
            this.Name = Name;
            this.baseStats = baseStats;
            this.Controllable = Controllable;
            ScaleOnPlayerLevel();
        }

        public void ScaleOnPlayerLevel()
        {
            //find a way to scale on player's level
        }

        // randomize the creation of monsters (later)
        abstract public MonsterFactory CreateKuku();
        abstract public MonsterFactory CreateKodama();
        abstract public MonsterFactory CreateBandit();
    }

    class MonsterControllerFactory : MonsterFactory
    {
        public MonsterControllerFactory(string Name, stat baseStats, bool Controllable) : base(Name, baseStats, Controllable)
        {
            base.Name = Name;
            base.baseStats = baseStats;
            base.Controllable = Controllable;
        }

        public override MonsterFactory CreateBandit()
        {
            return new Bandit(Name, baseStats, Controllable);
        }

        public override MonsterFactory CreateKodama()
        {
            return new Kodama(Name, baseStats, Controllable);
        }

        public override MonsterFactory CreateKuku()
        {
            return new Kuku(Name, baseStats, Controllable);
        }
    }
}
