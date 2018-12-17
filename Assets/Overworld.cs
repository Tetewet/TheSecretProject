using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overworld : Map
{


    public static Vector PlayerPos;
    public static List<Vector> SpawnPoints = new List<Vector>();
    public Overworld(Vector size) : base(size)
    {

    }


}


public class Events : IUniversalID
{
  
    public string Name;
    public Vector VID;
    public Events(Vector id)
    {
        VID = id;
        Name = "Normal Event";
        ID = GameManager.GenerateID(this);
    }
    private readonly string ID;
    public string GetID()
    {
        return ID;
    }

    public virtual void Run()
    {

        //Scripts here


    }

}
public class TextBox : Events
{
    public string TextToshow;
    public bool RequiredComfirmation = true;
    public TextBox(Vector id,string Tts):base(id) 
    {
        this.TextToshow = Tts;
        Name = "TEXTBOX" + VID;
    }
    public override void Run()
    {
        GameManager.ShowText(TextToshow);
    }
}

public class BattleEvent : Events
{
     
    public BattleEvent(Vector id, Actor[] act, float luck) : base(id)
    {
        belligerent = act;
        Chance = luck;
        Name = Chance * 100 + "% Battle Event ";
    }
     float Chance = .15f;
    Actor[] belligerent;
    public override void Run()
    {
        if(Chance < Random.Range(0, 1f))      
        GameManager.OverworldStartBattle(belligerent, new Map(new Vector(8, 8)));
     
    }
}
