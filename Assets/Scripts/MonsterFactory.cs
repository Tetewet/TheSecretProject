using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public MonsterFactory(string Name, Stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
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

        // randomize the creation of monsters 
        abstract public MonsterFactory CreateKuku();
        abstract public MonsterFactory CreateKodama();
        abstract public MonsterFactory CreateBandit();
    }

class MonsterControllerFactory : MonsterFactory
{
    static Random random = new Random();

    public MonsterControllerFactory(string Name, Stat baseStats, bool Controllable, string AnimatorP) : base(Name, baseStats, Controllable, AnimatorP)
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

    public static Actor[] SpawnMonsters()
    {
        int difficulty = 2;
        UnityEngine.Debug.Log("DIFFICULTY OF ENCOUNTER " + difficulty);
        int difficulTemp = 1;
        if (difficulty <= difficulTemp && difficulty <= 6)
        {
            difficulty = difficulTemp;
        }
        else { difficulTemp++; }
        // randomize here
        int chances = random.Next(0, 100); // quels monstres ? = aleatoire
        int number = random.Next(2, difficulty); // nombre de monstres a faire spawn
        var monsters = new List<MonsterControllerFactory>(); //TODO refactor pour avoir un meilleur code
        bool once = true;
        UnityEngine.Debug.Log("Spawning " + number + " ennemies");
        if (chances < 100)
        {
            for (int i = 0; i < number; i++)
            {
                monsters.Add(new Kuku("Kuku " + i.ToString(), new Stat { AGI = 4, END = 3, LUC = 20, STR = 2 }, false, "~Kuku"));
                ++i;
                monsters.Add(new Kuku("Slime " + i.ToString(), new Stat { AGI = 1, END = 2, LUC = 5, STR = 4, WIS = 3, CriticalHitFlat = 15 }, false, "Slime"));
                if (once && ++i < number)
                {
                    monsters.Add(SingleChefKuku.ChefKukuInstance);
                    once = false;
                }
            }
            return monsters.ToArray();
        }
        else if (chances < 0)
        {
            for (int i = 0; i < number; i++)
            {
                monsters.Add(new Kodama("Kodama " + i.ToString(), new Stat { AGI = 4, END = 3, LUC = 20, STR = 2 }, false, "~Kuku"));
                ++i;
                monsters.Add(new Kodama("Slime " + i.ToString(), new Stat { AGI = 1, END = 2, LUC = 5, STR = 4, WIS = 3, CriticalHitFlat = 15 }, false, "Slime"));
                if (once && ++i < number)
                {
                    monsters.Add(SingleChefKodama.ChefKodamaInstance);
                    once = false;
                }
            }
            return monsters.ToArray();
        }
        else
        {
            for (int i = 0; i < number; i++)
            {
                monsters.Add(new Bandit("Bandit " + i.ToString(), new Stat { AGI = 4, END = 3, LUC = 20, STR = 2 }, false, "~Kuku"));
                ++i;
                monsters.Add(new Bandit("Slime " + i.ToString(), new Stat { AGI = 1, END = 2, LUC = 5, STR = 4, WIS = 3, CriticalHitFlat = 15 }, false, "Slime"));
                if (once && ++i < number)
                {
                    monsters.Add(SingleChefBandit.ChefBanditInstance);
                    once = false;
                }
            }
            return monsters.ToArray();
        }
    }
    //SpawnMonsters(random.range(0, Monsterlist.count);

    //MonsterControllerFactory factory = factories[random.Next(0, factories.Count)];
    //factories.Remove(factory);
}

