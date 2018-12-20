using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AI
{    
    private int Nb_Actors;
    private int Nb_Foes;
    private int ennemy_Move_Point;
    private int my_Move_Points;
    private int dist_to_foe;
    private int weigth;

    public Vector pos_Ac;
    public Vector pos_Foe;
    public Actor current_Ac;

    private bool dangerClose;
    private bool threaten;
    private bool inRange;
    public bool flee { get; private set; }

    public string whatToSay;

    GameManager GM;
    public InGameActor inGame;
    
    private List<AIInfoClass> collectionChief = new List<AIInfoClass>();
    private List<AIInfoClass> collectionDumb = new List<AIInfoClass>();
    private List<Node_Tile> ChildnodesList = new List<Node_Tile>();
    private List<List<Node_Tile>> ParentnodesList = new List<List<Node_Tile>>();
    private List<Actor> InGameFoes = new List<Actor>();
    private List<Actor> Ally = new List<Actor>();
    private List<Node_Tile> node_Tile;

    public AI()
    {
        GM = GameManager.GM;
    }

    public void info()
    {

        current_Ac = GameManager.CurrentBattle.ActingThisTurn;

        if (current_Ac.IsInPlayerTeam)
        {
            Ally.Clear();
            foreach (var item in GameManager.Protags)
            {
                Ally.Add(item);
            }
            InGameFoes.Clear();
            foreach (var item in GM.InGameFoes)
            {
                InGameFoes.Add(item.actor);
            }
            Nb_Actors = Ally.Count;
            Nb_Foes = InGameFoes.Count;
         
        }
        else
        {
            Ally.Clear();
            foreach (var item in GM.InGameFoes)
            {
                Ally.Add(item.actor);
            }
            InGameFoes.Clear();
            foreach (var item in GameManager.Protags)
            {
                InGameFoes.Add(item);
            }
            Nb_Actors = Ally.Count;
            Nb_Foes = InGameFoes.Count;
            
        
        }
      
    }

    private bool Numeric_Advantage()
    {
        if (Nb_Actors > Nb_Foes)
        {
           return true;
        }
        else
        {
            return false;
        }
    }

    private void TargetName()
    {
        info();
        flee = false;
                
        for (int i = 0; i < Nb_Foes ; i++)
        {
            weigth = 0;

            //ennemy move points
            ennemy_Move_Point = InGameFoes[i].GetStats.AGI * InGameFoes[i].SpAvaillableThisTurn - InGameFoes[i].TileWalkedThisTurn;

            //my move points
            my_Move_Points = current_Ac.GetStats.AGI * current_Ac.SpAvaillableThisTurn - current_Ac.TileWalkedThisTurn;
     
            ///// path finding
            Pathfinding path = new Pathfinding();
            ChildnodesList = path.GetPath(current_Ac, InGameFoes[i], flee);
         

            dist_to_foe = ChildnodesList.Count;

            ParentnodesList.Add(ChildnodesList);

            ////////////////
            // if can get to target
            if (my_Move_Points > dist_to_foe)
            {
                
                inRange = true;
            }
            else
            {
                inRange = false;
            }
            // calcul si au prochain coup la force de l'ennemie peux tuer l'IA
            if (InGameFoes[i].GetStats.STR > (current_Ac.HP*3))
            {
                
                threaten = true;
               
            }
            // if an ennemy is in danger close range (peux se rendre a l'IA et la tuer)
            if (threaten == true & ennemy_Move_Point > dist_to_foe)
            {
                dangerClose = true;
                
                if (InGameFoes[i].HP < current_Ac.GetStats.STR)
                {
                    weigth += 50;
                }
                else
                {
                   
                    flee = true;
                    ChildnodesList = path.GetPath(current_Ac, InGameFoes[i], flee);
                    
                    weigth = -50;
                }
            }
           
            
            if (InGameFoes[i].HP > current_Ac.GetStats.STR)
            {
                
                if (inRange)
                {
                   
                    
                    
                    weigth += 20;
                }
            }


            //some debug
           // UnityEngine.Debug.Log("i am : " + current_Ac + ", my hp is : " + current_Ac.HP + ", my strenght : " + current_Ac.GetStats.STR + ",  his name is : " + InGameFoes[i].Name + ", his hp is : " + InGameFoes[i].HP + ", enemy strenght: " + InGameFoes[i].GetStats.STR);
            //if flee only return 1 list;
            if (flee)
            { 
                dangerClose = true;
                collectionChief.Clear(); collectionDumb.Clear();
                collectionChief.Add(new AIInfoClass(current_Ac.TilePosition, ParentnodesList, InGameFoes[i], weigth, dist_to_foe, inRange, dangerClose, my_Move_Points));
                collectionDumb.Add((new AIInfoClass(ChildnodesList, InGameFoes[i], weigth, dist_to_foe, dangerClose, my_Move_Points)));
                return;
            }

            collectionChief.Add(new AIInfoClass(current_Ac.TilePosition, ParentnodesList, InGameFoes[i], weigth, dist_to_foe, inRange, dangerClose, my_Move_Points));

            collectionDumb.Add((new AIInfoClass(ChildnodesList, InGameFoes[i], weigth, dist_to_foe, dangerClose, my_Move_Points)));
        }

    }

   
    




    public List<AIInfoClass> BonusPoints(List<AIInfoClass> collection)
    {
        collectionDumb.Sort((a, b) => (a.Distancefoe.CompareTo(b.Distancefoe)));
        collectionDumb.Reverse();
        int bonus = 20;
        int iteration = collection.Count;
        foreach (var item in collection)
        {

            bonus = ((bonus / collection.Count) * iteration);
            item.Weight += bonus;
            iteration--;

        }
        return collection;
    }

    public List<AIInfoClass> Monster_Grunt_Ai()
    {
       
        TargetName();


        
       
        
        if (!flee)
        {
            collectionDumb = BonusPoints(collectionDumb);
            collectionDumb.Sort((a, b) => (a.Weight.CompareTo(b.Weight)));

        }

        AIUIDialogues aIUIDialogues = new AIUIDialogues();
        aIUIDialogues.AIUIDialogueGrunt(collectionDumb);

        return collectionDumb;

       


    }
    private void Monster_Elite_Ai()
    {
        

    }
    private void Monster_Leader_Ai()
    {
       

    }
}
