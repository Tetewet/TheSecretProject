using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Battle{
    //Console Mode

    public delegate void OnBattleEndHandler();
    public event OnBattleEndHandler BattlEnd, OnTurnEnd;
    public float BattleTime = 0;
    public int GoldEarnedThisBattle = 0;
    public float BattleExp = 0;
    bool Ended = false;
   
    public virtual string Grade
    {
        get
        {
            float pts = 0;
           
            //Logic for the grade of the battle where the damaged receive, damage dealt and time come in place

            if (BattleTime <= 60) pts += 200;
            if (BattleTime <= 30) pts += 200;
            pts += GoldEarnedThisBattle * 1;
            var tot = (History[0].ActorsThisTurn.Length - GameManager.Protags.Count);
            pts += tot * 100;
            if (History.Count <= tot + 1) pts += 500;


            if (pts >= 1000) return "SSS";
            if (pts >= 900) return "S";
            if (pts >= 700) return "A";
            if (pts >= 500) return "B";
            if (pts >= 400) return "C";
            if (pts >= 300) return "D";
            if (pts >= 100) return "E";
            UnityEngine.Debug.Log("BATTLE PTS: " + pts);
            return "F";

           
        }
    
    }
    public Actor ActingThisTurn
    {
        get {
            if (ThisTurn == null ) return null;
            return ThisTurn.Order[0]; }
    }
    public bool IsPlayerTurn
    {
        get {  

            if (Foes.Count > 0)
                if (Foes.Contains(ActingThisTurn)) return false;
            return true;

        }
    }
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
    //Unity Mode 
 
    public Battle (Actor[] Player, Actor[] Foe)
    {
        foreach (var item in Player)
            Players.Add(item);

        foreach (var item in Foe)
            Foes.Add(item);
    }
 

    public Battle(InGameActor[] Player, InGameActor[] Foe)
    {
        foreach (var item in Player)
            Players.Add(item.actor);

        foreach (var item in Foe)
            Foes.Add(item.actor);
    }


    public bool Paused = true;
    public void Proceed()
    {
        ActingThisTurn.Turn(this);
    }
    public void StartNewTurn()
    {
        var e = new Turn(Players, Foes);
        
        History.Add(e);
        var f = "";

      if(!Paused)
        e.Order[0].Turn(this);
        foreach (var item in e.Order )
        {
           f +=  item.ToString() + "\n";
        }
        UnityEngine.Debug.Log(f);
    }
   
    public void EndTurn()
    {
        if (Ended) return;
        if (!OnGoing)
        {
            OnBattleEnd();
            return;
        }

            ThisTurn.Order.Remove(ThisTurn.Order[0]);
 

        if (ThisTurn.Order.Count <= 0)
        {
            if (OnGoing)
                StartNewTurn();
            else
                OnBattleEnd();
            return;
        }
        else
        {
            if (!ActingThisTurn.IsDefeat) ActingThisTurn.Turn(this);
            else
            {
                OnTurnEnd();
                EndTurn();
            }
        }
    


        OnTurnEnd();

    }
    public Map map;
    public bool IsTeamWith(Actor a, Actor b)
    {
        
        return (a != b) && (Players.Contains(a) && Players.Contains(b)) || (Foes.Contains(a) && Foes.Contains(b));
    }
    public void OnBattleEnd()
    {
        if (!Ended)
        {
            BattlEnd();
            Ended = true;
        }
       

        UnityEngine.Debug.Log("Battle ended");
    }
    public bool OnGoing
    {
        get { return Win.IsDone(this) == WinCondition.Result.OnGoing; }
    }
    public Turn ThisTurn
    {

        get {
            if (History.Count == 0) return null;
            return History[History.Count - 1]; }
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
        //Which one is going to play next - for now, two list will do trick. Less bothersome than using an index
        public List<Actor> Order = new List<Actor>();
        public Actor[] ActorsThisTurn;
        public Turn(List<Actor>  TeamA, List<Actor> TeamB)
        {
            var e = new List<Actor>();

            foreach (var item in TeamA) e.Add(item);
            foreach (var item in TeamB) e.Add(item);
 
            e.Sort();

            Order = e;
            ActorsThisTurn = Order.ToArray();

        }
    }
}
 [System.Serializable]
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
                Tiles[x, y] = new Tile();
                Tiles[x, y].Position = new Vector(x, y);
                
            }
        }


    }
    public Tile AtPos(Vector v)
    {
        var a = (int)v.x;
        var b = (int)v.y;
        if (a < 0) a = 0; if (a >= Width) a = Width - 1;
        if (b < 0) b = 0; if (b >= Length) b= Length - 1;
 
        return Tiles[a, b];
       
    }
    public Tile AtPos(int X ,int Y )
    {
        return AtPos(new Vector(X, Y));
    }
    public class  Tile
    {
        public enum ColliderType
        {
            None = 0, Up = 1, Down = 2, Right = 3, Left = 4, All = 5
        }

        public ColliderType collider;
        public delegate void EventsHandler(Tile t);
        public EventsHandler onEnter, onPressed, onExits;
        public Events Event;
        //There can be at any given time one actor or a list of item
        public Actor Actor;
        public List<Item> Items = new List<Item>();
        public Vector Position;
        public int Heigth = 0;
        /// <summary>
        /// Return Null if has no wall or no actor. Return a wall if there is one;
        /// </summary>
        public Wall HasWall
        {
            get { if (Actor == null) return null;
                else if (Actor is Wall) return Actor as Wall;
                else return null;

            }
        }
        public int x
        {
            get { return (int)Position.x; }
        }
        public int y
        {
            get { return (int)Position.y; }
        }
        static bool combatbuffer =false;
        public virtual void Enter(Actor a )
        {
            Actor = a;           
            a.TileWalkedThisTurn++;
            UnityEngine.Debug.Log(a.ToString() + " enter " + Position.ToString());

            if (Items.Count >= 0)           
                for (int i = 0; i < Items.Count; i++)
                {
                    if(Items[i] != null)
                        if (Items[i] is Gold)
                        {
                    
                            GameManager.CurrentBattle.GoldEarnedThisBattle += Items[i].GoldValue;
                            a.Grab(Items[i]);
                        }
                         
                        else  if (!a.inventory.IsFull)
                        {
                            a.Grab(Items[i]);
                           
                            Items.Remove(Items[i]);
                        }
                }


            if (onEnter != null) onEnter(this);
            if(!GameManager.BattleMode)
            {
                if (Event != null && !combatbuffer)
                {

                    if (Event is BattleEvent)
                        combatbuffer = true;


                    Event.Run();

                }
                
            }

              

        }
        public void OnPressed(Actor a)
        {
            if (Event != null) Event.Run();
            if (onPressed != null) onPressed(this);
        }
        public  virtual void OnQuitting()
        {
            if(Actor != null)
            {
               
                Actor.tilecounter++;
                if (Actor.tilecounter >= Actor.GetStats.AGI)
                {
                    Actor.tilecounter = 0;
                    Actor.SP--;
                    
                }

            }
            if(!GameManager.BattleMode) combatbuffer = false;

            if (onExits != null) onExits(this);
            Actor = null;
        }
        public Tile()
        {
            
        }
        public void AddItem(Item a)
        {
            Items.Add(a);
            if(a!= null)
            a.CurrentTile = this;
        }
        public override string ToString()
        {
            var e = Position.ToString();
            e += " Items: ";
            foreach (var x in Items)
            { e += " " + x.ToString() + "\n"; }
            e += " Actor: " + Actor;
            if (collider != ColliderType.None)
                e += " Collider: " + collider;
            return e;
        }
    }

    public override string ToString()
    {
        var e = Name + " | Size : " + Tiles.GetLength(0) + "-" + Tiles.GetLength(1);
        e += "\n";
        for (int x = 0; x <  Width; x++)
        {
            for (int y = 0; y <  Length; y++)
            {

                 e +=  Tiles[x, y].ToString() + "\n"; 

            }

        }
        return e;
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