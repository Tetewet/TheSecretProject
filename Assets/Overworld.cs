using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overworld : Map
{



    public static List<Vector> SpawnPoints = new List<Vector>();
    public Overworld(Vector size) : base(size)
    {

    }


}


public class Events
{
  
    public string Name;
    public Vector ID;
    public Events(Vector id)
    {
        ID = id;
        Name = "Normal Event";
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
        Name = "TEXTBOX" + ID;
    }
    public override void Run()
    {
        GameManager.ShowText(TextToshow);
    }
}
