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

    
    [SerializeField] private string name = "";
    public string Name {
        get {
            return name;
        }
        set {
            name = value;
        }
    }

    [SerializeField] private DamageType type;

    public DamageType Type {
        get {
            return type;
        }
        set
        {
            type = value;
        }
    }

    public int Reach = 1;
    //What percentage of the stats it uses; 1 = 100%, .2 = 20% of STR or INT - A la pokemon
    public float Damage = 1;
    //
    protected float BaseCritChance = 5;
    public TargetType Targets;
    public bool Unlocked = false;

    //Requirement    
    public float MpCost = 0, HpCost = 0;
    public int SpCost = 0;
    public static Skill Base
    {

        get
        {
            var e = new Skill();
            e.Name = "Attack";
            e.SpCost = 2;
            e.Reach = 1;
            e.Type = DamageType.Physical;
            e.Damage = .5f;
            e.Targets = TargetType.OneEnemy;
            return e;
        }
    }
    public virtual void Activate(Actor Target, stat Stats = new stat(), Actor f = null)
    {

        var x = Damage;
        if (Type == DamageType.Magical) x *= Stats.INT;
        else if (Type == DamageType.Physical) x *= Stats.STR;

        if ((Stats.LUC * 2 + BaseCritChance) > UnityEngine.Random.Range(1, 101)) Damage *= 1.50f;

        Target.TakeDamage(x, this, f);
    }
    public void Activate(Actor[] a, Actor f = null)
    {
        foreach (var item in a) { Activate(item); }
    }






    public static Skill Weapon(Weapon w)
    {

        var e = new Skill();
        e.Name = "Attack";
        e.SpCost = 2;
        e.Reach = 1;
        e.Type = w.DamageType;
        e.Damage = .5f;
        e.Targets = TargetType.OneEnemy;
        return e;

    }


}
