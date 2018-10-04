using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle{

    public void Run()
    {
        while (OnGoing)
        {
            var e = new Turn(Players,Foes);
            History.Add(e);
            foreach (var item in e.Order) item.Turn(this);
       }
        OnBattleEnd();
    }

    public void OnBattleEnd()
    {

    }
    public bool OnGoing
    {
        get { return Win.IsDone(this) == WinCondition.Result.OnGoing; }
    }
    public Turn ThisTurn
    {

        get { return History[History.Count - 1]; }
    }
    public Turn LastTurn
    {

        get {
            if (History.Count <= 1) return null;
            return History[History.Count - 2]; }
    }
    public List<Turn> History = new List<Turn>();
    public WinCondition Win = WinCondition.Default;
    public Actor[] Players, Foes; 
    public class Turn
    {

        public List<Actor> Order = new List<Actor>();
        public Turn(Actor[] TeamA, Actor[] TeamB)
        {
            var e = new List<Actor>();

            foreach (var item in TeamA) e.Add(item);
            foreach (var item in TeamB) e.Add(item);
            e.Sort();
            Order = e;

        }
    }
}

public class WinCondition
{
    //In case we want to be specific - e.g One actor left
    public enum Result
    {
        Win,
        Lose,
        OnGoing
    }
    public virtual Result IsDone(Battle b)
    {

        bool PlayerStillAlive = false;
        bool FoeStillAlive = false;

        foreach (var item in b.Players) if (!item.IsDefeat) PlayerStillAlive = true;
        foreach (var item in b.Foes) if (!item.IsDefeat) FoeStillAlive = true;
        if (FoeStillAlive && PlayerStillAlive) return Result.OnGoing;
        else if (!PlayerStillAlive) return Result.Lose;
        else return Result.Win;

    }
    public static WinCondition Default
    {
        get { var e = new WinCondition();           
            return e;
        }
    }
}