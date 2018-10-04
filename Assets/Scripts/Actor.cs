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
        return (a - b).magnitude;
    }
    public float magnitude { get { return (x + y) / 2; } }
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
    public override string ToString()
    {
        return "{" + x + " , " + y + " }";
    }
}

public abstract class Actor : IComparable<Actor> {


    public const float BASEEXP = 100;
    public string Name;

    //Level
    public int GetLevel
    {
        get { return Level; }
    }
    private int Level = 0;
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
    public Actor(string Name)
    {
        this.Name = Name;
    }

    //Status
    protected float HP = 10;
    protected float MP = 10;
    protected int SP = 10;
    public void Heal()
    {
        HP = GetStats.MaximumHP;
        MP = GetStats.MaximumMP;
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
    public delegate void DamageHandler(float z,Skill x, DamageType d);
    public event DamageHandler OnAttack, OnDamage;
    public virtual void Use(Skill s, Actor[] Target)
    {
        if (HP >= s.NeedHp) TakeDamage(s.NeedHp);
        else return;

        if (MP >= s.NeedMP) ConsumeMP(s.NeedMP);
        else return;

        if (SP >= s.NeedSP) ConsumeSP(s.NeedSP);
        else return;

        foreach (var item in Target) s.Activate(item, GetStats);

    }
    public void TakeDamage(float x)
    {
        TakeDamage(x, null, DamageType.None);
    }
    public virtual void TakeDamage(float x, Skill f, DamageType d)
    {
   
        if (d == DamageType.Magical) x -= baseStats.MagDEF;
        else if (d == DamageType.Physical) x -= baseStats.PhysDEF;
        HP -= x;
        if (OnDamage != null) OnDamage(x,f,d);
    }

    public void ConsumeMP(float x )
    {
        MP -= x;
    }
    public void ConsumeSP(int x)
    {
        SP -= x;
    }


    //Act for one turn. Must be in a battle
    public virtual void Turn(Battle battle)
    {
        //This turn

        Battle.Turn ThisTurn = battle.ThisTurn;
        //Act Here - Add logic for monster here             
        
    }
 
    //Position In world;
    public void WalkTo(Vector position)
    {

    }

    internal Transform _transform;
    /// <summary>
    /// World-Related Variables (Position, Direction )
    /// </summary>
    public Transform transform
    {
        get{ return transform; }
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
    //What percentage of the stats it uses; 1 = 100%, .2 = 20% of STR or INT
    public float Damage  = 0;
    protected float BaseCritChance = 5;
    public TargetType Targets;

    //Requirement

        
    public float NeedMP =0, NeedHp = 0;
    public int NeedSP = 0;
    public static Skill Base {

        get
        {
            var e = new Skill();
            e.Name = "Attack";
            e.Type = DamageType.Physical;
            e.Damage = 5;
            e.Targets = TargetType.OneEnemy;
            return e;
        }
    }

    public virtual void Activate(Actor Target, stat Stats = new stat())
    {
        var x = Damage;
        if (Type == DamageType.Magical) x *= Stats.INT;
        else if (Type == DamageType.Physical) x *= Stats.STR;

        if ((Stats.LUC * 2 + BaseCritChance) > SkillRandom.Next(0, 100)) Damage *= 1.50f;

            Target.TakeDamage(x,this, Type);
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
public struct stat :IComparable<stat>
{

    public delegate void OnStatsGainHandler( );
    public event OnStatsGainHandler OnGainStats;
    public int STR, AGI, END, WIS, INT, LUC;
    //For fine tuning turn order
    public int Priority ;
    //For fine tuning targeting
    public int Threat;
    public float PhysDEF { get { return STR / 2 + END + AGI / 4; } }
    public float MagDEF { get { return INT + WIS * 2 ; } }
    public float MaximumHP { get { return END * 5; } }
    public float MaximumMP { get { return WIS * 5; } }
 
    public int Magnitude{ get { return (STR + AGI + END + WIS + INT + LUC)/6; } }
  
    public void AddStats(int x , _stats s)
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
