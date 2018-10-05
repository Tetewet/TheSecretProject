using System.Collections;
using System.Collections.Generic;
 

public class Player : Actor
{
    public Player(string Name, stat BaseStats) : base(Name, BaseStats)
    {
        this.Name = Name;
        this.baseStats = BaseStats;
    }

    
}
