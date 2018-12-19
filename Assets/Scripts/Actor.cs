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

    /// <summary>
    /// (0,0)
    /// </summary>
    public static Vector zero { get { var v = new Vector(); v.x = 0; v.y = 0; return v; } }
    /// <summary>
    /// (1,1)
    /// </summary>
    public static Vector one { get { var v = new Vector(); v.x = 1; v.y = 1; return v; } }
    /// <summary>
    /// (0,1)
    /// </summary>
    public static Vector up { get { var v = new Vector(); v.x = 0; v.y = 1; return v; } }
    /// <summary>
    /// (1,0)
    /// </summary>
    public static Vector right { get { var v = new Vector(); v.x = 1; v.y = 0; return v; } }
    /// <summary>
    /// (0,-1)
    /// </summary>
    public static Vector down { get { var v = new Vector(); v.x = 0; v.y = -1; return v; } }
    /// <summary>
    /// (-1,0)
    /// </summary>
    public static Vector left { get { var v = new Vector(); v.x = -1; v.y = 0; return v; } }

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

public abstract class Actor : IComparable<Actor>,IUniversalID {

    public bool IsInPlayerTeam
    {
        get { return GameManager.CurrentBattle.Players.Contains(this); }
    }
    public const float BASEEXP = 10;
    public string Name, AnimatorPath;
    public bool Controllable = true;


    //Level
    public delegate void OnGainEXP(float x);
    public event OnGainEXP OnExpGain;
    public delegate void OnKillHandler(Actor a);
    public event OnKillHandler OnKillActor;
 
    
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
        get { return BASEEXP * ((float)Math.Exp(Math.Abs(Level - 1))); }
    }

    public virtual void AddExp(float exp)
    {
        EXP += exp;
        UnityEngine.Debug.Log(Name + " gained " + exp + " exp.");
        if (OnExpGain != null) OnExpGain(exp);
        if (EXP >= RequiredEXP) LevelUP();
    }
    public virtual void LevelUP()
    {
        //Even when there is a level up
        UnityEngine.Debug.Log(Name + "is now level" + GetLevel);
        var e = new Random();
        baseStats += new Stat() { END = e.Next(0,5), STR = e.Next(0, 5), INT = e.Next(0, 5), LUC = e.Next(0, 5), AGI = e.Next(0, 5), WIS = e.Next(0, 5) };
        Level++;
    }

    public Vector DefaultPos = new Vector(5, 2);
    public Actor(string Name, Stat BaseStats, bool Controllable, string AnimatorPath)
    {
        this.Name = Name;
        this.baseStats = BaseStats;
        this.Controllable = Controllable;
        this.AnimatorPath = AnimatorPath;
        ID = GameManager.GenerateID(this);
    }

    //Oy Vey Goyim
    protected int Gold = 0;
    public void AddGold(int g)
    {
        Gold += g;
    }
    public void RemoveGold(int g)
    {
        Gold -= g;
    }
    //Status
    public float HP = 10;
    public float MP = 10;
    public int SP = 10; 
    public bool Defending = false;
    public void Heal()
    {
        HP = GetStats.MaximumHP;
        MP = GetStats.MaximumMP;
        Defending = false;
        SP = 0;                     
    }

    public Functionality ActionsPermis
    {
        get
        {
            if (effects.Count == 0) return Functionality.normal;
            var e = new Functionality();
            foreach (var item in effects)
                e *= item.Func;
            return e;


        }
    }
    Resistance resistance = new Resistance();

    public List<Effects> effects = new List<Effects>();
 
    //Stats
    protected Stat baseStats = new Stat();
    public Profession Class = new Profession(new Stat());
    public string Description = "A normal actor.";

   
    
    public Stat GetStats
    {
        get {
            var t = Class.GetBase + baseStats;
            if (inventory.Slot.Length > 0)        
                foreach (var x in inventory.Slot)
                {
                    if (x.item != null)
                        t += x.item.StatsBonus;
                }
            if(effects.Count > 0)
                foreach (var x in effects)
                {
                    t += x.StatChange;
                }
 
            return t; }
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
    public delegate void DamageHandler(float z, Skill x);

    public event DamageHandler OnAttack, OnDamage, OnBlocked, OnDeath;



    public bool CanUseSkill(Skill s)
    {
        if (s == null)
            s = Skill.Base;
        if (s.HpCost > HP) { UnityEngine.Debug.Log(Name + " not enough HP :" + HP + "/" + s.HpCost); return false; }
        if ( s.MpCost > MP ) { UnityEngine.Debug.Log(Name + " not enough MP: " + MP + "/" + s.MpCost); return false; }
        if(s.SpCost > SP) { UnityEngine.Debug.Log(Name + " not enough SP: " + SP + "/" + s.SpCost); return false; }

        return true; 
    }
    public virtual void UseEffect(Skill s, Actor Target = null)
    {
        if (s == null) return;
        if (!CanUseSkill(s)) return;

        if (s.HpCost > 0) TakeDamage(s.HpCost);
        if (s.MpCost > 0) ConsumeMP(s.MpCost);
        if (s.SpCost > 0) ConsumeSP(s.SpCost);

        UnityEngine.Debug.Log(Name + " uses " + s.Name + " on " + Target.Name);
        DamageType dam = s.DmgType;
        UnityEngine.Debug.Log(Enum.Parse(typeof(DamageType),(DamageType.Offensive & s.DmgType).ToString()));
        if (( DamageType.Offensive & s.DmgType) == s.DmgType) {
            dam = DamageType.Offensive;
            UnityEngine.Debug.Log("Offensive");
        }
        if ((DamageType.Effects & s.DmgType) == s.DmgType) {
            dam = DamageType.Effects;
            UnityEngine.Debug.Log("Effects");
        }
        switch (dam)
        {
            case DamageType.Offensive:
                s.ApplyAttack(Target, GetStats, this);
                UnityEngine.Debug.Log("Applied Attack");
                break;
            case DamageType.Effects:
                Target.Apply(s.FX);
                UnityEngine.Debug.Log("Applied Effects");
                break;
            case DamageType.None:
                s.DoEffect(GameManager.CursorPos,this);
                UnityEngine.Debug.Log("Applied Ability");
                break;
        }

    }
    public virtual void UseEffect(Skill s, Actor[] Target)
    {
        if (s == null) return;
        if (!CanUseSkill(s)) return;

        if (s.HpCost > 0) TakeDamage(s.HpCost);
        if (s.MpCost > 0) ConsumeMP(s.MpCost);
        if (s.SpCost > 0) ConsumeSP(s.SpCost);

        UnityEngine.Debug.Log(Name + " uses " + s.Name);
        foreach (var item in Target)
        {
            DamageType dam = s.DmgType;
            UnityEngine.Debug.Log(Enum.Parse(typeof(DamageType), (DamageType.Offensive & s.DmgType).ToString()));
            if ((DamageType.Offensive & s.DmgType) == s.DmgType)
            {
                dam = DamageType.Offensive;
                UnityEngine.Debug.Log("Offensive");
            }
            if ((DamageType.Effects & s.DmgType) == s.DmgType)
            {
                dam = DamageType.Effects;
                UnityEngine.Debug.Log("Effects");
            }
            switch (dam)
            {
                case DamageType.Offensive:
                    s.ApplyAttack(item, GetStats, this);
                    break;
                case DamageType.Effects:
                    item.Apply(s.FX);
                    break;

            }
            
        }

    }



    public virtual void Use(Item i, Actor[] T)
    {
        i.Uses--;
        foreach (var item in T)
        {
            i.UseOn(item);
            if (i.Uses <= 0)
            {
                for (int x = 0; x < inventory.items.Length; x++)
                    if (inventory.items[x] == i) inventory.items[x] = null;
                i.Dispose();
                break;
            }
        }

    }
    public void SetProfession(Profession p)
    {
        this.Class = p;
        OnTurn += Class.ClassLogic;
    }
    public virtual void Equip(Equipement q)
    {
        var f = false;
        foreach (var g in inventory.Slot)
            if(g.item == null && g.SlotType == q.slot) { f = true; g.Equip(q); }

        if (!f) return;
        if (OnEquip!= null) OnEquip(q);
        UnityEngine.Debug.Log(Name + " equips " + q.Name);

    }
    public void OnMurder(Actor a)
    {
        if (OnKillActor != null) OnKillActor(a);

    }
    public void Use(Item i, Actor T)
    {
        Use(i, new Actor[1] { T });
    }
    public void TakeDamage(float x)
    {
        TakeDamage(x, null );
    }
    public virtual void Apply(Effects fx)
    {
        if (fx == null) return;
        effects.Add(fx);
        OnTurn += fx.OnTurn;
        OnDamage += fx.OnBeingHit;
        OnAttack += fx.OnAttack;
        fx._actor = this;
        UnityEngine.Debug.Log(fx.StatChange);

        if (OnApplyEffets != null) OnApplyEffets(fx);
        // We have a lot to add...
        
    }

    public virtual void Remove(Effects fx)
    {
        if (!effects.Contains(fx)) return;
        effects.Remove(fx);
        OnTurn -= fx.OnTurn;
        OnDamage -= fx.OnBeingHit;
        OnAttack -= fx.OnAttack;
        fx._actor = null;
        if (OnApplyEffets != null) OnApplyEffets(fx);
    }
    public Resistance getRes
    {
        get
        {
            var e = new Resistance();

            e += resistance;
            foreach (var item in inventory.Slot)        
               if(item.item != null) e += item.item.resistance;        
            return e;
        }
    }
    public virtual void TakeDamage(float x, Skill f , Actor a = null)
    {

        var i = x;
        if(f != null)
        {
            if ((f.DmgType & DamageType.Offensive) != 0)
            {
                if (Defending) x *= .5f;

                if (f.element.ID >= 0)
                {
                    foreach (var item in getRes.resistance)
                        x *= f.element.EfficacyFactor(item, this);
                    foreach (var item in getRes.weakness)
                        x *= f.element.EfficacyFactor(item, this);
                }


                if ((f.DmgType & DamageType.Magical) != 0) x -= GetStats.MagDEF;
                else if ((f.DmgType & DamageType.Physical) != 0) x -= GetStats.PhysDEF;
                if (x <= 0 )
                {
                    if (OnBlocked != null) OnBlocked(x, f);

                    UnityEngine.Debug.Log(f.Name + " has no effects! - " + i + " damages against  def:" + GetStats.PhysDEF + ",mDef" + GetStats.MagDEF);
                    return;
                }

                if (OnDamage != null) OnDamage(x, f);
                UnityEngine.Debug.Log(Name + " took " + f.DmgType + " " + x);
            }
            Apply(f.FX);

        }


        HP -= x;
        if (HP <= 0) { Ondeath(x, f, a);  HP = 0; }
    
       
    }
    
    public virtual void Ondeath(float x, Skill f , Actor a = null)
    {
        OnDeath(x, f);
        if (GameManager.CurrentBattle.Foes.Contains(this))
            GameManager.CurrentBattle.Foes.Remove(this);
        if (GameManager.CurrentBattle.Players.Contains(this))
            GameManager.CurrentBattle.Players.Remove(this);

        CurrentTile.OnQuitting();

        UnityEngine.Debug.Log(this.Name + " is death");
    }
    public void ConsumeMP(float x )
    {
        MP -= x;
    }
    public void ConsumeSP(int x)
    {
        SP -= x;
        if (SP < 0) SP = 0;
        if(x > 0)
        {
            SpAvaillableThisTurn += x;
        }
    }

     public int tilecounter = 0;

    public int TileWalkedThisTurn = 0;
    public void Grab( Item a)
    {

        if (IsDefeat) return;


        for (int i = 0; i < inventory.items.Length; i++)
        {

            if(a is Gold)
            {
                a.OnGrab(this);
                a.Dispose();
            }
            else 
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
        set { CurrentTile.Position = value; }
    }
    //Act for one turn. Must be in a battle

    public int SpAvaillableThisTurn = 3;
    public virtual void Turn(Battle battle)
    {
        if (IsDefeat) return;
        if (SP < 0) SP = 0;
        if (Defending)
        {
            Defending = false;
            battle.EndTurn();
            return;
        }
       
    
        //This turn
        SP = GetStats.MaximumSP;
        this.TileWalkedThisTurn = 0;
        
        if (SP > GetStats.MaximumSP) SP = GetStats.MaximumSP;
       
        SpAvaillableThisTurn = SP;
       if(OnTurn!=null) OnTurn(battle.ThisTurn);
        //Act Here - Add logic for monster here             
        
    }
    public virtual void OnTurnEnded()
    {
        
      
    }
    public delegate void OnEquipHandler(Equipement e);
    public event OnEquipHandler OnEquip;
    public delegate void OnTurnHandler(Battle.Turn e);
    public event OnTurnHandler OnTurn;
    public delegate void OnApplyEffectHandler(Effects e);
    public event OnApplyEffectHandler OnApplyEffets;
    //Position In world;
    public void Move(Vector position )
    {
       
        _transform.position += position;
        
    }



    /// <summary>
    /// Move with bound and collision in mind 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="bypass"></param>
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
        if(CurrentTile != null) CurrentTile.OnQuitting();
        CurrentTile = where;      
        CurrentTile.Enter(this);

    }

    public void test(Func<int,int,int> t)
    {
        DamageHandler g = (x,y) => { y.DmgType.ToString(); } ;
        test((x, y) => { return x + y; });

    }
    /// <summary>
    /// Create a path toward X
    /// </summary>
    /// <param name="where"></param>
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
        Map e;
        if (GameManager.BattleMode) e = GameManager.CurrentBattle.map;
        else e = GameManager.Map;

        if (Math.Abs(x) > Math.Abs(y) || e.AtPos(TilePosition + Vector.up * b).Actor != null)
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
 }

    bool CanMoveThere(Map.Tile Where)
    {
        var e = true;
        var s = Where.collider;
        if (Where == CurrentTile) e = false;
        if (Where.x > TilePosition.x && (s == Map.Tile.ColliderType.Left || CurrentTile.collider == Map.Tile.ColliderType.Right) ) e = false;
        if (Where.x < TilePosition.x && ( s == Map.Tile.ColliderType.Right || CurrentTile.collider == Map.Tile.ColliderType.Left) ) e = false;
        if (Where.y > TilePosition.y && (s == Map.Tile.ColliderType.Down || CurrentTile.collider == Map.Tile.ColliderType.Up)) e = false;
        if (Where.y < TilePosition.y && (s == Map.Tile.ColliderType.Up || CurrentTile.collider == Map.Tile.ColliderType.Down)) e = false;
        if(s == Map.Tile.ColliderType.All) e = false;
     

        return e;


    }


    public virtual void Move(Map.Tile where)
    {
       var v = Vector.Distance(where.Position,CurrentTile.Position);

     
        if (where.Actor != null || !CanMoveThere(where))
        {
            CantMove(where.Position);
            return;
        }

        Path.Clear();
        CreatePath(where);
        return;
 
    }
    public bool IsTeamWith(Actor a)
    {
        return GameManager.CurrentBattle.IsTeamWith(this, a);
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

        public List<Weapon> GetWeapons
        {
            get
            {
                if (Slot.Length == 0) return null;
                if (!HasWeapon) return null;

                List < Weapon > w = new List<Weapon>();
                foreach (var item in Slot)              
                    if (item.item is Weapon)
                        w.Add(item.item as Weapon);
                return w; 

            }
        }
        public bool HasWeapon
        {
            get
            {
                for (int i = 0; i < Slot.Length; i++)             
                    if (Slot[i].item is Weapon) return true;               
                return false;
            }
        }

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
                e.Slot[0] = new EquipementSlot(Equipement.Slot.Armor);
                e.Slot[1] = new EquipementSlot(Equipement.Slot.Head);
                e.Slot[2] = new EquipementSlot(Equipement.Slot.Weapon);
                e.Slot[3] = new EquipementSlot(Equipement.Slot.Accessory);
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
        public class EquipementSlot
        {

            public EquipementSlot(  Equipement.Slot type)
            {
                this.SlotName = type.ToString();
                this.SlotType = type;
            }
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
        return Name + " " + LanguageDao.GetLanguage("lvl", GameManager.language) + " " + GetLevel;
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
    private readonly string ID;
    public string GetID()
    {
        return ID;
    }
}


[Flags]
public enum _stats
{
    STR = 1, AGI = 2, END =4, WIS = 8, INT = 16, LUC = 32 
}
[Flags]
public enum DamageType
{
    None = 0,
    Melee = 1,
    Magic = 2,
    Pierce = 4,
    Slashing = 8,
    Blunt = 16,
    Buff = 32,
    Debuff = 64,
    Physical = Melee | Pierce | Slashing | Blunt,
    Magical = Magic,
    Effects = Buff | Debuff,
    Offensive = Physical | Magical
}
/// <summary>
/// Stats of any living being.
/// </summary>
public struct Stat : IComparable<Stat>
{

    public delegate void OnStatsGainHandler();
    public event OnStatsGainHandler OnGainStats;
    public int STR, AGI, END, WIS, INT, LUC;
    //For fine tuning turn order
    public int Priority;
    //For fine tuning targeting
    public int Threat;
    public int DamageBonus, DefenseBonus;
    public float PhysDEF { get { return ( STR / 2 + END + AGI / 4)/2; } }
    public float MagDEF { get { return (INT + WIS * 2)/2; } }
    public float MaximumHP { get { if (END == 0) return 1; return END * 2; } }
    public float MaximumMP { get { return WIS * 5; } }
    public float CriticalHitPercentage { get { return CriticalHitFlat + LUC; } }
    public float CriticalHitFlat;
    public int MaximumSP { get {

            int a = 2;
            a += (int)((STR + AGI + END + WIS + INT + LUC) / 22.8571428571);
            if (a > 7) a = 7;
            return a;
        } }

    public int Magnitude { get { return (STR + AGI + END + WIS + INT + LUC) / 6; } }
    public static Stat operator /(Stat stat, int divider)
    {
        stat.AGI =stat.AGI / 2;
        stat.END =stat.END / 2;
        stat.INT =stat.INT / 2;
        stat.LUC =stat.LUC / 2;
        stat.STR =stat.STR / 2;
        stat.WIS =stat.WIS / 2;

        return stat;
    }
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
    public void AddStats(Stat a)
    {
        this += a;
        if (OnGainStats != null) OnGainStats();
        else Console.WriteLine("No OnGainStats");
    }

    public int CompareTo(Stat other)
    {
        return (Priority + AGI).CompareTo(other.Priority + other.AGI);
            //(Threat + Magnitude).CompareTo(Threat + other.Magnitude);
    }

    //Useful Functions


    public static Stat zero
    {
        get { return new Stat(); }
    }
    public static Stat operator +(Stat a, Stat b)
    {
        var e = new Stat();
        e.STR = a.STR + b.STR;
        e.INT = a.INT + b.INT;
        e.AGI = a.AGI + b.AGI;
        e.WIS = a.WIS + b.WIS;
        e.END = a.END + b.END;
        e.LUC = a.LUC + b.LUC;
        e.OnGainStats += a.OnGainStats + b.OnGainStats;
        e.CriticalHitFlat = a.CriticalHitFlat + b.CriticalHitFlat;
        e.DamageBonus = a.DamageBonus + b.DamageBonus;
        e.DefenseBonus = a.DefenseBonus + b.DefenseBonus;
        
        return e;
    }
    public static Stat operator *(Stat a, int b)
    {
        var e = new Stat();
        e.STR = a.STR * b;
        e.INT = a.INT * b;
        e.AGI = a.AGI * b;
        e.WIS = a.WIS * b;
        e.END = a.END * b;
        e.LUC = a.LUC * b;
        e.CriticalHitFlat = a.CriticalHitFlat * b;
        e.DamageBonus = a.DamageBonus * b;
        e.DefenseBonus = a.DamageBonus * b;
        e.OnGainStats += a.OnGainStats;
        return e;
    }

    public override string ToString()
    {
        return "Stats: \n" + LanguageDao.GetLanguage("statstr", GameManager.language) + " " + STR
            + "\n" + LanguageDao.GetLanguage("statagi", GameManager.language) + " " + AGI
            + "\n" + LanguageDao.GetLanguage("statwis", GameManager.language) + " " + WIS
            + "\n" + LanguageDao.GetLanguage("statend", GameManager.language) + " " + END
            + "\n" + LanguageDao.GetLanguage("statint", GameManager.language) + " " + INT
            + "\n" + LanguageDao.GetLanguage("statluc", GameManager.language) + " " + LUC
            + "\n" + LanguageDao.GetLanguage("statcrit", GameManager.language) + " " + CriticalHitPercentage;
    }
}



