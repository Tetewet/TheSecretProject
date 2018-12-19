using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Pathfinding
{
    public Vector Flee(Actor me, Actor cible)
    {
        Vector cibleV = new Vector(0, 0);

        if (me.TilePosition.x > cible.TilePosition.x)
            cibleV.x = GameManager.CurrentBattle.map.Tiles.GetLength(0) - 1;
        else cibleV.x = 0;
        
           

        if (me.TilePosition.y > cible.TilePosition.y)
            cibleV.y = GameManager.CurrentBattle.map.Tiles.GetLength(1) - 1;
        else cibleV.y = 0;

        
        return cibleV;
    }

    public List<Node_Tile> GetPath(Actor me, Actor cible, bool flee)
    {

        UnityEngine.Debug.Log("my fault");
        List<Node_Tile> collection = new List<Node_Tile>();
        List<Node_Tile> nodePath = new List<Node_Tile>();
        List<Vector> randomVector = new List<Vector>();
       


        
        Node_Tile Next = new Node_Tile(0, 0, new Vector(0, 0), null, false, false, false); 
        Node_Tile result;
        

        bool actorPresent;
        bool wallPresent;
        bool activated;
        bool terminate;

        float distance_to_root;
        float distance_to_Target;
        float targetVaccinity;
        float lowerCostVacinity;

       
        Vector cibleVector = new Vector(0, 0); ;

        Vector myVector;       
        Vector nextVector = new Vector(0, 0);
        Vector fleeVector = new Vector(0, 0);

        int instance = 0;
      
       
       

        targetVaccinity = 1000;

        // fleing
        if (flee)
               fleeVector= Flee(me, cible);
        //

        for (int X = 0; X < GameManager.CurrentBattle.map.Tiles.GetLength(0); X++)
        {
            for (int Y = 0; Y < GameManager.CurrentBattle.map.Tiles.GetLength(1); Y++)
            {
                actorPresent = false;
                wallPresent = false;
  
                activated = false;

                if (GameManager.CurrentBattle.map.Tiles[X, Y].Actor is Actor)
                {
                    actorPresent = true;
                    if (!flee)
                    {
                        if (GameManager.CurrentBattle.map.Tiles[X, Y].Actor == cible)
                        {
                            cibleVector = GameManager.CurrentBattle.map.Tiles[X, Y].Position;
                            actorPresent = false;
                        }
                    }
                    
                }
               
                
                
                if (GameManager.CurrentBattle.map.Tiles[X, Y].Actor is Wall)
                {
                    wallPresent = true; ;
                }
                
                Vector currentV = new Vector(X, Y);
                if (!wallPresent)
                {
                    if (GameManager.CurrentBattle.map.Tiles[X, Y].Actor == me)
                    {

                        myVector = GameManager.CurrentBattle.map.Tiles[X, Y].Position;
   
                    }
                    else
                    {
                  
                    }
                    distance_to_root = (float)Math.Sqrt(Math.Pow(currentV.x - me.TilePosition.x, 2) + Math.Pow(currentV.y - me.TilePosition.y, 2));
                    if(flee)
                    {
                        
                        distance_to_Target = (float)Math.Sqrt(Math.Pow(currentV.x - fleeVector.x, 2) + Math.Pow(currentV.y - fleeVector.y, 2));
                        collection.Add(new Node_Tile(distance_to_Target, distance_to_root, currentV, fleeVector, actorPresent, wallPresent, activated));
                    }
                    else
                    {
                        distance_to_Target = (float)Math.Sqrt(Math.Pow(currentV.x - cible.TilePosition.x, 2) + Math.Pow(currentV.y - cible.TilePosition.y, 2));
                        collection.Add(new Node_Tile(distance_to_Target, distance_to_root, currentV, cible, actorPresent, wallPresent, activated));
                    }
                   

                }


            }
        }

        

        collection.Sort((a, b) => (a.DistanceToTarget.CompareTo(b.DistanceToTarget)));

        nextVector = new Vector(0, 0);
        nodePath.Clear();
        foreach (var node in collection)
        {
           




            lowerCostVacinity = 1000;
            terminate = false;
            if (instance != 0 & nextVector!= null)
            {
                if (nextVector != new Vector(0, 0))
                {
                    Next = collection.Find(x => x.Pos == nextVector);
                }
                

                if (Next.Pos == cibleVector || Next.ActorPresent)
                {
                   
                    
                    break;
                }
                else
                {
                    nodePath.Add(new Node_Tile(nextVector));
             
                }
               
               
            }
            else
            {
                Next = collection[0];
                nodePath.Add(new Node_Tile(collection[0].Pos));
            }
            Vector bestOfBad = new Vector();
            for (int voidt = 0; voidt < 2; voidt++)
            {

                
                for (float X = Next.Pos.x - 1; X <= Next.Pos.x + 1; X += 1)
                {
                    for (float Y = Next.Pos.y - 1; Y <= Next.Pos.y + 1; Y += 1)
                    {

                        Vector neighboor = new Vector();
                        neighboor.x = X;
                        neighboor.y = Y;
                        // because we dont suppport diag movements
                        if (neighboor != new Vector(Next.Pos.x - 1, Next.Pos.y - 1) & neighboor != new Vector(Next.Pos.x - 1, Next.Pos.y + 1) & neighboor != new Vector(Next.Pos.x + 1, Next.Pos.y - 1) & neighboor != new Vector(Next.Pos.x + 1, Next.Pos.y + 1))
                        {
                            if (collection.Any(item => item.Pos == neighboor))
                            {
                                 result = collection.Find(x => x.Pos == neighboor);
                            }
                            else
                            {
                                continue;
                            }

                            

                            if (!result.ActorPresent & !result.Activated)
                            {
                                if (result.Score < lowerCostVacinity)
                                {
                                    lowerCostVacinity = result.Score;
                                    targetVaccinity = lowerCostVacinity;
                                    bestOfBad = result.Pos;
                                }

                                if (terminate)
                                {
                                    if (result.Score == lowerCostVacinity)
                                    {
                                        targetVaccinity = result.Score;
                                        nextVector = result.Pos;


                                    }


                                    result.Activated = true;

                                }


                            }
                            if (terminate & X==1 & Y==1)
                            {
                                nextVector = bestOfBad;
                            }
                      
                        }

                    }
                } 
               
                terminate = true;
                
            }
           

            instance++;

        }
        //foreach (var item in nodePath)
        //{
        //    UnityEngine.Debug.Log(" node position : " + item.Pos + ", currently targeting : " + cible.Name + ", cible node position : " + cible.TilePosition + ", i am : " + me.TilePosition);
        //}



        return nodePath;

    }
    
}

