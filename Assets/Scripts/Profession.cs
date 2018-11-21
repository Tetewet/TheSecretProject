using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Profession
{
    public enum ProfessionType
    {
        Adventurer = 0,
        Clerc = 1,
        Mercenary = 2,
        Mage = 3,
        Priest = 4,
        Paladin = 5,
        Rogue = 6,
        Sorcerer = 7,
        Barbarian = 8,
        Alchemist = 9,
        Archpriest = 10,
        Templar = 11,
        Berserker = 12,
        Elementalist = 13,
        Apostle = 14,
        Dragoon = 15
    }

    public const float BASEPROFIENCYEXP = 10;

    public ProfessionType type = ProfessionType.Adventurer;

    private int Profiency = 0;
    public int GetProfiency
    {
        get { return Profiency; }
    }

    protected Stat BaseStats;
    protected float ClassEXP = 0;


    public static Profession Madoshi
    {

        get
        {
            return new Profession(new Stat { AGI = 1, WIS = 2, INT = 1 }, ProfessionType.Mage, new Skill[5]
            {
                new Skill { Name = "Firebolt",
                Damage = .5f,
               MpCost = 10,
               BaseCritChance = 5f,
               Reach = 5,
               SpCost = 2,
               Targets = Skill.TargetType.OneEnemy,
               DmgType = DamageType.Magic,
               Unlocked = true
                ,Description = "Sparks aims at one target. Don't play with it , might start a fire."
            },
                   new Skill { Name = "Icebolt",
                Damage = .3f,
               MpCost = 6,
               BaseCritChance = 0f,
               Reach = 9,
               SpCost = 1,
               Targets = Skill.TargetType.OneEnemy,
               DmgType = DamageType.Magic,
               Unlocked = true
                ,Description = "Icicle aims at one target. Stings a bit."
            },
                      new Skill { Name = "Lightingbolt",
                Damage = .2f,
               MpCost = 8,
               BaseCritChance = 30f,
               Reach = 15,
               SpCost = 2,
               Targets = Skill.TargetType.OneEnemy,
               DmgType = DamageType.Magic,
               Unlocked = true
                ,Description = "Electrical arc launch at one target. Kinda hurt, but have 30% crit chance."
            },
                         new Skill { Name = "Mana Shock",
                Damage = 1f,
               MpCost = 30,
               BaseCritChance = 5f,
               Reach = 1,
               SpCost = 2,
               Targets = Skill.TargetType.OneEnemy,
               DmgType = DamageType.Magic,
               Unlocked = true,
               Description = "Magical Impulsion toward one target. Useful for cooking."
            },
                            new Skill { Name = "Brainstorm",
                Damage = 0f,
               MpCost = -10,
               BaseCritChance = 15f,
               Reach = 1,
               SpCost = 3,
               Targets = Skill.TargetType.Self,
               DmgType = DamageType.None,
               Unlocked = true
               ,Description = "Ponder on the situation. Give 10 MP to self."
            }


            });
        }
    }




    public Skill[] UsableSkill
    {
        get
        {
            var a = new List<Skill>();
            foreach (var item in Skills)
                if (item.Unlocked) a.Add(item);

            return a.ToArray();
        }
    }
    public Skill[] Skills;
    public void AddExp(float exp)
    {
        ClassEXP += exp;
        if (ClassEXP >= RequiredEXP) ProfiencyUP();
    }
    public virtual float RequiredEXP
    {
        get { return BASEPROFIENCYEXP * (2 * (float)Math.Exp(Profiency)); }
    }
    public virtual void ProfiencyUP()
    {
        Profiency++;
    }
    public virtual Stat GetBase
    {
        get { return BaseStats; }
    }
    public virtual void ClassLogic(Battle.Turn turn)
    {
        
    }

    public Profession(Stat s, ProfessionType profession = ProfessionType.Adventurer, Skill[] sk = null)
    {
        this.BaseStats = s;
        this.type = profession;
        Skills = sk;

    }


}

