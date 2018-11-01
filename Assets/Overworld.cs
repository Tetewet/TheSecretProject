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
    public virtual void Run()
    {

        //Scripts here


    }




}
