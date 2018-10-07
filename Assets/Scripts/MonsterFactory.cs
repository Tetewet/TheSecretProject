using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class MonsterFactory
    {
        
        public string name;
        public int hp;
        public int mp;
        public int sp;
        public enum MonsterType
        {
            goblin, orc
        }
        public enum MonsterClass
        {
            warrior, mage, rogue
        }

        public MonsterFactory(string name, int hp, int mp, int sp)
        {
            this.name = name;
            this.hp = hp;
            this.mp = mp;
            this.sp = sp;
        }

        public void ScaleOnPlayerLevel()
        {
            //find a way to scale on player's level
        }
    }
}
