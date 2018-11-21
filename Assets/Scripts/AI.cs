using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AI
{
    
    public bool player_team;
    public int Nb_Actors;
    public int Nb_Foes;
    public Vector pos_Ac;
    public Vector pos_Foe;
    public Actor current_Ac = GameManager.CurrentBattle.ActingThisTurn;
    private bool dangerClose;
    private bool threaten;
    private int ennemy_Move_Point;
    private int my_Move_Points;
    private int dist_to_foe;
    private int weigth = 0;
    public int target_in_list;
    GameManager GM;
    InGameActor inGame;
    private List<Actor> InGameFoes = new List<Actor>();
    private List<Actor> Ally = new List<Actor>();



    public AI()
    {
        GM = GameManager.GM;
    }

    public void info()
    {

        // bool Numeric_Advantage;

        if (player_team)
        {
            Nb_Actors = Ally.Count;
            Nb_Foes = InGameFoes.Count;

        }
        else
        {
            Nb_Foes = Ally.Count;
            Nb_Actors = InGameFoes.Count;
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

    private Actor TargetName()
    {
        int heaviest = 0;


        for (int q = 0;  q< 2; q++)
        {


            for (int i = 0; i < Nb_Actors; i++)
            {

                if (player_team)
                {
                    ennemy_Move_Point = InGameFoes[i].GetStats.AGI * InGameFoes[i].SpAvaillableThisTurn - InGameFoes[i].TileWalkedThisTurn;
                }
                else
                {
                    ennemy_Move_Point = Ally[i].GetStats.AGI * Ally[i].SpAvaillableThisTurn - Ally[i].TileWalkedThisTurn;
                }
                dist_to_foe = GameManager.EstimathPath(pos_Foe);
                //my move points
                my_Move_Points = current_Ac.GetStats.AGI * current_Ac.SpAvaillableThisTurn - current_Ac.TileWalkedThisTurn;

                pos_Ac = current_Ac.TilePosition;
                pos_Foe = InGameFoes[i].TilePosition;
                // calcul si les points de vie de la cible sont plus bas que la force de l'IA
                if (InGameFoes[i].HP < current_Ac.GetStats.STR)
                {
                    weigth += 10;
                }
                // calcul si au prochain coup la force de l'ennemie peux tuer l'IA
                if (InGameFoes[i].GetStats.STR > current_Ac.HP)
                {
                    threaten = true;
                }
                // if an ennemy is in danger close range (peux se rendre a l'IA et la tuer)
                if (threaten == true && ennemy_Move_Point < dist_to_foe)
                {
                    dangerClose = true;
                }
                // if can get to target
                if (my_Move_Points > dist_to_foe)
                {
                    weigth += 10;

                }
                else
                {
                    weigth -= 10;
                }
                if (heaviest < weigth)
                {
                    heaviest = weigth;
                    target_in_list = i;
                }
            }
        }
        return InGameFoes[target_in_list];




    }
   

    private void Monster_Grunt_Ai()
    {
        info();
        inGame.Attack(TargetName(), Skill.Base);
        //inGame.Attack(InGameFoes[target_in_list], Skill.Base);

        

    }
    private void Monster_Elite_Ai(bool player_team)
    {
        

    }
    private void Monster_Leader_Ai(bool player_team)
    {
       

    }
}
