using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager GM;
    public static bool InBattleMode = true;
    public Camera Cam;
    public static Vector3 VecTo3(Vector v)
    {
        return new Vector3(v.x, v.y, 0);
    }
    public static BattleTile[,] Battlefied;

    Image[] Inventory;
    public GridLayoutGroup grid, CharacterInventory;
    public Color GridColor;
    [Header("Templates")]


    public Text SpCostUI;
    public GameObject ActorPrefab;
    public GameObject ItemPrefab;
    public GameObject panel, InventoryCeil, GameEnd;
    public GameObject TabButtons, MiniMenu;
    public Text OnHover; 
    public Animator Cursor;
    public static Vector CursorPos;
    public Vector3 CursorOffset;
    public static Actor SelectedActor;
    public static Actor ActorAtCursor = null;
    
    public static InGameItem CreateNewItemOnField(Item i, Vector Position)
    {
        var e = Instantiate(GM.ItemPrefab, Vector3.zero, Quaternion.identity).GetComponent<InGameItem>();

        e.item = i;
        CurrentBattle.map.AtPos(Position).AddItem(e.item);
        return e;
    }

    public static Battle CurrentBattle;
 
    public string MapName;

    public InGameActor[] Actors;
    public InGameActor[] Foes;

    
    public static InGameActor GetInGameFromActor(Actor a)
    {
        foreach (var item in GM.Actors)
            if (item.actor == a) { return item;  }
        foreach (var item in GM.Foes)
            if (item.actor == a) { return item;   }
        return null;
    }
    void InitializeUI()
    {
        Inventory = new Image[100];
        for (int i = 0; i < Inventory.Length; i++)
            Inventory[i] = Instantiate(InventoryCeil, CharacterInventory.transform).GetComponentInChildren<Image>();


    }
    private void Awake()
    {
        if (!GM) GM = this;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
 
    }
    public void Start()
    {
        CurrentBattle = new Battle(Actors, Foes);
        CurrentBattle.BattlEnd += OnBattleEnd;
        CurrentBattle.StartNewTurn();
        CurrentBattle.OnTurnEnd += OnTurnEnd;
        //14 6
        CurrentBattle.map = new Map(new Vector(18, 9));
        InitializeUI();
        GenerateMap(CurrentBattle.map);
        OnHover.enabled = true;
        foreach (var item in Foes)      
            item.actor.Controllable = false;
        foreach (var item in Actors)
            item.actor.Controllable = true;

     
        //Debug
        for (int i = 0; i < Actors.Length; i++)
        {
            Actors[i].actor.Teleport(CurrentBattle.map.AtPos(8+ i, 3));
        }
        for (int i = 0; i < Foes.Length; i++)
        {
           Foes[i].actor.Teleport(CurrentBattle.map.AtPos(12 + i, 3));
        }
        CursorPos = new Vector(9, 4);
        CreateNewItemOnField(new Consumeable("SpPotion", "Items/SP_POTION") { rarity = Item.Rarity.Common, GoldValue = 10, Uses = 1, SPregen = 3 }, new Vector(5,5));
        ToggleGrid();

        foreach (var item in Actors) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right * 2; ;
        foreach (var item in Foes) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right * 2; ;

    }

    private void OnTurnEnd()
    {
        ResetGrid();
    }

    private void OnPathCleared()
    {
        ResetGrid();
    }
    private void OnBattleEnd()
    {
        GameEnd.SetActive(true);
    }

    float timer = 0;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
    }


    public static List<Vector> PathUI = new List<Vector>( );
    public static void EstimathPath( Vector where)
    {


        var ThisTurnPlayer = CurrentBattle.ThisTurn.Order[0];
        GM.SpCostUI.enabled = SelectedActor != null;
        if (SelectedActor == null && CurrentBattle.ThisTurn.Order[0] == null )
        {
            GM.ResetGrid();
            return;
        }
        
        PathUI.Clear();
        // if (ThisTurnPlayer == SelectedActor && ThisTurnPlayer.Path.Count == 1 && ThisTurnPlayer.Path.Peek() == SelectedActor.TilePosition) GM.ResetGrid();


        if (ThisTurnPlayer != null && ThisTurnPlayer.Path.Count > 1 )
        {
      
            for (int h = 0; h < Battlefied.GetLength(0); h++)
                for (int j = 0; j < Battlefied.GetLength(1); j++)
                    foreach (var ff in Battlefied[h, j].Sprite)               
                        ff.enabled = ThisTurnPlayer.Path.Contains(Battlefied[h, j].tile.Position);
            return;
        }
        if (SelectedActor == null  && SelectedActor != ThisTurnPlayer   )

        { GM.ResetGrid(); return; }

        int x = (int)(where.x - SelectedActor.TilePosition.x);
        int y = (int)(where.y - SelectedActor.TilePosition.y);
        var a = 1;
        var b = 1;
        if (x < 0) a = -1;
        if (y < 0) b = -1;

        var fs = SelectedActor.TileWalkedThisTurn  -1;
        
        var maximumtile =   (SelectedActor.GetStats.AGI * SelectedActor.SP   ) - fs  ;

        var xc = PathUI.Count <= maximumtile;

        if (SelectedActor.GetStats.AGI * SelectedActor.SP < SelectedActor.TileWalkedThisTurn && SelectedActor.SP > 0)
            maximumtile++;

        var e = (int)(PathUI.Count / SelectedActor.GetStats.AGI);
        if (Mathf.Abs(x) > Mathf.Abs(y) || CurrentBattle.map.AtPos(SelectedActor.TilePosition + Vector.up * b).Actor != null)
        {
            //for (int i = 1; i <= Mathf.Abs(x) && PathUI.Count <   maximumtile; i++)
            var lastxpos = SelectedActor.TilePosition + Vector.right * x;

            for (int i = 1; i <= Mathf.Abs(x) && xc; i++)
            {

                if (PathUI.Count >= maximumtile)
                {
                    lastxpos = SelectedActor.TilePosition + Vector.right * i * a;
                    break;
                }
                PathUI.Add(SelectedActor.TilePosition + Vector.right * i * a);
            }
          //  if (PathUI.Count < maximumtile)

                for (int i = 1; i <= Mathf.Abs(y); i++)
                {
                    PathUI.Add(lastxpos + Vector.up * i * b);
                    //if (PathUI.Count > maximumtile) break;
                }

        }
        else
        {
            var lastypos = SelectedActor.TilePosition + Vector.up * y;

            for (int i = 1; i <= Mathf.Abs(y) && xc; i++)
            {
                if (PathUI.Count >= maximumtile)
                {
                    lastypos = SelectedActor.TilePosition + Vector.up * i * b;
                    break;
                }
                PathUI.Add(SelectedActor.TilePosition + Vector.up * i * b);

            }
           // if (PathUI.Count < maximumtile)
                for (int i = 1; i <= Mathf.Abs(x); i++)
                {

                    PathUI.Add(lastypos + Vector.right * i * a);
                
                }
        }
       
        if(PathUI.Count > maximumtile && PathUI.Count > 0)
        while (PathUI.Count > maximumtile)
        {
                if (PathUI.Count < 0) break;
                    PathUI.RemoveAt(PathUI.Count-1);
                
        }
        GM.SpCostUI.text = ((int)(PathUI.Count / SelectedActor.GetStats.AGI)).ToString("00") + " sp" ;

        for (int h = 0; h < Battlefied.GetLength(0); h++)
            for (int j = 0; j < Battlefied.GetLength(1); j++)
            {
               
               
               
                foreach (var ff in Battlefied[h, j].Sprite)
                      ff.enabled = PathUI.Contains(Battlefied[h, j].tile.Position);
                    
               

            }
             

    }

    public void OnCursorEnter(Map.Tile t)
    {
   
    }
    public void OnCursorExit(Map.Tile t)
    {

   

    }
   
    public void OnCursorUpdate(Map.Tile t)
    {

        OnHover.gameObject.SetActive(ActorAtCursor != null);
        CharacterInventory.gameObject.SetActive(ActorAtCursor != null);
        Cursor.SetBool("Hover", ActorAtCursor != null);
        ActorAtCursor = t.Actor;
        if (ActorAtCursor != null)
        {

            if (ActorAtCursor != SelectedActor) ChangeGridColor(Color.red);

            OnHover.text =
     "* " + ActorAtCursor.Name + " *\n\nlvl"
    + ActorAtCursor.GetLevel.ToString("00") + "\n[ hp  "
    + ActorAtCursor.HP.ToString("00") + " ]\n[ mp "
    + ActorAtCursor.MP.ToString("00") + " ]\n[ sp  "
    + ActorAtCursor.SP.ToString("00") + " ]";

            for (int i = 0; i < 100; i++)
            {
                if (i < ActorAtCursor.inventory.items.Length)
                {

                    if (ActorAtCursor.inventory.items[i] != null)
                    {
                        Inventory[i].transform.parent.gameObject.SetActive(true);
                        Inventory[i].enabled = true;

                        if (Inventory[i].sprite == null)
                        {
                            Inventory[i].sprite = LoadSprite(ActorAtCursor.inventory.items[i].ResourcePath);
                        }
                    }
                    else
                    {
                        Inventory[i].sprite = null;
                        Inventory[i].enabled = false;
                    }

                }
                else
                {
                    Inventory[i].sprite = null;
                    Inventory[i].transform.parent.gameObject.SetActive(false);
                }

            }

        }
        else { ChangeGridColor(GridColor); }

    }

    public static Sprite LoadSprite(string name)
    {
        return Resources.Load<Sprite>("Sprites/" + name);
    }
    public void OnPressed(Map.Tile t)
    {
       
        var curtile = t;

        //Debug
        print("Tiles: " + curtile.Position + " Unity: " + CursorPos + " Actor: " + curtile.Actor);

        if (curtile.Actor != null && SelectedActor == null) { SelectedActor = curtile.Actor; return; }
        else if (SelectedActor == curtile.Actor) SelectedActor = null;

        if(SelectedActor != null)
            if( CurrentBattle.ThisTurn.Order.Count > 0)
        if(GetInGameFromActor(SelectedActor).MyTurn && SelectedActor.Controllable)
        {
                var gig = GetInGameFromActor(SelectedActor);
          if(curtile.Actor == null) SelectedActor.Move(curtile);
          else { if (gig) gig.Attack(curtile.Actor,Skill.Base ); }
        }
     

    }
    public void CameraUpdate()
    {
        var curpos = Cursor.transform.position;  
        if (SelectedActor != null)
            curpos = GetInGameFromActor(SelectedActor).transform.position ;

        curpos.z = Cam.transform.position.z;
        Cam.transform.position = Vector3.Lerp(Cam.transform.position, curpos, 10 * Time.smoothDeltaTime);
    }
    private void Update()
    {

        CursorPos = new Vector(Mathf.Clamp((int)CursorPos.x, 0, CurrentBattle.map.Width - 1), Mathf.Clamp((int)CursorPos.y, 0, CurrentBattle.map.Length - 1));
        var Position = GameManager.Battlefied[(int)CursorPos.x,(int)CursorPos.y ];
        Cursor.transform.position = Vector3.Lerp(Cursor.transform.position, Position.transform.position + CursorOffset, 9 * Time.smoothDeltaTime);
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
   
        var inputs = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0);
        if (timer >= .10f && inputs)
        {

            OnCursorExit(CurrentBattle.map.AtPos(CursorPos));
            var u = new Vector(h, -v);
    
            CursorPos += u;
            timer = 0;
            OnCursorEnter(CurrentBattle.map.AtPos(CursorPos));
        }
        EstimathPath(CursorPos);
        var curtile = CurrentBattle.map.AtPos(CursorPos);
        OnCursorUpdate(curtile);
        if (Input.GetKeyDown(KeyCode.Space)) OnPressed(curtile);
        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleGrid();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            print(CurrentBattle.map);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowTabMenu();
        }
        CameraUpdate();
    }
 
    public void ResetGrid()
    {
        foreach (var item in Battlefied)
            foreach (var z in item.Sprite)
                z.enabled = !GM.ShowGrid;
        PathUI.Clear();
     
        
    }
    public void ToggleGrid()
    {
        foreach (var item in Battlefied)
        {
            foreach (var x in item.Sprite)
            {
                x.enabled = ShowGrid;
            }
        }
        ShowGrid = !ShowGrid;
    }
    public bool ShowGrid;
    public void ChangeGridColor(Color a)
    {


        foreach (var item in Battlefied)
        {
            foreach (var x in item.Sprite)
            {
                a.a = x.color.a;
                x.color = a;
            }

        }
    }
    public void ShowTabMenu()
    {
        TabButtons.SetActive(!TabButtons.activeSelf);
        TabButtons.gameObject.SetActive(!MiniMenu.gameObject.activeSelf);

    }
    public void GenerateMap(Map t)
    {
       
        grid.constraintCount = t.Tiles.GetLength(0);
        Battlefied = new BattleTile[t.Tiles.GetLength(0), t.Tiles.GetLength(1)];
        for (int x = 0; x < t.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < t.Tiles.GetLength(1); y++)
            {
                var e = Instantiate(panel, grid.transform).GetComponent<BattleTile>();
                e.tile = t.Tiles[x, y];
                foreach (var item in e.Sprite)
                {
                    var z = GridColor;
                    z.a = item.color.a;

                    item.color = z;
                }
                var v = t.Tiles[x, y].Position;
                Battlefied[x, y] = e;
                e.Value = new Vector2(v.x, v.y);
            }
      
        }
        MapName = t.ToString();
    }
  
}
