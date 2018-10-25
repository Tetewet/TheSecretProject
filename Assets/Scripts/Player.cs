using System.Collections;
using System.Collections.Generic;
 

public class Player : Actor
{
    public Player(string Name, stat BaseStats, bool Controllable, string AnimatorP) : base(Name, BaseStats, Controllable, AnimatorP)
    {
        this.Name = Name;
        this.baseStats = BaseStats;
        this.Controllable = Controllable;
    }

    
}


public class Monster : Actor
{
    public float ExpGain = 5;
    public Monster(string Name, stat BaseStats, bool Controllable,string AnimatorP) : base(Name, BaseStats, Controllable, AnimatorP)
    {
        
    }

    public override void Ondeath(float x, Skill f,  Actor a = null)
    {
       
        if (a != null)
        {
            a.AddExp(ExpGain);
            a.OnMurder(this);
            foreach (var item in inventory.items)
            { 

                if (item != null)
                    GameManager.CreateNewItemOnField(item, TilePosition);
             
            }

            if(Gold > 0)
            {
                var h = GameManager.CreateNewItemOnField(Item.Gold, TilePosition);
                h.item.GoldValue = Gold;
            }
             
        }
        base.Ondeath(x, f, a);



    }
}