 using System.Collections;
using System.Collections.Generic;
 

public class Effects {

    public static List<Effects> Core = new List<Effects>()
    {
        new Poison("Poison","poison.png", new Functionality(),Vector.right ,duration:5),
        new Effects("Paralyze","paralyze.png",new Functionality(){CanMove = false}),
        new Burning ("Burning", "burning.png",new Functionality(),Vector.right,EffectToCause:4,chanceToEffect:5),
        new Effects("Soaked","soak.png",new Functionality(),new Stat(){ AGI =-1},duration: 2),
        new Effects("Frenzy","frenzy.png",new Functionality(){IsRational = false},duration:1),
        new Bonus("Critical +","critup",new Functionality(), duration:3, statchange: new Stat(){CriticalHitFlat = 20 })


    };

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
        remainingturn = Duration;
        Curse = incurable;

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

    public override string ToString()
    {
        return base.ToString();
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
    public bool CanUseItems = true, CanMove = true, CanAttack = true, CanUseSkills = true, CanDefend = true, CanReceiveHealing = true, IsRational = true;
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





public struct Element
{
    public static List<Element> Core = new List<Element>
    {
    new Element("Pyrus","fire.png",  new List<int>(){2},-1,5){ID =0 },
    new Element("Aqus","water.png",  new List<int>(){0},-1,5){ID =1 },
    new Element("Viridis","Wire.png",new List<int>(){1},-1,10){ID =2 },
    };  //Contains elements that we are going to used
    public string Name;
    public string imgpath;
    public int FXID;
    int ID; 
    public Element(string Name, string path, List<int> priotylist,int EffectID,float chanceofEffect =0f)
    {
        ChanceOfEffect = chanceofEffect;
        this.Name = Name;
        imgpath = path;
        FXID = EffectID;
        PriorityAgainst = priotylist;
        ID = 0;
        if (!Element.Core.Contains(this))
        {
            ID = Element.Core.Count;
            Element.Core.Add(this);
        }
  
    }
    //on 100%
    public float ChanceOfEffect;

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
 

}