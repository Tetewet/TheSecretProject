using System;
using System.Collections;
using System.Collections.Generic;



public abstract class Item : IDisposable
{
    public delegate void OnGrabHandler(Actor a);
    public event OnGrabHandler Ongrabbed;

    public delegate void OnDisposeHandler();
    public event OnDisposeHandler onDispose;
    public void OnGrab(Actor a)
    {
        Ongrabbed( a);
    }
    public void OnDispose()
    {
        OnDispose();
    }
    public static List<Item> Inventory;
    public Skill.TargetType targetType;
    public Map.Tile CurrentTile = new Map.Tile();
    public Vector TilePosition
    {
        get { return CurrentTile.Position; }
    }
    public enum Rarity
    {
        Trash = 0, Common = 1, Uncommon = 2, Rare = 3, Unique = 4, Legendary =5, WorldClass = 6
    }
    public int Uses = 1;
    public string Name;
    public string ResourcePath;
    public Rarity rarity = Rarity.Common;
    public int GoldValue = 0;

    
    public Item (string Name, string Path)
    {
        this.Name = Name;
        ResourcePath = Path;
    }
    /// <summary>
    /// Use on
    /// </summary>
    /// <param name="a">Use the item on this actor</param>
    public virtual void OnUse(Actor a = null)
    {
        Uses--;
        if (Uses <= 1)
        {
            //It Dispose of it after it has being remove from every inventory
           // Dispose();
            return;
        }
       
    }

    public override string ToString()
    {
        return rarity.ToString() + " " + Name;
    }

    public void Dispose()
    {
        if (Inventory.Contains(this)) Inventory.Remove(this);
        if(CurrentTile!=null) CurrentTile.Items.Remove(this);
        onDispose();
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

    public Equipement(string Name, string Path) : base(Name, Path)
    {
    }

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
    public float HPregen, MPregen;
    public int SPregen;

    public Consumeable(string Name, string Path) : base(Name, Path)
    {
    }

    public override void OnUse(Actor a)
    {
        if(HPregen!=0)a.TakeDamage(-HPregen);
        if (MPregen != 0) a.ConsumeMP(-MPregen);
        if (SPregen != 0) a.ConsumeSP(-SPregen);
        base.OnUse(a);
    }
}
