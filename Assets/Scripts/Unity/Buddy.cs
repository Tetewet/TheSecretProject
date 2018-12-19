using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityEngine;

public class Buddy:MonoBehaviour
    {


    public static void GetRandomBuddy(Vector position, Actor summoner) {
        var buddy = new Player("Mini "+ summoner.Name, new Stat { AGI = 2, END = 1, INT = 6, LUC = 2, STR = 1, WIS = 5 }, true, "Mage")
        { inventory = Actor.Inventory.Light, Class = new Profession(summoner.GetStats/2, Profession.ProfessionType.Mage), Description = "A being from the realm of Idea. It'll figuratively and literally take arms against evil. Dislike doing his taxes." , TilePosition = position };
        GameManager.CurrentBattle.Players.Add(buddy);
        InGameActor buddyInGame = GameManager.GenerateInGameActor(buddy);
        buddyInGame.actor.TilePosition = position;
        GameManager.GM.InGameActors.Add(buddyInGame);
        Debug.Log("Mini " + summoner.Name + " summoned at "+ position);
    }


}



