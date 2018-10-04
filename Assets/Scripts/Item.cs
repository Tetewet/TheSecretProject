using System.Collections;
using System.Collections.Generic;



public abstract class Item
{
    public enum Rarity
    {
        Trash = 0, Common = 1, Uncommon = 2, Rare = 3, Unique = 4, Legendary =5, WorldClass = 6
    }

    public string Name;
    public Rarity rarity = Rarity.Common;
    public int GoldValue = 0;

    /// <summary>
    /// Use on
    /// </summary>
    /// <param name="a">Use the item on this actor</param>
    public virtual void On(Actor a)
    {

    }

    public override string ToString()
    {
        return rarity.ToString() + " " + Name;
    }
}

public class Equipement : Item
{
    public enum Slot
    {
        Head = 0, Armor =1, Accessory =2
    }
    public stat StatsBonus;
    public Slot slot;
    public float DEF = 0;
    public float MagDEF = 0;
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
