using System;
using System.Collections;
using System.Collections.Generic;


namespace igm
{
    public struct Color
    {
        public float R { get { if (r > 1) r = 1; if (r < 0) r = 0; return r; } }
        public float G { get { if (g > 1) g = 1; if (g < 0) g = 0; return g; } }
        public float B { get { if (b > 1) b = 1; if (b < 0) b = 0; return b; } }
        public float A { get { if (a > 1) a = 1; if (a < 0) a = 0; return a; } }

        public Color(float r, float b, float g)
        {
            this.r = r;
            this.b = b;
            this.g = g;
            this.a = 1;
        }
        public Color(float r, float b, float g, float a)
        {
            this.r = r;
            this.b = b;
            this.g = g;
            this.a = a;
        }
        float r, g, b, a;
 
    }

}


public abstract class Item : IDisposable,IUniversalID
{
    public delegate void OnGrabHandler(Actor a);
    public event OnGrabHandler Ongrabbed;

    public delegate void OnDisposeHandler();
    public event OnDisposeHandler onDispose;
    public virtual void OnGrab(Actor a)
    {

        UnityEngine.Debug.Log(a.ToString() + " takes  " + Name);
        if(Ongrabbed!= null)
        Ongrabbed( a);
    }
    public void OnDispose()
    {
        OnDispose();
    }
    public static List<Item> Inventory = new List<Item>();
    public string Description;
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
    public virtual string ResourcePath
    {
        get { return resourcepath; }
        set { resourcepath = value; }
    }
    string resourcepath;
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
        resourcepath = Path;
       ID = GameManager.GenerateID(this);

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
       if(onDispose!=null) onDispose();
        GC.SuppressFinalize(this);
    }

    private readonly string ID;
    public string GetID()
    {
        return ID;
    }
}

interface IUniversalID
{ 
    string GetID();
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

    /*public override string ResourcePath
    {
        get
        {

            return "~IGW";
        }
 
    }*/
    public Stat StatsBonus;
    public Slot slot;
    public float DEF = 0;
    public float MagDEF = 0;


    public Resistance resistance = new Resistance();

    public Equipement(string Name, string Path = "") : base(Name, Path)
    {
        ResourcePath = Path;
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


public class Weapon : Equipement
{
   public enum type
    {
        //Dagger, Sword, GreatSword
         Sword = 1,
         //Wand, Staff, Scepter
        Staff =2 ,
        //Mace, Warmace, Hammer
        Mace =3,
        //Bow, Arbalest, Cannon
        Bow =  4 ,
        //Spear, Lance
        Lance = 5  ,
        //Bible, Tome
        Tome = 6 ,
        //Axe, WarAxe
        Axe = 7, 
    }
    //Bonus Attacks - Raw Addtional Damage, this should be keep low 
    public int ATK = 0;
    public int FXID = -1;
    public int ELEID = -1;

    public class EquipmentSpriteData
    {
        public Component Blade, Hilt, Enchant;
        public class Component
        {

            public Component(igm.Color c, int a)
            {
                this.Color = c;
                this.SpriteIndex = a;
            }
            public igm.Color Color;
            public int SpriteIndex;
        }
        public override string ToString()
        {
            return "~SpriteData: Blade  =" + Blade.SpriteIndex +" Hilts =" + Hilt.SpriteIndex + " Enchant ="+Enchant.SpriteIndex ;
        }
    }
    public bool HasEquipementData
    {
        get { return SpriteData != null; }
    }
    public EquipmentSpriteData GetSpriteData
    {
        get { return SpriteData; }
    }
    EquipmentSpriteData SpriteData;
    public type WeaponType;
    public Weapon(string Name) : base(Name )
    {
 
    }

    public void SaveSpriteData(EquipmentSpriteData s)
    {
        SpriteData = s;
    }
    public DamageType DamageType;
    
}
public class Consumeable : Item
{
    public Stat StatsBonus;
    public float HPregen, MPregen;
    public int SPregen;
    public List<int> EffectToCure = new List<int>();

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
