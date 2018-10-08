using System;
using System.Collections;
using System.Collections.Generic;

//Can't use Unity Classes.... Creating my Own
public struct Vector
{
    public float x, y;
  
    public Vector(Vector v)
    {
        this.x = v.x;
        this.y = v.y;
    }
    public Vector (float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    public static float Distance(Vector a, Vector b)
    {
        return Math.Abs( (float)Math.Sqrt(Math.Pow((b.x - a.x),2) + Math.Pow((b.y - b.y),2)) );
    }
    public float magnitude { get { return Math.Abs((x + y) / 2); } }
    public Vector normalized { get { return new Vector(x, y) / magnitude; } }
    
    public static Vector zero { get { var v = new Vector(); v.x = 0; v.y = 0; return v; } }
    public static Vector one { get { var v = new Vector(); v.x = 1; v.y = 1; return v; } }
    public static Vector up { get { var v = new Vector(); v.x = 0; v.y = 1; return v; } }
    public static Vector right { get { var v = new Vector(); v.x = 1; v.y = 0; return v; } }


    public static Vector operator /(Vector a, float b)
    {
        var e = new Vector(a.x, a.y);
        e.x /= b;
        e.y /= b;
        return e;
    }
    public static Vector operator *(Vector a, float b)
    {
        var e = new Vector(a.x, a.y);
        e.x *= b;
        e.y *= b;
        return e;
    }
    public static Vector operator +(Vector a, Vector b)
    {
        var e = new Vector(a.x, a.y);
        e.x  += b.x;
        e.y  += b.y;

   
        return e;
    }
    public static Vector operator -(Vector a, Vector b)
    {
        var e = new Vector(a.x, a.y);
        e.x -= b.x;
        e.y -= b.y;
        return e;
    }
    public static bool operator ==(Vector a, Vector b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Vector a, Vector b)
    {
        return !(a == b);
    }
    public override string ToString()
    {
        return "{X:" + x + " , Y:" + y + "}";
    }
}

public abstract class Actor : IComparable<Actor> {


    public const float BASEEXP = 100;
    public string Name;
    public bool Controllable = true;
    //Level
    public int GetLevel
    {
        get { return Level; }
    }
    private int Level = 1;
    protected float EXP = 0;
    public float GetEXP
    {
        get { return EXP; }
    }
    public virtual float RequiredEXP
    {
        get { return BASEEXP * (2 * (float)Math.Exp(Level)); }
    }
    public virtual void AddExp(float exp)
    {
        EXP += exp;
        if (EXP >= RequiredEXP) LevelUP();
    }
    public virtual void LevelUP()
    {
        //Even when there is a level up

        Level++;
    }
    public Actor(string Name, stat BaseStats, bool Controllable)
    {
        this.Name = Name;
        this.baseStats = BaseStats;
        this.Controllable = Controllable;
        
    }

    //Status
    public float HP = 10;
     public float MP = 10;
    public int SP = 10;
    public void Heal()
    {
        HP = GetStats.MaximumHP;
        MP = GetStats.MaximumMP;
        SP = 0;
    }


    //Stats
    protected stat baseStats = new stat();
    public Profession Class = new Profession(new stat());
    public stat GetStats
    {
        get { return Class.GetBase + baseStats; }
    }
    public int RemainingPoint
    {
        get {
            if (baseStats.STR < 0) baseStats.STR = 0;
            if (baseStats.AGI < 0) baseStats.AGI = 0;
            if (baseStats.END < 0) baseStats.END = 0;
            if (baseStats.WIS < 0) baseStats.WIS = 0;
            if (baseStats.INT < 0) baseStats.INT = 0;

            return (Level + 1) * 5 - (baseStats.STR + baseStats.AGI + baseStats.END + baseStats.WIS + baseStats.INT); }
    }

    
    //Action
    public delegate void DamageHandler(float z,Skill x );

    public event DamageHandler OnAttack, OnDamage, OnBlocked;
    
    public virtual void Use(Skill s, Actor[] Target)
    {

        if (HP >= s.HpCost) { if (s.HpCost != 0) TakeDamage(s.HpCost); } 
            else { UnityEngine.Debug.Log(Name + " not enough HP :" + HP +"/" + s.HpCost ); return; }

        if (MP >= s.MpCost) { if (s.MpCost != 0) ConsumeMP(s.MpCost); }
        else { UnityEngine.Debug.Log(Name + " not enough MP: " + MP + "/" + s.MpCost); return; }

        if (SP >= s.SpCost) { if (s.SpCost != 0) ConsumeSP(s.SpCost); }
        else { UnityEngine.Debug.Log(Name + " not enough SP: " + SP + "/" + s.SpCost); return; }

        UnityEngine.Debug.Log(Name + " uses " + s.Name);
        foreach (var item in Target) s.Activate(item, GetStats);

    }
    public virtual void Use(Item i, Actor[] T)
    {
        foreach (var item in T)
        {
           if(i.Uses >0) i.OnUse(item);
           else
            {
                for (int x = 0; x < inventory.items.Length; x++)
                    inventory.items[x] = null;
                i.Dispose();
                break;
            }
        } 
         
    }
    public void TakeDamage(float x)
    {
        TakeDamage(x, null );
    }
    public virtual void TakeDamage(float x, Skill f , Actor a = null)
    {

        var i = x;
        if(f != null) 
        {
            if (f.Type == DamageType.Magical) x -= GetStats.MagDEF;
            else if (f.Type == DamageType.Physical) x -= GetStats.PhysDEF;
            if (x <= 0 && f.Type != DamageType.None)
            {
                if(OnBlocked!= null)OnBlocked(x, f);
                UnityEngine.Debug.Log(f.Name + " has no effects! - " + i + " damages against " + GetStats.PhysDEF);
                return;
            }
            UnityEngine.Debug.Log(Name + " took " + f.Type + " " + x);
        }
       
        HP -= x;
        if (HP <= 0) { Ondeath(x, f, a);  HP = 0; }
    
        if (OnDamage != null) OnDamage(x,f);
    }
    
    public virtual void Ondeath(float x, Skill f , Actor a = null)
    {
   
    }
    public void ConsumeMP(float x )
    {
        MP -= x;
    }
    public void ConsumeSP(int x)
    {
        SP -= x;
        if (SP < 0) SP = 0;
    }

     public int tilewalked = 0;
    public int TileWalkedThisTurn = 0;
    public void Grab( Item a)
    {

        for (int i = 0; i < inventory.items.Length; i++)
        {
            if (inventory.items[i] == null)
            {   
                inventory.items[i] = a;
                a.OnGrab(this);
                break;

            }
        }
    }
    //Inventory

 
    public Map.Tile CurrentTile = new Map.Tile();
  
    public Vector TilePosition
    {
        get { return CurrentTile.Position; }
    }
    //Act for one turn. Must be in a battle
    public virtual void Turn(Battle battle)
    {
        if (IsDefeat) return;
        //This turn
        SP += 3;
        this.TileWalkedThisTurn = 0;
        
        if (SP > 10) SP = 10;
        Battle.Turn ThisTurn = battle.ThisTurn;
       if(OnTurn!=null) OnTurn(ThisTurn);
        //Act Here - Add logic for monster here             
        
    }
    public virtual void OnTurnEnded()
    {
        
      
    }

    public delegate void OnTurnHandler(Battle.Turn e);
    public event OnTurnHandler OnTurn;
    //Position In world;
    public void Move(Vector position, int map = 0 )
    {
       
        _transform.position += position;
        
    }

    //Move with bound and collision in mind 
    public virtual void Move(Vector v,bool bypass = false)
    {
        if (v == Vector.zero) return;
        var b = GameManager.CurrentBattle;
        var e =  CurrentTile.Position + v;

        if (e.x < 0) e.x = 0;if (e.x > b.map.Width) e.x = b.map.Width - 1;
        if (e.y < 0) e.y = 0; if (e.y > b.map.Length) e.y = b.map.Length - 1;
        if(!bypass)
        for (int x = 0; x < b.map.Width; x++)
        {
           
            for (int y = 0; y < b.map.Length; y++)
            {
                //Check if there is something blocking the right
                if (e.normalized.x > 0 && x < b.map.Width)            
                    if(x >  TilePosition.x && x < e.x)                                  
                        if (b.map.Tiles[x, y].Actor != null || !b.IsTeamWith(this, b.map.Tiles[x, y].Actor))
                        { UnityEngine.Debug.Log((b.map.Tiles[x, y].Actor != null)); CantMove(v);  return; }
                //Check if there is something blocking the  left
                if (e.normalized.x < 0 && x > 0)
                    if (x <  TilePosition.x && x > e.x)
                        if (b.map.Tiles[x, y].Actor != null || !b.IsTeamWith(this, b.map.Tiles[x, y].Actor)) { CantMove(v); return; }
                //Check if ....
                if (e.normalized.y  > 0 && y < b.map.Length)
                    if (y >  TilePosition.y && y < e.y)
                        if (b.map.Tiles[x, y].Actor != null || !b.IsTeamWith(this, b.map.Tiles[x, y].Actor)) { CantMove(v); return; }

                //Ch...
                if (e.normalized.y < 0 && y > 0)
                    if (y >  TilePosition.y && y < e.y)
                        if (b.map.Tiles[x, y].Actor != null || !b.IsTeamWith(this,b.map.Tiles[x, y].Actor)) { CantMove(v); return; }

            }
        }


        // Move(b.map.AtPos(v));
        
        Path.Clear();
        CreatePath(b.map.AtPos(v));
        return;
    }
    //Position In Battle

    public Queue<Vector> Path = new Queue<Vector>();
    public void Teleport(Map.Tile where)
    {
        CurrentTile.OnQuitting();
        CurrentTile = where;      
        CurrentTile.Enter(this);

    }
    public void CreatePath(Map.Tile where)
    {

        Path.Clear();
               int x = (int)(where.x -TilePosition.x );
               int y = (int)(where.y - TilePosition.y );
        UnityEngine.Debug.Log(Name + ": Creating Path : from "+TilePosition.ToString() + " to " + where.Position.ToString() + " Offset: {" + x + " " + y +"}" );
               var a = 1;
               var b = 1;
               if (x < 0) a = -1;
               if (y < 0) b = -1;

        /*
        for (int i = 0; i <=  Math.Abs(x); i++) Path.Enqueue( TilePosition + Vector.right * i * a);
        for (int i = 0; i <= Math.Abs(y) + 1; i++) Path.Enqueue(TilePosition + Vector.right * x + Vector.up * i * b);
        */


        if (Math.Abs(x) > Math.Abs(y) || GameManager.CurrentBattle.map.AtPos(TilePosition + Vector.up * b).Actor != null)
        {
            for (int i = 1; i <= Math.Abs(x); i++)
            { if (TilePosition + Vector.up * i * b == TilePosition) continue; Path.Enqueue(TilePosition + Vector.right * i * a); }
            // y +1
            for (int i = 1; i <= Math.Abs(y)  ; i++) Path.Enqueue( TilePosition + Vector.right * x + Vector.up * i * b);
        }
        else
        {
            for (int i = 1; i <= Math.Abs(y); i++)
            { if (TilePosition + Vector.up * i * b == TilePosition) continue; Path.Enqueue(TilePosition + Vector.up * i * b); }
            //x + 1
            for (int i = 1; i <= Math.Abs(x)  ; i++) Path.Enqueue( TilePosition + Vector.up * y + Vector.right * i * a);
        }

       // if(where.Actor == null )Path.Enqueue(where.Position);
        //We will have to move this section in the Unity section to sync the position
        /*
       while (Path.Count > 1)
       {
           CurrentTile.OnQuitting();
           if (GameManager.CurrentBattle.map.AtPos(Path.Peek()).Actor != null)
           {
               CantMove();
               break;
           }
           CurrentTile = GameManager.CurrentBattle.map.AtPos(Path.Dequeue());

           CurrentTile.Enter(this);
       }
       */

    }

    public virtual void Move(Map.Tile where)
    {
       var v = Vector.Distance(where.Position,CurrentTile.Position);

     
        if (where.Actor != null)
        {
            CantMove(where.Position);
            return;
        }

        Path.Clear();
        CreatePath(where);
        return;
 
    }
    
   public void CantMove(Vector v)
    {
        //DEBUG
        UnityEngine.Debug.Log("Couldn't move to " + v);
        Path.Clear();
    }
    protected Transform _transform;
    /// <summary>
    /// World-Related Variables (Position, Direction )
    /// </summary>
    public Transform transform
    {
        get{ return _transform; }
       
    }
    public struct Transform
    {
         public  enum Direction
        {
            Up =1, 
            Down =2,
            Right = 3,
            Left =4
        }
       
    
        public Vector position
        {
            get { return _position; }
            set { _position = value; }
        }
        Vector _position;
        int Map;
        Direction direction;
        public void Teleport(Vector Tile, int Map = -1, int dir = -1)
        {
            position = Tile;
            if (Map >= 0) this.Map = Map;
            if(dir >= 0)this.direction = (Direction)dir;
        }
        public void Teleport(Vector Tile, Direction dir, int Map = -1)
        {
            position = Tile;
            this.direction = dir;
            if (Map >= 0) this.Map = Map;
         
        }


    }

    public Inventory inventory = Inventory.Light;
    //Inventory
    public class Inventory
    {
        //Not everyone have the same size of inventory. Not everybody have the same size of bag nor can equip as much items. Some cannot equip anything,
        public void OnItemBreak(Item a)
        {

        }
        public Item[] items;
    
        public EquipementSlot[] Slot;

        /// <summary>
        /// Light Equipement Build
        /// </summary>
        public static Inventory Light
        {
            get
            {
                var e = new Inventory();

                e.items = new Item[3];
                e.Slot = new EquipementSlot[4];
                e.Slot[0].SlotType = global::Equipement.Slot.Armor;
                e.Slot[1].SlotType = global::Equipement.Slot.Head;
                e.Slot[2].SlotType = global::Equipement.Slot.Accessory;
                e.Slot[3].SlotType = global::Equipement.Slot.Weapon;
               // foreach (var ITEM in e.Slot) ITEM.item.OnItemBreak += e.OnItemBreak;

                return e;
            }
        }
        public void ChangeInventory(Inventory r, Inventory z)
        {
            var g = this;
        
            Slot = z.Slot;
             items = z.items;
            var t = new List<Equipement>();
            foreach (var item in g.Slot)
                t.Add(item.item);

            for (int i = 0; i < t.Count; i++)
            {
                if (t[i].slot ==  Slot[i].SlotType)
                {
                    Slot[i].Equip(t[i]);
                }
            }
 
        }


        public bool IsFull
        {
            get
            {
                bool t = true;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null) t = false;
                }
                return t;
            }
        }
        public struct EquipementSlot
        {
            public string SlotName;
            public Equipement item;
            public Equipement.Slot SlotType;
            public void Equip(Equipement q)
            {
                if (q.slot != SlotType) return;
                item = q;


            }
        }
    }


    /// <summary>
    /// Some Actor have different defeat condition - but for most, it's on fainted
    /// </summary>
    public virtual bool IsDefeat
    {
        get { return HP <= 0; }
    }
   
    
    //Functionality
    public static Actor operator ++(Actor a)
    {
        a.AddExp((a.RequiredEXP - a.EXP) + 1);
        return a;
    }
    public override string ToString()
    {
        return Name + " Lvl: " + GetLevel;
    }

    public int CompareTo(Actor other)
    {
        //Turn order is define by priority + AGI
        var t = GetStats.Priority + GetStats.AGI;
        var x = (other.GetStats.AGI + other.GetStats.Priority);
        //In unity we will be using a bar having these charging it each tick. The first to 100 Act first

        if (t == x) (t + GetStats.LUC).CompareTo(x + other.GetStats.LUC);
        return t.CompareTo(x); 
    }
}

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

    static Random SkillRandom = new Random();
    public string Name ="";
    public DamageType Type;
    public int Reach = 1;
    //What percentage of the stats it uses; 1 = 100%, .2 = 20% of STR or INT - A la pokemon
    public float Damage  = 1;
    protected float BaseCritChance = 5;
    public TargetType Targets;
    

    //Requirement    
    public float MpCost =0, HpCost = 0;
    public int SpCost = 0;
    public static Skill Base {

        get
        {
            var e = new Skill();
            e.Name = "Attack";
            e.SpCost = 1;
            e.Reach = 1;
            e.Type = DamageType.Physical;
            e.Damage = .25f;
            e.Targets = TargetType.OneEnemy;
            return e;
        }
    }
    public virtual void Activate(Actor Target, stat Stats = new stat())
    {
     
        var x = Damage;
        if (Type == DamageType.Magical) x *=  Stats.INT;
        else if (Type == DamageType.Physical) x *= Stats.STR;
        
        if ((Stats.LUC * 2 + BaseCritChance) > SkillRandom.Next(0, 100)) Damage *= 1.50f;

            Target.TakeDamage(x,this);
    }
    public void Activate(Actor[] a)
    {
        foreach (var item in a) { Activate(item);  } 
    }



    

}
[Flags]
public enum _stats
{
    STR = 1, AGI = 2, END =4, WIS = 8, INT = 16, LUC = 32 
}
public enum DamageType
{
    None = 0,
    Physical =1,
    Magical =2,
    Pierce = 3,
    
}
/// <summary>
/// Stats of any living being.
/// </summary>
public struct stat : IComparable<stat>
{

    public delegate void OnStatsGainHandler();
    public event OnStatsGainHandler OnGainStats;
    public int STR, AGI, END, WIS, INT, LUC;
    //For fine tuning turn order
    public int Priority;
    //For fine tuning targeting
    public int Threat;
    public float PhysDEF { get { return ( STR / 2 + END + AGI / 4)/2; } }
    public float MagDEF { get { return (INT + WIS * 2)/2; } }
    public float MaximumHP { get { return END * 5; } }
    public float MaximumMP { get { return WIS * 5; } }

    public int Magnitude { get { return (STR + AGI + END + WIS + INT + LUC) / 6; } }

    public void AddStats(int x, _stats s)
    {
        switch (s)
        {
            case _stats.STR:
                STR += x;
                break;
            case _stats.AGI:
                AGI += x;
                break;
            case _stats.END:
                END += x;
                break;
            case _stats.WIS:
                WIS += x;
                break;
            case _stats.INT:
                INT += x;
                break;
            case _stats.LUC:
                LUC += x;
                break;
            default:
                break;
        }
        OnGainStats();
    }
    public void AddStats(stat a)
    {
        this += a;
        if (OnGainStats != null) OnGainStats();
        else Console.WriteLine("No OnGainStats");
    }

    public int CompareTo(stat other)
    {
        return (Threat + Magnitude).CompareTo(Threat + other.Magnitude);
    }

    //Useful Functions


    public static stat zero
    {
        get { return new stat(); }
    }
    public static stat operator +(stat a, stat b)
    {
        var e = new stat();
        e.STR = a.STR + b.STR;
        e.INT = a.INT + b.INT;
        e.AGI = a.AGI + b.AGI;
        e.WIS = a.WIS + b.AGI;
        e.END = a.END + b.END;
        e.LUC = a.LUC + b.LUC;
        e.OnGainStats += a.OnGainStats + b.OnGainStats;
        
        return e;
    }
    public static stat operator *(stat a, int b)
    {
        var e = new stat();
        e.STR = a.STR * b;
        e.INT = a.INT * b;
        e.AGI = a.AGI * b;
        e.WIS = a.WIS * b;
        e.END = a.END * b;
        e.LUC = a.LUC * b;
        e.OnGainStats += a.OnGainStats;
        return e;
    }
}
public class Profession
{

    public const float BASEPROFIENCYEXP= 10;
 
    public string Name = "Adventurer";

    private int Profiency = 0;
    public int GetProfiency
    {
        get { return Profiency; }
    }
    
    protected stat BaseStats;
    protected float ClassEXP = 0;

    public Skill[] SkillList;
    
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
    public virtual stat GetBase
    {
        get { return BaseStats; }
    }
       
    public Profession(stat s, string Name = "Adventurer")
    {
        this.BaseStats = s;
        this.Name = Name;
        
    }

}
