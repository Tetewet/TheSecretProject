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

public class Skill : MonoBehaviour
{
    public enum TargetType
    {
        Self = 0,
        AnAlly = 1,
        OneEnemy = 2,
        Enemy = 3,
        Anyone = 4
    }


    [SerializeField] private new string name = "";
    public string Name { get { return name; } set { name = value; } }
    [SerializeField] private DamageType type;
    public DamageType Type { get { return type; } set { type = value; } }
    [SerializeField] private int reach = 1;
    public int Reach { get { return reach; } set {reach  = value; } }
    //What percentage of the stats it uses; 1 = 100%, .2 = 20% of STR or INT - A la pokemon
    [SerializeField] private float damage = 1;
    public float Damage{ get { return damage; } set { damage = value; } }
    [SerializeField] private float baseCritChance = 5;
    public float BaseCritChance{ get { return baseCritChance; } set { baseCritChance = value; } }
    [SerializeField] private TargetType targets;
    public TargetType Targets { get { return targets; } set { targets = value; } }


    //Requirement    
    [SerializeField] private float mpCost = 0, hpCost = 0;
    public float MpCost { get { return mpCost; } set { mpCost= value; } }
    public float HpCost { get { return hpCost; } set { hpCost = value; } }
    [SerializeField] private int spCost = 0;
    public int SpCost { get { return spCost; } set { spCost = value; } }
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


}
