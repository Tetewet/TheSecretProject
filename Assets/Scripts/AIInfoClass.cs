using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AIInfoClass
{
    public Vector Pos { get; set; }
    public List<List<Node_Tile>> Nodes { get; set; }
    public List<Node_Tile> NodesS { get; set; }
    public Actor Target { get; set; }
    public int Weight { get; set; }
    public int MyMovePoints { get; set; }
    public float Distancefoe { get; set; }
    public bool InRange { get; set; }
    public bool Threathened { get; set; }
    
    // leader public  leader { get; set; } future

    public AIInfoClass(Vector pos, List<List<Node_Tile>> nodes, Actor target, int weight, float distanceFoe, bool inRange, bool threathened, int myMovePoints)
    {
        Pos = pos;
        Nodes = nodes;
        Target = target;
        Weight = weight;
        Distancefoe = distanceFoe;
        InRange = inRange;
        Threathened = threathened;
        MyMovePoints = myMovePoints;
    }
    public AIInfoClass(List<Node_Tile> nodesS, Actor target, int weight, float distanceFoe, bool threathened, int myMovePoints)
    {
       
        NodesS = nodesS;
        Target = target;
        Weight = weight;
        Distancefoe = distanceFoe;
        Threathened = threathened;
        MyMovePoints = myMovePoints;
        
    }
}

