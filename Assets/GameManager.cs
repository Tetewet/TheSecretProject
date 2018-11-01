using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    /// <summary>
    /// Are we in battle mode?
    /// </summary>
    public static bool BattleMode 
    {
       get { return !(CurrentBattle == null || CurrentBattle.OnGoing); }
    }
    public static GameManager GM;
    public static bool InBattleMode = true;
    public Camera Cam;
    public static Vector3 VecTo3(Vector v)
    {
        return new Vector3(v.x, v.y, 0);
    }
    public static BattleTile[,] Battlefied;

    Image[] Inventory;GameObject[] Skills;
    public GridLayoutGroup grid, CharacterInventory;
    public Color GridColor;
    [Header("Templates")]

    public BattleField[] Battlefields;
    public Text SpCostUI, InfoBar;
    public GameObject ActorPrefab;
    public GameObject ItemPrefab;
    public GameObject SkillsPrefab;
   
    public GameObject panel, InventoryCeil;
    public RectTransform GameEnd;
    public GameObject TabButtons, MiniMenu;
    public Text OnHover;
    public Animator Cursor;
    public static Vector CursorPos;
    public Vector3 CursorOffset;
    public static Actor SelectedActor;
    public static Actor ActorAtCursor = null;


    AudioSource audi;

    [System.Serializable]
    public struct BattleField
    {

        public AudioClip[] Sounds;
        public GameObject Map;

    }


    public static InGameItem CreateNewItemOnField(Item i, Vector Position)
    {


        if(i is Weapon)
        {
            InGameWeapon.GenerateInGameWeapon(i as Weapon);
            return null;
        }
        var e = Instantiate(GM.ItemPrefab, Vector3.zero, Quaternion.identity).GetComponent<InGameItem>();
        GM.InGameItems.Add(e);
        e.item = i;
        CurrentBattle.map.AtPos(Position).AddItem(e.item);
        return e;
    }
    public static Battle CurrentBattle;
    public string MapName;


    /// <summary>
    /// The Protagonists
    /// </summary>
    public static List<Actor> Protags = new List<Actor>
    {
        new Player("Nana",new Stat{ AGI  =2 , END =1, INT =4, LUC =2 , STR = 1, WIS =5 }, true, "Mage"){ inventory = Actor.Inventory.Light},
        new Player("Mathew", new Stat{ STR = 6, AGI = 2, END =4, LUC =3 ,WIS = 1, INT = 0},true,"Barbarian"){ inventory = Actor.Inventory.Light}
    };

    public List<InGameActor> InGameActors = new List<InGameActor>(), InGameFoes = new List<InGameActor>();
    public List<InGameItem> InGameItems = new List<InGameItem>();


    public static InGameActor GenerateInGameActor(Actor f)
    {
        var e = Instantiate(GM.ActorPrefab, Vector3.zero, Quaternion.identity).GetComponent<InGameActor>();
        e.InitializedActor(f, f.AnimatorPath, LoadAnimatorController("Actor"));

        
        return e;
    }

    public static void ClearBattle(Battle b)
    {

        b.History.Clear();
        b.Foes.Clear();
        b.Players.Clear();
        b.map = null;


    }

    void ClearActor()
    {
        foreach (var h in GM.InGameActors)
            Destroy(h.gameObject);
        foreach (var l in GM.InGameFoes)
            Destroy(l.gameObject);

        GM.InGameActors.Clear();
        GM.InGameFoes.Clear();
    }
    public static void StartBattle(Actor[] F, Map m, int Map = 0)
    {
        GM.Cam.enabled = true;
        GM.Cursor.gameObject.SetActive(true);
        Map = Mathf.Clamp(Map, 0, GM.Battlefields.Length - 1);
        for (int i = 0; i < GM.Battlefields.Length; i++)
            GM.Battlefields[i].Map.SetActive(false);
        GM.Battlefields[Map].Map.SetActive(true);

        GM.audi.clip = GM.Battlefields[Map].Sounds[0];
        GM.audi.Play();


        if (CurrentBattle != null)
            ClearBattle(CurrentBattle);

        CurrentBattle = null;
        GM.GameEnd.gameObject.SetActive(false);
        GM.ShowTabMenu(false);
        GM.GameEnd.transform.gameObject.SetActive(false);
        CurrentBattle = new Battle(Protags.ToArray(), F);
        CurrentBattle.BattlEnd += GM.OnBattleEnd;
        CurrentBattle.OnTurnEnd += GM.OnTurnEnd;
        CurrentBattle.map = m;

        GM.GenerateMap(CurrentBattle.map);

        GM.OnHover.enabled = true;

        GM.ClearActor();
 

        foreach (var h in Protags)
        {
            GM.InGameActors.Add(GenerateInGameActor(h));
            h.SP = 0;
            if (h.HP < 0) h.HP = 1;
        }
          
        foreach (var q in F)
        {
            GM.InGameFoes.Add(GenerateInGameActor(q));
            q.AddGold(Random.Range(0, 5));
        }


        for (int i = 0; i < GM.InGameActors.Count; i++)
            GM.InGameActors[i].actor.Teleport(CurrentBattle.map.AtPos(6 + i, 4));
        for (int i = 0; i < GM.InGameFoes.Count; i++)
        {
            GM.InGameFoes[i].actor.Teleport(CurrentBattle.map.AtPos(12 + i, 3 + Random.Range(-2, 2)));
            GM.InGameFoes[i].actor.Heal();
            if (GM.InGameFoes[i].actor is Monster)
            {
                var ggd = GM.InGameFoes[i].actor as Monster;
                CurrentBattle.BattleExp += ggd.ExpGain;
            }
           
        }
           




        CursorPos = new Vector(9, 4);
        GM.ShowGrid = false;
        GM.ToggleGrid();

        foreach (var item in GM.InGameActors) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right * 1; ;
        foreach (var item in GM.InGameFoes) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right * 1; ;
        CurrentBattle.StartNewTurn();


        GM.StartCoroutine(GM.BattleStart());

    }
    IEnumerator BattleStart()
    {
        yield return new WaitForSeconds(.1f);
        foreach (var item in GM.InGameActors) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right * 1; ;
        foreach (var item in GM.InGameFoes) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right * 1; ;
        yield return new WaitForSeconds(1);
        CurrentBattle.Paused = false;
        CurrentBattle.Proceed();
        yield break;
    }
    /// <summary>
    /// Get the InGameActor from the Actor.
    /// </summary>
    /// <param name="actor">Actor to get the InGameActorFrom</param>
    /// <returns></returns>
    public static InGameActor GetInGameFromActor(Actor actor)
    {
        foreach (var item in GM.InGameActors)
            if (item.actor == actor) { return item; }
        foreach (var item in GM.InGameFoes)
            if (item.actor == actor) { return item; }
        return null;
    }
    /// <summary>
    /// Initialize the UI
    /// </summary>
    void InitializeUI()
    {
        Inventory = new Image[100];
        for (int i = 0; i < Inventory.Length; i++)
            Inventory[i] = Instantiate(InventoryCeil, CharacterInventory.transform).GetComponentInChildren<Image>();

        Skills = new GameObject[100];
        for (int i = 0; i < Skills.Length; i++)
            Skills[i] = Instantiate(SkillsPrefab,SkillList.transform);

    }
    private void Awake()
    {
        if (!GM) GM = this;
        else Destroy(this.gameObject);
       DontDestroyOnLoad(this.gameObject);
        audi = GetComponent<AudioSource>();


        Protags[0].SetProfession( Profession.Madoshi);

    }
    public void Start()
    {

     
        GM.InitializeUI();

        foreach (var item in Protags)
        {
            item.Heal();
        }
        //14 6
        var nGroup = new List<Monster>();
      
        for (int i = 0; i < Random.Range(1, 5 ); i++)
            nGroup.Add(new Monster("Kuku " + i, new Stat { AGI = 4, END = 3, LUC = 20, STR = 2 }, false, "~Kuku"));
        StartBattle(nGroup.ToArray(), new Map(new Vector(38, 9)),0);


        Protags[1].Equip(
       new Weapon("Iron Sword")
       {
           slot = Equipement.Slot.Weapon,
           StatsBonus = new Stat { STR = 2 },
           DamageType = DamageType.Slashing,
           WeaponType = Weapon.type.Sword,
           GoldValue = 100,
           rarity = Item.Rarity.Common,
           Durability = 100
       })
        ;

        //Debug


        CreateNewItemOnField(new Consumeable("Orange Potion", "Items/SP_POTION")
        { rarity = Item.Rarity.Common, GoldValue = 10, Uses = 1, SPregen = 3 }, new Vector(11, 5));
        CreateNewItemOnField(Item.Gold, new Vector(2, 5));



    }

    /// <summary>
    /// Is called at the end of the turn
    /// </summary>
    private void OnTurnEnd()
    {
        SelectedActor = null;
        CloseInventory();
        ResetGrid();
    }

    /// <summary>
    /// Called when the battle end
    /// </summary>
    private void OnBattleEnd()
    {
        GM.Cursor.gameObject.SetActive(false);
        ShowTabMenu(false);
        SelectedActor = null;
        PathUI.Clear();
        foreach (var item in Protags)
        {
            item.AddExp(CurrentBattle.BattleExp / Protags.Count);
        }
        StartCoroutine(BattleEndTransition());

    }

    public GridLayoutGroup Spoils;
   List<GameObject> spoils_Inventory = new List<GameObject>();
    public Text spoils_Gold, spoils_grade, spoils_BattleTime;

    //THE END
    IEnumerator BattleEndTransition()
    {
        yield return new WaitForSeconds(1.5f);
        GameEnd.anchorMax = new Vector2(0.5f,1);
        GameEnd.anchorMin = new Vector2(0.5f,1);
        GameEnd.anchoredPosition = Vector2.zero + Vector2.up * 25;
 
        GameEnd.gameObject.SetActive(true);
 

        spoils_Gold.text = CurrentBattle.GoldEarnedThisBattle.ToString("0000") + " Gold"    ;
        var s = CurrentBattle.BattleTime; var m = 0;
        while ((s -60) > 0)
        {
            s -= 60;
            m++;
        }
        spoils_BattleTime.text = "Battle Time: " + m.ToString("00") + ":" + s.ToString("00");
        spoils_BattleTime.enabled = false;
        spoils_Gold.enabled = false;
        spoils_grade.text = CurrentBattle.Grade;
        spoils_grade.enabled = false;

        ///Clean the inventory - We should spawn 100 at the start of 'em instead of doing this
        var vx = spoils_Inventory;
        for (int i = 0; i < vx.Count; i++)
        {
            if (vx[i] == Spoils.transform) continue;
            Destroy(vx[i].gameObject);
        }
        spoils_Inventory.Clear();

        var e = InGameItems.Count;
        for (int i = 0; i < 20; i++)
        {

            if (spoils_Inventory.Count >= 20) break;
            if (i < e && InGameItems[i])
            {
                Item.Inventory.Add(InGameItems[i].item);
                var h = Instantiate(InventoryCeil, Spoils.transform).GetComponentInChildren<Image>();
                h.sprite = LoadSprite(InGameItems[i].item.ResourcePath);
                h.enabled = true;
                InGameItems[i].gameObject.SetActive(false);
                spoils_Inventory.Add(h.transform.parent.gameObject);
                yield return new WaitForSeconds(.5f);
                continue;
            }
            else
            {
     
                var h = Instantiate(InventoryCeil, Spoils.transform);
                spoils_Inventory.Add(h);
                yield return new WaitForSeconds(.1f);
            }

        }
        while (!Input.anyKey)
        {
            yield return new WaitForSeconds(.5f);
            spoils_BattleTime.enabled = true;
            yield return new WaitForSeconds(.5f);
            spoils_Gold.enabled = true;
            yield return new WaitForSeconds(1f);
            spoils_grade.enabled = true;

            break;
        }
        spoils_BattleTime.enabled = true;
        spoils_Gold.enabled = true;
        spoils_grade.enabled = true;
        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < InGameItems.Count; i++)
        {
            Destroy(InGameItems[i].gameObject);
        } 


        GameEnd.gameObject.SetActive(false);
    
        InGameItems.Clear();
         

        foreach (var item in Battlefields)
            item.Map.gameObject.SetActive(false);

        ClearActor();

        GM.Cam.enabled = false;
        //------------------------------------------------------

        var nGroup = new List<Monster>();
        var gd = 0;
        foreach (var item in Protags)
        {
            gd += item.GetLevel;
        }
        for (int i = 0; i < Random.Range(2, 5 + gd); i++)
        {
            nGroup.Add(new Monster("Kuku " + i, new Stat { AGI = 4, END =3, LUC = 20,STR =2 }, false, "~Kuku"));
        }
        StartBattle(nGroup.ToArray(), new Map(new Vector(38, 9)),0);
        yield break;
    }
    float timer = 0;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (CurrentBattle != null) CurrentBattle.BattleTime += Time.fixedUnscaledDeltaTime;

    }

    /// <summary>
    /// Show a bar in the upper section of the screen
    /// </summary>
    /// <param name="Message">What to show</param>
    public static void GiveInfo(string Message)
    {
        var e = GameManager.GM;

        e.InfoBar.text = Message;
        e.StartCoroutine(e._ShowInfo());

    }
    IEnumerator _ShowInfo()
    {
        InfoBar.transform.parent.gameObject.SetActive(true);
        StartCoroutine(_freezecam(.5f));
        yield return new WaitForSeconds(1f);
        InfoBar.transform.parent.gameObject.SetActive(false);

        yield break;
    }

    public static List<Vector> PathUI = new List<Vector>();

    /// <summary>
    /// Create a path toward a certain position from SelectedActor
    /// </summary>
    /// <param name="Position">Where.</param>
    /// <returns></returns>
    public static int EstimathPath(Vector Position)
    {


        var ThisTurnPlayer = CurrentBattle.ThisTurn.Order[0];
        GM.SpCostUI.enabled = SelectedActor != null;
        if (SelectedActor == null && CurrentBattle.ThisTurn.Order[0] == null)
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

        if (SelectedActor == null || SelectedActor != ThisTurnPlayer)

        { GM.ResetGrid(); return -1; }

        int x = (int)(Position.x - SelectedActor.TilePosition.x);
        int y = (int)(Position.y - SelectedActor.TilePosition.y);
        var a = 1;
        var b = 1;
        if (x < 0) a = -1;
        if (y < 0) b = -1;

        var fs = SelectedActor.TileWalkedThisTurn;

        var maximumtile = (SelectedActor.GetStats.AGI * SelectedActor.SpAvaillableThisTurn) - fs;

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



        if (PathUI.Count != 0)
            if (PathUI.Count > maximumtile && PathUI.Count > 0)
                while (PathUI.Count > maximumtile)
                    if (PathUI.Count - 1 >= 0) PathUI.RemoveAt(PathUI.Count - 1);


        GM.SpCostUI.text = ((int)(PathUI.Count / SelectedActor.GetStats.AGI)).ToString("00") + " sp";

        for (int h = 0; h < Battlefied.GetLength(0); h++)
            for (int j = 0; j < Battlefied.GetLength(1); j++)
                foreach (var ff in Battlefied[h, j].Sprite)
                    ff.enabled = PathUI.Contains(Battlefied[h, j].tile.Position);

        return PathUI.Count;
    }
    /// <summary>
    /// Create a tile-path from a actor toward a position
    /// </summary>
    /// <param name="Whom">The actor in question</param>
    /// <param name="where">The position in question</param>
    /// <returns></returns>
    public static int EstimathPath(Actor Whom, Vector where)
    {


        var ThisTurnPlayer = CurrentBattle.ThisTurn.Order[0];
        if (Whom == null && CurrentBattle.ThisTurn.Order[0] == null)
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

        if (Whom == null || Whom != ThisTurnPlayer)

        { GM.ResetGrid(); return -1; }

        int x = (int)(where.x - Whom.TilePosition.x);
        int y = (int)(where.y - Whom.TilePosition.y);
        var a = 1;
        var b = 1;
        if (x < 0) a = -1;
        if (y < 0) b = -1;

        var fs = Whom.TileWalkedThisTurn;

        var maximumtile = (Whom.GetStats.AGI * Whom.SpAvaillableThisTurn) - fs;

        var xc = PathUI.Count <= maximumtile;
        var e = (int)(PathUI.Count / Whom.GetStats.AGI);
        if (Mathf.Abs(x) > Mathf.Abs(y) || CurrentBattle.map.AtPos(Whom.TilePosition + Vector.up * b).Actor != null)
        {

            var lastxpos = Whom.TilePosition + Vector.right * x;
            for (int i = 1; i <= Mathf.Abs(x) && xc; i++)
            {

                if (PathUI.Count >= maximumtile)
                {
                    lastxpos = Whom.TilePosition + Vector.right * i * a;
                    break;
                }
                PathUI.Add(Whom.TilePosition + Vector.right * i * a);
            }
            for (int i = 1; i <= Mathf.Abs(y); i++)
            {
                PathUI.Add(lastxpos + Vector.up * i * b);
            }

        }
        else
        {
            var lastypos = Whom.TilePosition + Vector.up * y;

            for (int i = 1; i <= Mathf.Abs(y) && xc; i++)
            {
                if (PathUI.Count >= maximumtile)
                {
                    lastypos = Whom.TilePosition + Vector.up * i * b;
                    break;
                }
                PathUI.Add(Whom.TilePosition + Vector.up * i * b);

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


        GM.SpCostUI.text = ((int)(PathUI.Count / Whom.GetStats.AGI)).ToString("00") + " sp";

        for (int h = 0; h < Battlefied.GetLength(0); h++)
            for (int j = 0; j < Battlefied.GetLength(1); j++)
                foreach (var ff in Battlefied[h, j].Sprite)
                    ff.enabled = PathUI.Contains(Battlefied[h, j].tile.Position);

        return PathUI.Count;
    }
    public static int EstimathPath(Actor Whom, Vector where, int maximum)
    {


        var ThisTurnPlayer = CurrentBattle.ThisTurn.Order[0];
        if (Whom == null && CurrentBattle.ThisTurn.Order[0] == null)
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

        if (Whom == null || Whom != ThisTurnPlayer)

        { GM.ResetGrid(); return -1; }

        int x = (int)(where.x - Whom.TilePosition.x);
        int y = (int)(where.y - Whom.TilePosition.y);
        var a = 1;
        var b = 1;
        if (x < 0) a = -1;
        if (y < 0) b = -1;
 

        var maximumtile = maximum;

        var xc = PathUI.Count <= maximumtile;
        var e = (int)(PathUI.Count / Whom.GetStats.AGI);
        if (Mathf.Abs(x) > Mathf.Abs(y) || CurrentBattle.map.AtPos(Whom.TilePosition + Vector.up * b).Actor != null)
        {

            var lastxpos = Whom.TilePosition + Vector.right * x;
            for (int i = 1; i <= Mathf.Abs(x) && xc; i++)
            {

                if (PathUI.Count >= maximumtile)
                {
                    lastxpos = Whom.TilePosition + Vector.right * i * a;
                    break;
                }
                PathUI.Add(Whom.TilePosition + Vector.right * i * a);
            }
            for (int i = 1; i <= Mathf.Abs(y); i++)
            {
                PathUI.Add(lastxpos + Vector.up * i * b);
            }

        }
        else
        {
            var lastypos = Whom.TilePosition + Vector.up * y;

            for (int i = 1; i <= Mathf.Abs(y) && xc; i++)
            {
                if (PathUI.Count >= maximumtile)
                {
                    lastypos = Whom.TilePosition + Vector.up * i * b;
                    break;
                }
                PathUI.Add(Whom.TilePosition + Vector.up * i * b);

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


        GM.SpCostUI.text = ((int)(PathUI.Count / Whom.GetStats.AGI)).ToString("00") + " sp";

        for (int h = 0; h < Battlefied.GetLength(0); h++)
            for (int j = 0; j < Battlefied.GetLength(1); j++)
                foreach (var ff in Battlefied[h, j].Sprite)
                    ff.enabled = PathUI.Contains(Battlefied[h, j].tile.Position);

        return PathUI.Count;
    }

    /// <summary>
    /// Set the current SelectedActor
    /// </summary>
    /// <param name="Actor"></param>
    public static void SetActor(Actor Actor)
    {
        SelectedActor = Actor;
    }
    /// <summary>
    /// Called once When the cursor enter a tile
    /// </summary>
    /// <param name="t"> The tile in question</param>
    public void OnCursorEnter(Map.Tile t)
    {

    }
    /// <summary>
    /// Called once the cursor exited a tile
    /// </summary>
    /// <param name="t">The tile in question</param>
    public void OnCursorExit(Map.Tile t)
    {



    }

    /// <summary>
    /// Is called on Update
    /// </summary>
    /// <param name="t">The cursor in question</param>
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
                if (ActorAtCursor == SelectedActor)
                {
                    if (SelectedItem != null) ChangeGridColor(Color.magenta);

                    else ChangeGridColor(Color.gray);
                }
                else if (ActorAtCursor != SelectedActor && !CurrentBattle.IsTeamWith(ActorAtCursor, SelectedActor)) ChangeGridColor(Color.red);
                else if (CurrentBattle.IsTeamWith(ActorAtCursor, SelectedActor))
                {
                    if (SelectedItem != null) ChangeGridColor(Color.magenta);
                    else ChangeGridColor(GridColor);

                }
            }
            ShowUI(ActorAtCursor);

        }
        else if (SelectedItem != null) {
            ShowUI(SelectedActor);
            if (PathUI.Count == 1) ChangeGridColor(Color.cyan);
            else ChangeGridColor(Color.cyan + Color.blue);

        }
        else if (SelectedSkill != null)
        {
            ShowUI(SelectedActor);
            ChangeGridColor(Color.yellow);
        }
        else
        {
            if (SelectedActor != null) ShowUI(SelectedActor);
            ChangeGridColor(GridColor); }

        TabButtons.SetActive(!Tabmenu && SelectedActor != null && SelectedActor == CurrentBattle.ThisTurn.Order[0]);
    }

    /// <summary>
    /// Show the On-Screen info about this actor
    /// </summary>
    /// <param name="a">Actor in question</param>
    public void ShowUI(Actor a)
    {

        if (a == null)
        {
            print(a);
            return;
        }
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
    /// <summary>
    /// Load a sprite from the path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Sprite LoadSprite(string path)
    {
        return Resources.Load<Sprite>("Sprites/" + path);
    }
    public static RuntimeAnimatorController LoadAnimatorController(string path)
    {
        return Resources.Load<RuntimeAnimatorController>("Animators/" + path);
    }

    /// <summary>
    /// Is called once when the cursor activated
    /// </summary>
    /// <param name="t"></param>
    public void OnPressed(Map.Tile t)
    {

        var curtile = t;

        //Debug
        print("Tiles: " + curtile.Position + " Unity: " + CursorPos + " Actor: " + curtile.Actor);



        if (HasSelectedActor && curtile.Actor != null && SelectedItem != null)
        {
            GetInGameFromActor(SelectedActor).UseItem(curtile.Actor, SelectedItem);
            CloseInventory();
        }
        else
        if (HasSelectedActor && curtile.Actor != null && SelectedSkill != null)
        {
            if (GameManager.EstimathPath(SelectedActor, GameManager.CursorPos, 99) > SelectedSkill.Reach)
            {
                GiveInfo("Can't reach there");
                return;}
            else
            {
                GetInGameFromActor(SelectedActor).UseSkill(curtile.Actor, SelectedSkill);
                CloseInventory();
            }
            

            
        }
        else if(HasSelectedActor && SelectedSkill != null)
        {
            GiveInfo("No targets!");
            return;
        }

        if (Tabmenu) return;

        if (curtile.Actor != null && SelectedActor == null) { SelectedActor = curtile.Actor; return; }
        else if (SelectedActor == curtile.Actor) SelectedActor = null;

        if (SelectedActor != null)
        {
            if (CurrentBattle.ThisTurn.Order.Count > 0)
                if (GetInGameFromActor(SelectedActor).MyTurn && SelectedActor.Controllable)
                    if (curtile.Actor == null) SelectedActor.Move(curtile);
                    else if (curtile.Actor != null && SelectedActor.CanUseSkill(Skill.Base)) {

                        if (SelectedActor.inventory.HasWeapon)
                            foreach (var item in SelectedActor.inventory.GetWeapons)
                            {
                                if (item != null) GetInGameFromActor(SelectedActor).Attack(curtile.Actor, SelectedSkill);
                            }
                        else GetInGameFromActor(SelectedActor).Attack(curtile.Actor, Skill.Base);
                    }
        }
    }

    /// <summary>
    /// Camera Update
    /// </summary>
    public void CameraUpdate()
    {
        var campos = Cursor.transform.position;
        /*    if (SelectedActor != null)
                campos = GetInGameFromActor(SelectedActor).transform.position ;*/

        if (Tabmenu) campos = Cam.transform.position;
        if (SelectedItem != null || SelectedSkill != null)
            campos = Cursor.transform.position;
        campos.z = Cam.transform.position.z;
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 5,12 * Time.smoothDeltaTime);
        if (freezeCam) campos = Cam.transform.position;
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
            if (SkillsSelected)
                Position = Skills[skillUI].transform.position;
                
            if (SelectedItem != null || SelectedSkill != null) Position = GameManager.Battlefied[(int)CursorPos.x, (int)CursorPos.y].transform.position;
        }
        else CursorPos = CurrentBattle.ActingThisTurn.TilePosition;


        Cursor.transform.position = Vector3.Lerp(Cursor.transform.position, Position + CursorOffset, 9 * Time.smoothDeltaTime);

        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        var inputs = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0);



        if (CurrentBattle.IsPlayerTurn) if (timer >= .10f && inputs && (!Tabmenu || SelectedItem != null || SelectedSkill != null))
            {

                OnCursorExit(CurrentBattle.map.AtPos(CursorPos));
                var u = new Vector(h, -v);

                CursorPos += u;
                timer = 0;
                OnCursorEnter(CurrentBattle.map.AtPos(CursorPos));
            }

        if(SelectedSkill != null)
            EstimathPath(SelectedActor,CursorPos, 99);
        else 
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
            if (Input.GetKeyDown(KeyCode.Tab) && (!inventorySelected && !SkillsSelected))
            {
                ToggleTabMenu();
            }
        }
        OnCursorUpdate(curtile);

        MiniMenuLogic();
        CameraUpdate();
    }

    /// <summary>
    /// Reset the Grid
    /// </summary>
    public void ResetGrid()
    {
        foreach (var item in Battlefied)
            foreach (var z in item.Sprite)
                z.enabled = !GM.ShowGrid;
        PathUI.Clear();


    }
    /// <summary>
    /// Toggle the grid
    /// </summary>
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

    /// <summary>
    /// Change the color of the grid
    /// </summary>
    /// <param name="c">the Color</param>
    public void ChangeGridColor(Color c)
    {


        foreach (var item in Battlefied)
        {
            foreach (var x in item.Sprite)
            {
                c.a = x.color.a;
                x.color = c;
            }

        }
    }
    bool Tabmenu = false;
    /// <summary>
    /// Close or Open Tab Menu
    /// </summary>
    public void ShowTabMenu(bool a)
    {
        Tabmenu = a;

        
        if (!BattleMode && !HasSelectedActor || (HasSelectedActor && SelectedActor != CurrentBattle.ActingThisTurn) ) Tabmenu = false;

        TabButtons.SetActive(!Tabmenu && SelectedActor != null);
        MiniMenu.gameObject.SetActive(Tabmenu);
        inventorySelected = false;
        SkillsSelected = false;
        SkillList.SetActive(false);
        SelectedItem = null;
        if (!Tabmenu && HasSelectedActor) CursorPos = SelectedActor.TilePosition;
        TabChoice = 0;

        if (!a) StartCoroutine(_freezecam(.5f));
    }
    /// <summary>
    /// ToggleTheTabMenu
    /// </summary>
    public void ToggleTabMenu()
    {

        Tabmenu = !Tabmenu;
        ShowTabMenu(Tabmenu);

    }

    /// <summary>
    /// Generate the map on the field from a Map object 
    /// </summary>
    /// <param name="t">The map to load</param>
    public void GenerateMap(Map t)
    {
        var q = grid.GetComponentsInChildren<Transform>();

        for (int i = 0; i < q.Length; i++)
            if (q[i] != grid.transform) Destroy(q[i].gameObject);



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
    public GameObject SkillList;
    Skill SelectedSkill;
    int skillUI = 0;
    
    bool inventorySelected, SkillsSelected;
    int invUIItem = 0;
    Item SelectedItem;

    bool freezeCam = false;
    IEnumerator _freezecam(float f)
    {
        freezeCam = true;
        yield return new WaitForSeconds(f);
        freezeCam = false;
        yield break;
    }
    
    public void ActionFreeze()
    {
        StartCoroutine(_freezecam(1.5f));
    }

    /// <summary>
    /// Logic about the menu - run on update
    /// </summary>
    void MiniMenuLogic()
    {

        if (!Tabmenu || SelectedActor == null) return;
        if (Input.GetKeyDown(KeyCode.W)) { invUIItem++; TabChoice--;
            skillUI--; }
        if (Input.GetKeyDown(KeyCode.S)) { invUIItem--; TabChoice++; skillUI++; }
        if (Input.GetKeyDown(KeyCode.A)) { invUIItem--; if (TabChoice == 3) TabChoice = 0; }
        if (Input.GetKeyDown(KeyCode.D)) { invUIItem++; if (TabChoice < 3) TabChoice = 3; ; }

        if (TabChoice > 3) TabChoice = 0;
        if (TabChoice < 0) TabChoice = 3;
        if (skillUI < 0) skillUI = SelectedActor.Class.UsableSkill.Length - 1;
        if (skillUI > SelectedActor.Class.UsableSkill.Length - 1) skillUI = 0;


        invUIItem = Mathf.Clamp(invUIItem, 0, SelectedActor.inventory.items.Length - 1);
        skillUI = Mathf.Clamp(skillUI, 0, SelectedActor.Class.UsableSkill.Length);



        if(Tabmenu)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventorySelected)
            {
                    

                    if (SelectedItem != null) SelectedItem = null;
                else if (inventorySelected) inventorySelected = false;
                else CloseInventory();
            }
            else if ( SkillsSelected)
            {


                    if (SelectedSkill != null) { SkillList.SetActive(true); SelectedSkill = null; }
                    else if (SkillsSelected) { SkillList.SetActive(false); SkillsSelected = false; }
                    else CloseInventory();

            }


        }


        if(SelectedSkill == null && SelectedItem == null)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inventorySelected && SelectedActor.inventory.items[invUIItem] != null &&  SelectedItem == null)
            {
                var e = SelectedActor.inventory.items[invUIItem];
                SelectedItem = e;
            
                return;
            }
            else
            if(SkillsSelected && SelectedActor.Class.UsableSkill[skillUI] != null && SelectedSkill == null )
            {
                var e  = SelectedActor.Class.UsableSkill[skillUI];
                if (PathUI.Count > 0)
                    PathUI.Clear();

                StartCoroutine(_freezecam(.4f));
                SelectedSkill = e;
                return;
            }
            if (TabChoice == 0)
            {
                skillUI = 0;
           
                SkillsSelected = true;
                for (int i = 0; i < Skills.Length; i++)
                {
                    if (i < SelectedActor.Class.UsableSkill.Length)
                    {
                        Skills[i].GetComponent<InGameSkill>().ShowSkill(SelectedActor.Class.UsableSkill[i]);
                        Skills[i].gameObject.SetActive(true);
                    }
                    else Skills[i].gameObject.SetActive(false);
                }
                SkillList.SetActive(true);
            }
            else if (TabChoice == 1) { inventorySelected = true; invUIItem = 0; } 
            else if (TabChoice == 2 && SelectedActor.SP > 0) {
              
                GetInGameFromActor(SelectedActor).EnterDefenseMode();
                ShowTabMenu(false);
             
            }
            if (TabChoice == 3 && SelectedActor.SP > 0)
            {

                GetInGameFromActor(SelectedActor).EndTurn();
                ShowTabMenu(false);
            }


        }

 
        //Inventory
      



    }

    
    /// <summary>
    /// Close the Inventory
    /// </summary>
    void CloseInventory()
    {
        StartCoroutine(_freezecam(.2f));
        skillUI = 0;
        SkillsSelected = false;
        SelectedSkill = null;
        SelectedItem = null;
        inventorySelected = false;
        invUIItem = 0;
    }
    
 
    /// <summary>
    /// Return true if there is a actor selected
    /// </summary>
    public bool HasSelectedActor
    {
        get { return SelectedActor != null; }
    }
}
