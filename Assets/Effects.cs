 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects:IUniversalID {

    public static List<Effects> Core = new List<Effects>()
    {
        new Poison("null","null", new Functionality(),Vector.right ,duration:5),//0
        new Poison("Poison","null", new Functionality(),Vector.right ,duration:5),//1
        new Effects("Paralyze","null",new Functionality(){CanMove = false}),//2
        new Burning ("Burning", "null",new Functionality(),Vector.right,EffectToCause:4,chanceToEffect:5),//3
        new Effects("Soaked","null",new Functionality(),new Stat(){ AGI =-1},duration: 2),//4
        new Effects("Frenzy","null",new Functionality(){IsRational = false},duration:1),//5
        new Bonus("Critical +","critup",new Functionality(), duration:3, statchange: new Stat(){CriticalHitFlat = 20}),//6
        new Poison("HealingPerTurn","null", new Functionality(),Vector.left*2 ,duration:5),//7
        new InstaHealing("Healing","null", new Functionality(),Vector3.right*6),//8
        new InstaHealing("MPBoost","null", new Functionality(),Vector3.up*6),//9
        new InstaHealing("SPBoost","null", new Functionality(),Vector3.forward*3),//10
        new Bonus("Spawn", "null",new Functionality(){ IsSpawner = true }, statchange: new Stat(){  }),//11
        new Bonus("STR +","null",new Functionality(), duration:3, statchange: new Stat(){STR = 5 }),//12
        new Bonus("AGI +","null",new Functionality(), duration:3, statchange: new Stat(){AGI = 5 }),//13//
        new Bonus("END +","null",new Functionality(), duration:3, statchange: new Stat(){END = 5 }),//14
        new Bonus("WIS +","null",new Functionality(), duration:3, statchange: new Stat(){WIS = 5 }),//15
        new Bonus("INT +","null",new Functionality(), duration:3, statchange: new Stat(){INT = 5 }),//16//
        new Bonus("LUC +","null",new Functionality(), duration:3, statchange: new Stat(){LUC = 5 }),//17
         new Bonus("STR -","null",new Functionality(), duration:3, statchange: new Stat(){STR = -5 }),//18
         new Bonus("AGI -","null",new Functionality(), duration:3, statchange: new Stat(){AGI = -5 })//19
       
       

    };
    /// <summary>
    /// ShortHand for Core[i]
    /// </summary>
    /// <param name="f"></param>
    public static implicit operator  Effects(int f)   
    {
        if (f <= 0) return null;
   
        return Core[f];
    } 
    



    public string Name = "fx";
    public string Description = "emptied fx";
    public Stat StatChange;
    public string imgpath = "fx.png";
    public Functionality Func;
    public readonly int Duration = 3;
    
    public bool IsNegative = false;
    public bool Curse = false;
    //For exemple, if we want to kill something if their hp < 0 or give them bonus based on whom they are 
    internal Actor _actor;
    internal int remainingturn = 1;
    public Effects(string name, string Path, Functionality fun, Stat statchange = new Stat(), int duration = 3,bool incurable = false)
    {
        Name = name;
        imgpath = Path;
        Func = fun;
        StatChange = statchange;
        Duration = duration;
        remainingturn = Duration;
        Curse = incurable;
        ID = GameManager.GenerateID(this);

    }
    public virtual void OnTurn(Battle.Turn t)
    {
        if ((_actor != null && remainingturn <= 0) && !Curse)
        {
            _actor.Remove(this);
            return;
        }
            remainingturn--;
        
        ///Do shit there
    }
    public virtual void OnBeingHit(float x, Skill f)
    {

    }
    public virtual void OnAttack(float x, Skill f)
    {

    }
    public virtual void OnInstant(ref float HP, ref float MP,ref int SP) {


    }
    public override string ToString()
    {
        return base.ToString();
    }
    private readonly string ID;
    public string GetID()
    {
        return ID;
    }
}


public class Poison : Effects
{
    //x is HP, y is MP
    public Vector DamageEachTurn =  Vector.right;
    public Poison(string name, string Path, Functionality fun, Vector DamageEachTurn, Stat statchange = default(Stat), int duration = 3, bool incurable = false) : base(name, Path, fun, statchange, duration, incurable)
    {
        this.DamageEachTurn = DamageEachTurn;  
        
    }

    public override void OnTurn(Battle.Turn t)
    {
       
        base.OnTurn(t);
        _actor.HP -= DamageEachTurn.x;
        _actor.MP -= DamageEachTurn.y;
    }

}
public class InstaHealing : Effects
{
    //x is HP, y is MP
    public Vector3 constVector = Vector3.right;
    public InstaHealing(string name, string Path, Functionality fun, Vector3 constVector, Stat statchange = default(Stat), int duration = 0, bool incurable = false) : base(name, Path, fun, statchange, duration, incurable)
    {
        this.constVector = constVector;
    }
    
    public override void OnInstant(ref float HP,ref float MP,ref int SP)
    {

        
        HP += (int)constVector.x;
        MP += (int)constVector.y;
        SP += (int)constVector.z;
    }

}
public class Burning : Poison
{
    public int FXID = 0;
    public float ChanceToCauseEffect = 5;
    public Burning(string name, string Path, Functionality fun, Vector DamageEachTurn,int EffectToCause,float chanceToEffect , Stat statchange = default(Stat), int duration = 3, bool incurable = false) : base(name, Path, fun, DamageEachTurn, statchange, duration, incurable)
    {
        ChanceToCauseEffect = chanceToEffect;
        FXID = EffectToCause;
    }
    public override void OnTurn(Battle.Turn t)
    {
        base.OnTurn(t);
        if(UnityEngine.Random.Range(0, 100f) < ChanceToCauseEffect){
            if (FXID < Effects.Core.Count)
                _actor.Apply(Core[FXID]);
            else UnityEngine.Debug.Log("FXID: " + FXID + " is not valid. Max Effect in Effect.Core is " + Effects.Core.Count + ". Consider making it lower than such number.");
        }
    }


}

public class Bonus : Effects
{
    public Bonus(string name, string Path, Functionality fun, Stat statchange = default(Stat), int duration = 3, bool incurable = false) : base(name, Path, fun, statchange, duration, incurable)
    {
        IsNegative = false;
    }
}

public class Functionality
{
    public bool CanUseItems = true, CanMove = true, CanAttack = true, CanUseSkills = true, CanDefend = true, CanReceiveHealing = true, IsRational = true, IsSpawner = false;
    public  Functionality()
    {

    }
    public static Functionality normal
    {
        get
        {
            var e = new Functionality();
             e.CanMove = e.CanDefend = e.CanAttack = e.CanReceiveHealing = e.CanUseItems = e.CanUseSkills = true;
            return e;
        }
    }
    public static Functionality operator * (Functionality a, Functionality b)
    {
        var e = new Functionality();
        e.CanAttack = b.CanAttack && a.CanAttack;
        e.CanDefend = b.CanDefend && a.CanDefend;
        e.CanMove = b.CanMove && a.CanMove;
        e.CanReceiveHealing = b.CanReceiveHealing && a.CanReceiveHealing;
        e.CanUseItems = b.CanUseItems && a.CanUseItems;
        e.CanUseSkills = b.CanUseSkills && a.CanUseSkills;
        e.IsRational = b.IsRational && a.IsRational;
        return e;
    }
}

public struct Element : IUniversalID
{
    public static List<Element> Core = new List<Element>
    {
    new Element("null","null",  new List<int>(){0},0,0){ID =0 },
    new Element("Pyrus","fire",  new List<int>(){3},3,5){ID =0 },
    new Element("Aqus","water",  new List<int>(){1},4,5){ID =1 },
    new Element("Viridis","wind",new List<int>(){2},-1,10){ID =2 },
    };  //Contains elements that we are going to use
    public string Name;
    public string imgpath;
    public int FXID;
    internal int ID; 
    public Element(string Name, string path, List<int> priotylist,int EffectID,float chanceofEffect =0f)
    {
        ChanceOfEffect = chanceofEffect;
        this.Name = Name;
        imgpath = path;
        FXID = EffectID;
        PriorityAgainst = priotylist;
        ID = 0;
        UID = "";
        UID = GameManager.GenerateID(this);
  
    }
    //on 100%
    public float ChanceOfEffect;
    public static implicit operator Element(int f)   
    {
        if (f <= 0) return None;
        return Core[f];
    }
    public static Element None
    {
        get
        {
            var e = new Element();
            e.ID = -1;
            return e;
        }
    }
    public List<int> PriorityAgainst;
    public float EfficacyFactor(int e, Actor a)
    {
        var x = 1f;
        if (PriorityAgainst.Contains(e)) x = 2f;
        else if (Core[e].PriorityAgainst.Contains(this.ID)) x = .5f;
        if (UnityEngine.Random.Range(0, 100f) < ChanceOfEffect)
        {
            if (FXID < Effects.Core.Count)
                a.Apply( Effects.Core[FXID]);
            else UnityEngine.Debug.Log("FXID: " + FXID + " is not valid. Max Effect in Core.Effect is " + Effects.Core.Count + ". Consider making it lower than such number.");
        }

           

        return x;
    }
    private readonly string UID;
    public string GetID()
    {
        return UID;
    }
}

public class Resistance
{
    //ID OF ELEMENTS
    public List<int> resistance = new List<int>();
    public List<int> weakness = new List<int>();



    public   static Resistance operator +(Resistance x, Resistance y)
    {
        var g = new Resistance();
        g.resistance = new List<int>();
        g.weakness = new List<int>();
        foreach (var item in x.resistance)
            g.resistance.Add(item);
        foreach (var item in y.resistance)
            g.resistance.Add(item);
        foreach (var item in y.weakness)
            g.weakness.Add(item);
        foreach (var item in x.weakness)
            g.weakness.Add(item);
        return g;
    }
}