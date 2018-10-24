using System;
using System.Collections;
using System.Collections.Generic;



public abstract class Item : IDisposable
{
    public delegate void OnGrabHandler(Actor a);
    public event OnGrabHandler Ongrabbed;

    public delegate void OnDisposeHandler();
    public event OnDisposeHandler onDispose;
    public virtual void OnGrab(Actor a)
    {

        UnityEngine.Debug.Log(a.ToString() + " takes  " + Name);
        Ongrabbed( a);
    }
    public void OnDispose()
    {
        OnDispose();
    }
    public static List<Item> Inventory = new List<Item>();
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

    public static Item Gold
    {
        get {
            var g = new Gold("Gold", "Items/GOLD");
            g.rarity = Rarity.Common;
            g.GoldValue = 1;
            g.Uses = 0;
            return g; }
    }
    
    public Item (string Name, string Path)
    {
        this.Name = Name;
        ResourcePath = Path;
    }
    /// <summary>
    /// Use on
    /// </summary>
    /// <param name="a">Use the item on this actor</param>
    public virtual void UseOn(Actor a = null)
    {
        UnityEngine.Debug.Log(Name + "is used on " + a.Name);

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

    public override void UseOn(Actor a)
    {
        if(HPregen!=0)a.TakeDamage(-HPregen);
        if (MPregen != 0) a.ConsumeMP(-MPregen);
        if (SPregen != 0) a.ConsumeSP(-SPregen);
        base.UseOn(a);
    }
}

public class Gold: Item
{
   
    public Gold(string Name, string Path):base (Name,Path)
    {
        this.Name = Name;
        ResourcePath = Path;
    }
    public override void OnGrab(Actor a)
    {
        a.AddGold(GoldValue);
        UnityEngine.Debug.Log(a.Name + " gains " +  GoldValue + " gold pieces.");

        if (a.CurrentTile != null) a.CurrentTile.Items.Remove(this);
        base.OnGrab(a);
    }
}
