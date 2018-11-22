 using System.Collections;
using System.Collections.Generic;
 

public class Effects {

    public string Name = "fx";
    public string Description = "emptied fx";
    public Stat StatChange;
    public string imgpath = "fx.png";
    public Functionality Func;
    public readonly int Duration = 3;
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

public class Functionality
{
    public bool CanUseItems = true, CanMove = true, CanAttack = true, CanUseSkills = true, CanDefend = true, CanReceiveHealing = true;
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
        return e;
    }
}
