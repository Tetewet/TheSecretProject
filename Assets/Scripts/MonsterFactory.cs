using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    abstract class MonsterFactory : Monster
    {

        //public enum MonsterType
        //{
        //    goblin, orc, bandit, pirate
        //}
        //public enum MonsterClass
        //{
        //    warrior, mage, rogue
        //}

<<<<<<< HEAD
        public MonsterFactory(string Name, stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
=======
        public MonsterFactory(string Name, Stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
>>>>>>> CoreEngine
        {
            this.Name = Name;
            this.baseStats = baseStats;
            this.Controllable = Controllable;
            this.AnimatorPath = AnimatorP;
            //ScaleOnPlayerLevel();
        }

        //public void ScaleOnPlayerLevel()
        //{
        //    //find a way to scale on player's level
        //}

        // randomize the creation of monsters (later)
        abstract public MonsterFactory CreateKuku();
        abstract public MonsterFactory CreateKodama();
        abstract public MonsterFactory CreateBandit();
    }

    class MonsterControllerFactory : MonsterFactory
    {
        static Random random;

<<<<<<< HEAD
        public MonsterControllerFactory(string Name, stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
=======
        public MonsterControllerFactory(string Name, Stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
>>>>>>> CoreEngine
        {
            base.Name = Name;
            base.baseStats = baseStats;
            base.Controllable = Controllable;
            base.AnimatorPath = AnimatorP;
        }

        public override MonsterFactory CreateBandit()
        {
            return new Bandit(Name, baseStats, Controllable, AnimatorPath);
        }

        public override MonsterFactory CreateKodama()
        {
            return new Kodama(Name, baseStats, Controllable, AnimatorPath);
        }

        public override MonsterFactory CreateKuku()
        {
            return new Kuku(Name, baseStats, Controllable, AnimatorPath);
        }

        public static void SpawnMonsters()
        {
            // randomize here
            int chances = random.Next(0, 100); // quels monstres ? = aleatoire
            int number = random.Next(4, 6); // nombre de monstres a faire spawn
            var factories = new List<MonsterControllerFactory>();
            if (chances > 66)
            {
                for (int i = 0; i < number; i++) 
                {
<<<<<<< HEAD
                    factories.Add(new Kuku("Kuku " + i.ToString(), new stat(), false, "AnimatorP"));
=======
                    factories.Add(new Kuku("Kuku " + i.ToString(), new Stat(), false, "AnimatorP"));
>>>>>>> CoreEngine
                }
            }
            else if (chances < 33) 
            {
                for (int i = 0; i < number; i++)
                {
<<<<<<< HEAD
                    factories.Add(new Kodama("Kodama " + i.ToString(), new stat(), false, "AnimatorP"));
=======
                    factories.Add(new Kodama("Kodama " + i.ToString(), new Stat(), false, "AnimatorP"));
>>>>>>> CoreEngine
                }
            }
            else
            {
                for (int i = 0; i < number; i++)
                {
<<<<<<< HEAD
                    factories.Add(new Bandit("Bandit " + i.ToString(), new stat(), false, "AnimatorP"));
=======
                    factories.Add(new Bandit("Bandit " + i.ToString(), new Stat(), false, "AnimatorP"));
>>>>>>> CoreEngine
                }
            }
            //SpawnMonsters(random.range(0, Monsterlist.count);

            //MonsterControllerFactory factory = factories[random.Next(0, factories.Count)];
            //factories.Remove(factory);

        }
    }

}
