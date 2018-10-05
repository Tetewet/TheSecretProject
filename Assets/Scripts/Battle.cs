using System.Collections;
using System.Collections.Generic;
 

 
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
    public Map map;
    public bool IsTeamWith(Actor a, Actor b)
    {
        return (Players.Contains(a) && Players.Contains(b)) || (Foes.Contains(a) && Foes.Contains(b));
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
    public List<Actor> Players = new List<Actor>(), Foes = new List<Actor>(); 
    public class Turn
    {

        public List<Actor> Order = new List<Actor>();
        public Turn(List<Actor>  TeamA, List<Actor> TeamB)
        {
            var e = new List<Actor>();

            foreach (var item in TeamA) e.Add(item);
            foreach (var item in TeamB) e.Add(item);
            e.Sort();
            Order = e;

        }
    }
}
 
public class Map  
{
    public string Name = "Arena";
    public int Width
    {
        get { return Tiles.GetLength(0); }
    }
    public int Length
    {
        get { return Tiles.GetLength(1); }
    }
    public Tile[,] Tiles;
    public Map(Vector size)
    {
        Tiles = new Tile[(int)size.x, (int)size.y];
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                Tiles[x, y].Position = new Vector(x, y);
            }
        }


    }
    public Tile AtPos(Vector v)
    {
        Tile g = new Tile();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Length; y++)
            {

                int a = (int)v.x;
                int b = (int)v.y;
               
                
                if (v.y > y) b = y;
                 if (v.x > x) a= x;
                g = Tiles[a, b];
                
            }
        }
        return g;
    }
    public Tile AtPos(int X ,int Y )
    {
        return AtPos(new Vector(X, Y));
    }
    public struct Tile
    {

        //There can be at any given time one actor or a list of item
        public Actor Actor;
        public Item[] item;
        public Vector Position;
        public void Enter(Actor a)
        {
            Actor = a;
        }
        public  void OnQuitting()
        {
            Actor = null;
        }
        
        
    }

    public override string ToString()
    {
        return Name + " | Size : " + Tiles.GetLength(0) + "-" + Tiles.GetLength(1);
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