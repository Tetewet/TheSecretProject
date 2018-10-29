using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Skill 
{
    public enum TargetType
    {
        Self = 0,
        AnAlly = 1,
        OneEnemy = 2,
        Enemy = 3,
        Anyone = 4
    }

    public bool Unlocked = false;
    public string AnimationPath = "";

    private new string name = "";
    public string Name { get { return name; } set { name = value; } }
    private ProfessionType profession;
    public ProfessionType Profession { get { return profession; } set { profession = value; } }
    private DamageType type;
    public DamageType  Type { get { return type; } set { type = value; } }
    private int reach = 1;
    public int Reach { get { return reach; } set {reach  = value; } }
   
    //What percentage of the stats it uses; 1 = 100%, .2 = 20% of STR or INT - A la pokemon
    private float damage = 1;
    public float Damage{ get { return damage; } set { damage = value; } }


    private float baseCritChance = 5;
    public float BaseCritChance{ get { return baseCritChance; } set { baseCritChance = value; } }

    private TargetType targets;
    public TargetType Targets { get { return targets; } set { targets = value; } }


    //Requirement    
    private float mpCost = 0, hpCost = 0;
    public float MpCost { get { return mpCost; } set { mpCost= value; } }
    public float HpCost { get { return hpCost; } set { hpCost = value; } }
    private int spCost = 0, level = 0;
    public int SpCost { get { return spCost; } set { spCost = value; } }
    public int Level { get { return level; } set { level = value; } }


 
    public static Skill Base
    {

        get
        {
            var e = new Skill();
            e.name = "Attack";
            e.SpCost = 2;
            e.Reach = 1;
            e.Type = DamageType.Physical;
            e.Damage = .5f;
            e.Targets = TargetType.OneEnemy;
            return e;
        }
    }
    public virtual void Activate(Actor target, Stat stats = new Stat(), Actor f = null)
    {

        var x = Damage;
        if (Type == DamageType.Magical) x *= stats.INT;
        else if (Type == DamageType.Physical) x *= stats.STR;

        if ((stats.LUC * 2 + BaseCritChance) > UnityEngine.Random.Range(1, 101)) Damage *= 1.50f;

        target.TakeDamage(x, this, f);
    }
    public void Activate(Actor[] a, Actor f = null)
    {
        foreach (var item in a) { Activate(item); }
    }





}



