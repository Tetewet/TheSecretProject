using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityEngine;

public class Buddy:MonoBehaviour
    {


    public static void GetRandomBuddy(Vector position, Actor summoner) {
        var buddy = new Kuku("Slime ", summoner.GetStats / 2, false, "Slime");
        GameManager.CurrentBattle.Players.Add(buddy);
        InGameActor buddyInGame = GameManager.GenerateInGameActor(buddy);
        buddyInGame.actor.TilePosition = position;
        GameManager.GM.InGameActors.Add(buddyInGame);
    }


}



