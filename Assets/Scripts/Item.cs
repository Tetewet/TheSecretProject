using System;
using System.Collections;
using System.Collections.Generic;



public abstract class Item : IDisposable
{
    public static List<Item> Inventory;
    public Skill.TargetType targetType;

    public enum Rarity
    {
        Trash = 0, Common = 1, Uncommon = 2, Rare = 3, Unique = 4, Legendary =5, WorldClass = 6
    }
    public int Uses = 3;
    public string Name;
    public Rarity rarity = Rarity.Common;
    public int GoldValue = 0;

    /// <summary>
    /// Use on
    /// </summary>
    /// <param name="a">Use the item on this actor</param>
    public virtual void On(Actor a)
    {
        if(Uses <= 0)
        {
            Dispose();
            return;
        }
        Uses--;
    }

    public override string ToString()
    {
        return rarity.ToString() + " " + Name;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class Equipement : Item
{

    public delegate void OnEquipementBreak(Item i);
    public event Equipement.OnEquipementBreak OnItemBreak;
    public enum Slot
    {
        Head = 0, Armor =1, Accessory =2, Weapon = 3
    }
    public float Durability = 10;
    
    public stat StatsBonus;
    public Slot slot;
    public float DEF = 0;
    public float MagDEF = 0;

    public bool Useable
    {
        get
        {
            return Durability >= 0;
        }
    }

    public void TakeDamage(float x)
    {
        Durability -= x;
        if (Durability <= 0)
            if (OnItemBreak != null)  OnItemBreak(this);
    }
    public override string ToString()
    {
        return slot + " " + Name.ToString();
    }
}

public class Consumeable : Item
{
    public stat StatsBonus;
    public float HPregen, MPregen, SPregen;
    
}
