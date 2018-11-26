using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Skill 
{
    public Skill(string name,string description, Profession.ProfessionType profType, DamageType dmgType,int reach, float damage, float baseCritChance,TargetType target,int hpCost,int mpCost,int spCost, int level, bool unlocked = true) {
        this.name = name;
        this.Description = description;
        this.profType = profType;
        this.dmgType = dmgType;
        this.reach = reach;
        this.damage = damage;
        this.baseCritChance = baseCritChance;
        this.targets = target;
        this.Unlocked = unlocked;
        this.mpCost = mpCost;
        this.hpCost = hpCost;
        this.spCost = spCost;
        this.level = level;
    }

    public Skill() { }
    public enum TargetType
    {
        Self = 0,
        AnAlly = 1,
        OneEnemy = 2,
        Enemy = 3,
        Anyone = 4
    }
    
    private new string name = "";
    public string Name { get { return name; } set { name = value; } }
    private Profession.ProfessionType profType;
    public Profession.ProfessionType ProfType { get { return profType; } set { profType = value; } }
    private DamageType dmgType;
    public DamageType DmgType { get { return dmgType; } set { dmgType = value; } }
    private int reach = 1;
    public int Reach { get { return reach; } set {reach  = value; } }
    //What percentage of the stats it uses; 1 = 100%, .2 = 20% of STR or INT - A la pokemon
    private float damage = 1;
    public float Damage{ get { return damage; } set { damage = value; } }
    private float baseCritChance = 5;
    public float BaseCritChance{ get { return baseCritChance; } set { baseCritChance = value; } }
    private TargetType targets;
    public TargetType Targets { get { return targets; } set { targets = value; } }
    public bool Unlocked;
    public string Description;
    public Effects FX;
    public Element element;
    Weapon wep;
    //Requirement    
    private int mpCost = 0, hpCost = 0, spCost = 0, level = 0;
    public int MpCost { get { return mpCost; } set { mpCost= value; } }
    public int HpCost { get { return hpCost; } set { hpCost = value; } }
    public int SpCost { get { return spCost; } set { spCost = value; } }
    public int Level { get { return level; } set { level = value; } }
    //We should set a "base attack" for each character in case they deal effects naturaly ( think of Grizzly that can apply bleed with their claws)
    public static Skill Base
    {

        get
        {
            var e = new Skill();
            e.Name = "Attack";
            e.SpCost = 2;
            e.Reach = 1;
            e.DmgType = DamageType.Melee;
            e.Damage = .5f;
            e.Targets = TargetType.OneEnemy;
            e.Description = "Nonchalantly attack the target.";
            e.FX = null;
 
            return e;
        }


    }

    public static Skill Weapon(Weapon w)
    {
        if (w == null) return Skill.Base;
        else
        {

            var e = new Skill();
            e.Name = "Attack";
            e.SpCost = 2;
            e.Reach = 1;
            e.DmgType = w.DamageType;
            e.Damage = .5f;
            e.Targets = w.targetType;
            e.wep = w;
            //Effects from weapon are in weapon
            e.element = w.ELEID;
            e.FX = w.FXID;
            
            return e;

        }
    }
    public virtual void Activate(Actor target, Stat stats = new Stat(), Actor f = null)
    {

        var x = Damage;
     
        if ((DamageType.Magical& dmgType) != 0) x *= stats.INT;
        else if ( (DamageType.Physical & dmgType) != 0) x *= stats.STR;

        if (wep != null) x += wep.ATK;

        if ((stats.CriticalHitPercentage + BaseCritChance) > UnityEngine.Random.Range(1, 101))
        {
            x *= 1.50f;
            Debug.Log("Critical Hit!");
        } 
       

        target.TakeDamage(x, this, f);
    }
    public void Activate(Actor[] a, Actor f = null)
    {
        foreach (var item in a) { Activate(item); }
    }





}



