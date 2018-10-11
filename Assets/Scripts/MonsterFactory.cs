using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class MonsterFactory : Actor
    {

        public string monsterName;
        protected stat monsterStats = new stat();
        public bool monsterControllable = false;

        //public enum MonsterType
        //{
        //    goblin, orc, bandit, pirate
        //}
        //public enum MonsterClass
        //{
        //    warrior, mage, rogue
        //}

        public MonsterFactory(string monsterName, stat monsterStats, bool monsterControllable) : base(monsterName, monsterStats, monsterControllable)
        {
            this.monsterName = monsterName;
            this.monsterStats = monsterStats;
            this.monsterControllable = monsterControllable;
        }

        public void ScaleOnPlayerLevel()
        {
            //find a way to scale on player's level
        }
    }
}
