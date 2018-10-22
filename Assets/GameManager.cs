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
        CurrentBattle.map = new Map(new Vector(38, 9));
        InitializeUI();
        GenerateMap(CurrentBattle.map);
        OnHover.enabled = true;
        foreach (var item in Foes)      
            item.actor.Controllable = false;
        foreach (var item in Actors)
            item.actor.Controllable = true;
        ShowTabMenu();

        //Debug
        for (int i = 0; i < Actors.Length; i++)
        {
            Actors[i].actor.Teleport(CurrentBattle.map.AtPos(11+ i, 4));
        }
        for (int i = 0; i < Foes.Length; i++)
        {
           Foes[i].actor.Teleport(CurrentBattle.map.AtPos(12 + i, 3));
        }
        CursorPos = new Vector(9, 4);
        CreateNewItemOnField(new Consumeable("Orange Potion", "Items/SP_POTION")
        { rarity = Item.Rarity.Common, GoldValue = 10, Uses = 1, SPregen = 3 }, new Vector(5,5));
        CreateNewItemOnField(Item.Gold, new Vector(2, 5));

        ToggleGrid();

        foreach (var item in Actors) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right * 2; ;
        foreach (var item in Foes) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right * 2; ;

    }

    private void OnTurnEnd()
    {
        SelectedActor = null;
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
        if (CurrentBattle != null) CurrentBattle.BattleTime += Time.fixedDeltaTime;
       
    }


    public static List<Vector> PathUI = new List<Vector>( );
    public static int EstimathPath( Vector where)
    {


        var ThisTurnPlayer = CurrentBattle.ThisTurn.Order[0];
        GM.SpCostUI.enabled = SelectedActor != null;
        if (SelectedActor == null && CurrentBattle.ThisTurn.Order[0] == null )
        {
            GM.ResetGrid();
            return -1;
        }
        
        PathUI.Clear();
        if (ThisTurnPlayer != null && ThisTurnPlayer.Path.Count > 1 )
        {
      
            for (int h = 0; h < Battlefied.GetLength(0); h++)
                for (int j = 0; j < Battlefied.GetLength(1); j++)
                    foreach (var ff in Battlefied[h, j].Sprite)               
                        ff.enabled = ThisTurnPlayer.Path.Contains(Battlefied[h, j].tile.Position);
            return ThisTurnPlayer.Path.Count;
        }
      
        if (SelectedActor == null || SelectedActor != ThisTurnPlayer   )

        { GM.ResetGrid(); return -1; }
 
        int x = (int)(where.x - SelectedActor.TilePosition.x);
        int y = (int)(where.y - SelectedActor.TilePosition.y);
        var a = 1;
        var b = 1;
        if (x < 0) a = -1;
        if (y < 0) b = -1;

        var fs = SelectedActor.TileWalkedThisTurn ;
        
        var maximumtile =   (SelectedActor.GetStats.AGI * SelectedActor.SpAvaillableThisTurn ) - fs  ;

        var xc = PathUI.Count <= maximumtile;
        var e = (int)(PathUI.Count / SelectedActor.GetStats.AGI);
        if (Mathf.Abs(x) > Mathf.Abs(y) || CurrentBattle.map.AtPos(SelectedActor.TilePosition + Vector.up * b).Actor != null)
        {

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
                for (int i = 1; i <= Mathf.Abs(y); i++)
                {
                    PathUI.Add(lastxpos + Vector.up * i * b);
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


      
        if(PathUI.Count != 0)  
        if(PathUI.Count > maximumtile && PathUI.Count > 0)
        while (PathUI.Count > maximumtile)
                    if (PathUI.Count - 1 >= 0) PathUI.RemoveAt(PathUI.Count - 1);


        GM.SpCostUI.text = ((int)(PathUI.Count / SelectedActor.GetStats.AGI)).ToString("00") + " sp" ;

        for (int h = 0; h < Battlefied.GetLength(0); h++)
            for (int j = 0; j < Battlefied.GetLength(1); j++)
                foreach (var ff in Battlefied[h, j].Sprite)
                    ff.enabled = PathUI.Contains(Battlefied[h, j].tile.Position);

        return PathUI.Count;
    }

    public static int EstimathPath(Actor from,Vector where)
    {


        var ThisTurnPlayer = CurrentBattle.ThisTurn.Order[0];
        if (from == null && CurrentBattle.ThisTurn.Order[0] == null)
        {
            GM.ResetGrid();
            return -1;
        }

        PathUI.Clear();
        if (ThisTurnPlayer != null && ThisTurnPlayer.Path.Count > 1)
        {

            for (int h = 0; h < Battlefied.GetLength(0); h++)
                for (int j = 0; j < Battlefied.GetLength(1); j++)
                    foreach (var ff in Battlefied[h, j].Sprite)
                        ff.enabled = ThisTurnPlayer.Path.Contains(Battlefied[h, j].tile.Position);
            return ThisTurnPlayer.Path.Count;
        }

        if (from == null || from != ThisTurnPlayer)

        { GM.ResetGrid(); return -1; }

        int x = (int)(where.x - from.TilePosition.x);
        int y = (int)(where.y - from.TilePosition.y);
        var a = 1;
        var b = 1;
        if (x < 0) a = -1;
        if (y < 0) b = -1;

        var fs = from.TileWalkedThisTurn;

        var maximumtile = (from.GetStats.AGI * from.SpAvaillableThisTurn) - fs;

        var xc = PathUI.Count <= maximumtile;
        var e = (int)(PathUI.Count / from.GetStats.AGI);
        if (Mathf.Abs(x) > Mathf.Abs(y) || CurrentBattle.map.AtPos(from.TilePosition + Vector.up * b).Actor != null)
        {

            var lastxpos = from.TilePosition + Vector.right * x;
            for (int i = 1; i <= Mathf.Abs(x) && xc; i++)
            {

                if (PathUI.Count >= maximumtile)
                {
                    lastxpos = from.TilePosition + Vector.right * i * a;
                    break;
                }
                PathUI.Add(from.TilePosition + Vector.right * i * a);
            }
            for (int i = 1; i <= Mathf.Abs(y); i++)
            {
                PathUI.Add(lastxpos + Vector.up * i * b);
            }

        }
        else
        {
            var lastypos = from.TilePosition + Vector.up * y;

            for (int i = 1; i <= Mathf.Abs(y) && xc; i++)
            {
                if (PathUI.Count >= maximumtile)
                {
                    lastypos = from.TilePosition + Vector.up * i * b;
                    break;
                }
                PathUI.Add(from.TilePosition + Vector.up * i * b);

            }
            // if (PathUI.Count < maximumtile)
            for (int i = 1; i <= Mathf.Abs(x); i++)
            {

                PathUI.Add(lastypos + Vector.right * i * a);

            }
        }



        if (PathUI.Count != 0)
            if (PathUI.Count > maximumtile && PathUI.Count > 0)
                while (PathUI.Count > maximumtile)
                    if (PathUI.Count - 1 >= 0) PathUI.RemoveAt(PathUI.Count - 1);


        GM.SpCostUI.text = ((int)(PathUI.Count / from.GetStats.AGI)).ToString("00") + " sp";

        for (int h = 0; h < Battlefied.GetLength(0); h++)
            for (int j = 0; j < Battlefied.GetLength(1); j++)
                foreach (var ff in Battlefied[h, j].Sprite)
                    ff.enabled = PathUI.Contains(Battlefied[h, j].tile.Position);

        return PathUI.Count;
    }
    public void OnCursorEnter(Map.Tile t)
    {
    
    }
    public void OnCursorExit(Map.Tile t)
    {

       

    }
   
    public void OnCursorUpdate(Map.Tile t)
    {
      
        OnHover.gameObject.SetActive(ActorAtCursor != null || SelectedActor != null || Tabmenu);
        CharacterInventory.gameObject.SetActive(ActorAtCursor != null || SelectedActor != null);
        Cursor.SetBool("Hover", ActorAtCursor != null || Tabmenu);
        ActorAtCursor = t.Actor;
        if (ActorAtCursor != null)
        {
            if (ActorAtCursor.IsDefeat) ActorAtCursor = null;
            if (SelectedActor != null)
            {
                if(ActorAtCursor == SelectedActor)
                {
                    if (SelectedItem != null) ChangeGridColor(Color.magenta);
                    else ChangeGridColor(Color.gray);
                }
                else if (ActorAtCursor != SelectedActor && !CurrentBattle.IsTeamWith(ActorAtCursor, SelectedActor)) ChangeGridColor(Color.red);
                else if (CurrentBattle.IsTeamWith(ActorAtCursor, SelectedActor))
                {
                    if(SelectedItem != null) ChangeGridColor(Color.magenta);
                    else ChangeGridColor(GridColor);

                }
            }
            ShowUI(ActorAtCursor);
 
        }
        else if (SelectedItem != null) {
            ShowUI(SelectedActor);
            if (PathUI.Count ==1) ChangeGridColor(Color.cyan);
            else ChangeGridColor(Color.cyan + Color.blue);

        }
        else
        {
            if(SelectedActor != null) ShowUI(SelectedActor);
            ChangeGridColor(GridColor); }

        TabButtons.SetActive(!Tabmenu && SelectedActor != null && SelectedActor == CurrentBattle.ThisTurn.Order[0]);
    }

    public void ShowUI(Actor a)
    {
        OnHover.text =
"* " + a.Name + " *\n\nlvl"
+ a.GetLevel.ToString("00") + "\n[ hp  "
+ a.HP.ToString("00") + " ]\n[ mp "
+ a.MP.ToString("00") + " ]\n[ sp  "
+ a.SP.ToString("00") + " ]";

        for (int i = 0; i < 100; i++)
        {
            if (i < a.inventory.items.Length)
            {

                if (a.inventory.items[i] != null)
                {
                    Inventory[i].transform.parent.gameObject.SetActive(true);
                    Inventory[i].enabled = true;

                    if (Inventory[i].sprite == null)
                    {
                        Inventory[i].sprite = LoadSprite(a.inventory.items[i].ResourcePath);
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
    public static Sprite LoadSprite(string name)
    {
        return Resources.Load<Sprite>("Sprites/" + name);
    }
    public void OnPressed(Map.Tile t)
    {
       
        var curtile = t;

        //Debug
        print("Tiles: " + curtile.Position + " Unity: " + CursorPos + " Actor: " + curtile.Actor);


        
        if (HasSelectedActor && curtile.Actor != null && SelectedItem != null)
        {

            SelectedActor.Use(SelectedItem,SelectedActor);
            SelectedActor.ConsumeSP(1);
           
            if (SelectedActor.SP == 0 || SelectedItem == null || SelectedItem.Uses <= 0) StopInventory();
        }

        if (Tabmenu)
        {


            return;
        }
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
        var campos = Cursor.transform.position;  
    /*    if (SelectedActor != null)
            campos = GetInGameFromActor(SelectedActor).transform.position ;*/

        if (Tabmenu) campos = Cam.transform.position;
    
        if(SelectedItem != null)
            campos = Cursor.transform.position;



        campos.z = Cam.transform.position.z;
        Cam.transform.position = Vector3.Lerp(Cam.transform.position, campos, 10 * Time.smoothDeltaTime);
    }



    private void Update()
    {

        CursorPos = new Vector(Mathf.Clamp((int)CursorPos.x, 0, CurrentBattle.map.Width - 1), Mathf.Clamp((int)CursorPos.y, 0, CurrentBattle.map.Length - 1));
        var Position = GameManager.Battlefied[(int)CursorPos.x, (int)CursorPos.y].transform.position;

        if (CurrentBattle.IsPlayerTurn)
        {
            if (Tabmenu)
                Position = MiniMenuBTN[TabChoice].transform.position + Vector3.right * 2;
            if (inventorySelected)
                Position = Inventory[invUIItem].transform.position;
            if (SelectedItem != null) Position = GameManager.Battlefied[(int)CursorPos.x, (int)CursorPos.y].transform.position;
        }
        else CursorPos = CurrentBattle.ActingThisTurn.TilePosition;

      
        Cursor.transform.position = Vector3.Lerp(Cursor.transform.position, Position  + CursorOffset, 9 * Time.smoothDeltaTime);

        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
   
        var inputs = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0);

 

        if(CurrentBattle.IsPlayerTurn)if (timer >= .10f && inputs && (!Tabmenu || SelectedItem != null))
        {

            OnCursorExit(CurrentBattle.map.AtPos(CursorPos));
            var u = new Vector(h, -v);
    
            CursorPos += u;
            timer = 0;
            OnCursorEnter(CurrentBattle.map.AtPos(CursorPos));
        }
        EstimathPath(CursorPos);
        var curtile = CurrentBattle.map.AtPos(CursorPos);
        if (CurrentBattle.IsPlayerTurn)
        {


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
        }
        OnCursorUpdate(curtile);

        MiniMenuLogic();
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
    bool Tabmenu = false;
    public void ShowTabMenu()
    {
      
        Tabmenu = !Tabmenu;
        if (SelectedActor == null) Tabmenu = false;
            TabButtons.SetActive(!Tabmenu && SelectedActor!= null );
        MiniMenu.gameObject.SetActive(Tabmenu);
        inventorySelected = false;
        SkillsSelected = false;
        SelectedItem = null;
        if (!Tabmenu && HasSelectedActor) CursorPos = SelectedActor.TilePosition;
        TabChoice = 0;

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
                //if (y < 5) e.tile.Heigth = 5;
                    e.Value = new Vector2(v.x, v.y);
            }
      
        }
        MapName = t.ToString();
    }
    int TabChoice = 0;
    public GameObject[] MiniMenuBTN;
    bool inventorySelected, SkillsSelected;
    int invUIItem = 0;
    Item SelectedItem;
    
    void MiniMenuLogic()
    {
        if (!Tabmenu || SelectedActor == null) return;
        if (Input.GetKeyDown(KeyCode.W)) { invUIItem++; TabChoice--; }
        if (Input.GetKeyDown(KeyCode.S)) { invUIItem--; TabChoice++; }
        if (Input.GetKeyDown(KeyCode.A)) invUIItem--;
        if (Input.GetKeyDown(KeyCode.D)) invUIItem++;

            if (TabChoice > 2) TabChoice = 0;
        if (TabChoice < 0) TabChoice = 2;


        invUIItem = Mathf.Clamp(invUIItem, 0, SelectedActor.inventory.items.Length-1);
        
   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inventorySelected && SelectedActor.inventory.items[invUIItem] != null &&  SelectedItem == null)
            {
                var e = SelectedActor.inventory.items[invUIItem];
                SelectedItem = e;
                print(SelectedItem.Name + "is Selected");
                return;
            }
            if (TabChoice == 1) { inventorySelected = true; invUIItem = 0; } 
            
        
        }

        if (Input.GetKeyDown(KeyCode.Tab)){
            if (SelectedItem == null)
                StopInventory();
            else SelectedItem = null;


        }
        //Inventory
      



    }
    void StopInventory()
    {
        SelectedItem = null;
        inventorySelected = false;
        invUIItem = 0;
    }

    public bool HasSelectedActor
    {
        get { return SelectedActor != null; }
    }
}
