using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Node_Tile
{
    public Vector Pos { get; set; }
    public Vector SubjectV { get; set; }
    public Actor SubjectA { get; set; }
    public bool ActorPresent { get; set; }
    public bool Activated { get; set ; }
    public bool WallPresent { get; set; }    
    public float DistanceToRoot { get; set; }
    public float DistanceToTarget { get; set; }
    public float Score { get; set; }
    
    public Node_Tile(float distancToRoot, float distanceToTarget, Vector pos, Actor subjectA, bool actorPresent, bool wallPresent, bool activated)
    {
        DistanceToRoot = distancToRoot;
        DistanceToTarget = distanceToTarget;
        Score = distanceToTarget + distancToRoot;
        Pos = pos;
        SubjectA = subjectA;
        ActorPresent = actorPresent;
        WallPresent = wallPresent;     
        Activated = activated;
    }
    public Node_Tile(float distancToRoot, float distanceToTarget, Vector pos, Vector subjectV, bool actorPresent, bool wallPresent, bool activated)
    {
        DistanceToRoot = distancToRoot;
        DistanceToTarget = distanceToTarget;
        Score = distanceToTarget + distancToRoot;
        Pos = pos;
        SubjectV = subjectV;
        ActorPresent = actorPresent;
        WallPresent = wallPresent;
        Activated = activated;
    }

    public Node_Tile(Vector pos)
    {
        
        Pos = pos;
        
    }

}

