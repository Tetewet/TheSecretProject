using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Index of everything in the program. That ways we can call everything from any point at any time. Good for Events.
    /// </summary>
    public static Dictionary<string, object> Index = new Dictionary<string, object>();
 
    public string LOG;
    public static string GenerateID(object G)
    {

        var x = G.GetType().Name.ToUpper();
        System.Type a = G.GetType();
        //So we can have certain Item

        bool Filter = G.GetType() != typeof(Monster) && G.GetType() != typeof(Equipement);
        if (a.BaseType != typeof(System.Object) && a.BaseType != typeof(System.ValueType) && Filter)
        while (true)
        {
        
            if (a.BaseType == typeof(System.Object)) break;
            a = a.BaseType;
            x = a.Name.ToUpper();
          


        }
        var s = "";
        for (int i = 0; i < 3; i++)
            s += x[i];

        var oc = 0;
        foreach (var item in Index)
        {
            if (item.Key.Contains(s)) oc++;
        }
     
        s += 0 + oc;
        Index.Add(s, G);

        GameManager.GM.LOG += s + "(" + a.Name + ") added to INDEX\n";
        return s;
    }
    /// <summary>
    /// Are we in battle mode?
    /// </summary>
    public static bool BattleMode
    {
        get { return (CurrentBattle != null && CurrentBattle.OnGoing); }
    }
    public static GameManager GM;
    public static bool InBattleMode = true;
    public Camera Cam, OverworldCam;
    public static Vector3 VecTo3(Vector v)
    {
        return new Vector3(v.x, v.y, 0);
    }
    public static BattleTile[,] Battlefied;

    Image[] Inventory; GameObject[] Skills;
    public GridLayoutGroup grid, CharacterInventory;
    public Color GridColor;
    [Header("Templates")]

    public BattleField[] Battlefields;
    public Text SpCostUI, InfoBar;
    public GameObject ActorPrefab;
    public GameObject ItemPrefab;
    public GameObject SkillsPrefab;
    [Header("SFX")]
    public AudioClip click;
    public AudioClip select;
    public AudioClip sfxbattlestart;


    [Header("UI")]
    public Image DarknessMyOldFriend;
    public RectTransform BattleStartGameObject;
    public GameObject SkillsCursorPos;
    public Canvas TextAndUI;
    public UI_status uiStatus;
    public GameObject onHoverGO;
    public static string language = "fr";


    [Header("BattleMode")]
    public GameObject panel, InventoryCeil;
    public RectTransform GameEnd;
    public GameObject TabButtons, MiniMenu;
    public Text OnHover;
    public Animator Cursor;
    public static Vector CursorPos;
    public Vector3 CursorOffset;
    public static Actor SelectedActor;
    public static Actor ActorAtCursor = null;

    [Header("Overworld")]
    public GameObject OverWorldGO;
    public Tilemap Main;
    public Tilemap[] Events;
    /// <summary>
    /// The overworld
    /// </summary>
    public static Overworld Map;


    AudioSource audi;
    public AudioSource audiSFX;

    [System.Serializable]
    public struct BattleField
    {

        public AudioClip[] Sounds;
        public GameObject Map;

    }



    public static InGameItem CreateNewItemOnField(Item i, Vector Position)
    {


        if (i is Weapon)
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
    public static List<Actor> Protags;

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
    IEnumerator Transition(Color x)
    {
        var e = 0f;


        while (e < 3)
        {
            DarknessMyOldFriend.color = Color.Lerp(DarknessMyOldFriend.color, x, (e + 1) * Time.smoothDeltaTime);
            e += Time.fixedDeltaTime;
            yield return null;
        }
        yield break;
    }
    public IEnumerator BattleTransition(Actor[] F, Map m, int map = 0)
    {
        var f = OverworldCam.orthographicSize;
        OverworldCam.orthographicSize /= 2;    
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.1f);
        audiSFX.PlayOneShot(sfxbattlestart);
   
      

       
        var e = 0f;


        while (e < 3)
        {
            DarknessMyOldFriend.color = Color.Lerp(DarknessMyOldFriend.color,Color.black, (e + 1) * Time.unscaledDeltaTime);
            e += Time.unscaledDeltaTime;
            yield return null;
        }

        StartBattle(F, m, map);
        Time.timeScale = 1;
        OverworldCam.orthographicSize = f;
        yield break;
    }
    public static void OverworldStartBattle(Actor[] F, Map m, int map = 0)
    {
        GM.StartCoroutine(GM.BattleTransition(F,m,map));
    }
    public static void StartBattle(Actor[] F, Map m, int map = 0)
    {




        GM.CanInteract = false;
        GM.OverWorldGO.SetActive(false);

        if (IGA)
        {
            Overworld.PlayerPos = IGA.actor.TilePosition;
            IGA.gameObject.SetActive(false);
        }
        GM.Cam.enabled = true;
        GM.TextAndUI.worldCamera = GM.Cam;
        GM.Cursor.gameObject.SetActive(true);
        GM.onHoverGO.SetActive(true);
        map = Mathf.Clamp(map, 0, GM.Battlefields.Length - 1);
        for (int i = 0; i < GM.Battlefields.Length; i++)
            GM.Battlefields[i].Map.SetActive(false);
        GM.Battlefields[map].Map.SetActive(true);

        GM.audi.clip = GM.Battlefields[map].Sounds[0];
        GM.audi.Play();
        GM.OnHover.gameObject.SetActive(true);

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
        }


        for (int i = 0; i < GM.InGameActors.Count; i++)
            GM.InGameActors[i].actor.Teleport(CurrentBattle.map.AtPos(0, i));
        var mobsspawn = 0;
        var ymob = 0;
        for (int i = 0; i < GM.InGameFoes.Count; i++)
        {
            if (i % CurrentBattle.map.Width == 0) { ymob = 0; mobsspawn++; } 
            GM.InGameFoes[i].actor.Teleport(CurrentBattle.map.AtPos(CurrentBattle.map.Length -1 - mobsspawn,ymob));
            GM.InGameFoes[i].actor.Heal();
            if (GM.InGameFoes[i].actor is Monster)
            {
                var ggd = GM.InGameFoes[i].actor as Monster;
                CurrentBattle.BattleExp += ggd.ExpGain;
            }
            ymob++;
        }





        CursorPos = new Vector(9, 4);
        GM.ShowGrid = false;
        GM.ToggleGrid();

        CurrentBattle.StartNewTurn();


        GM.StartCoroutine(GM.BattleStart());

    }
    IEnumerator BattleStart()
    {
        var t = 0f;
        yield return new WaitForSeconds(.1f);

        BattleStartGameObject.localPosition = new Vector3(BattleStartGameObject.localPosition.x, 0);
        DarknessMyOldFriend.color = Color.black;
        foreach (var item in GM.InGameActors) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right;
        foreach (var item in GM.InGameFoes) item.transform.position = (Vector2)GameManager.Battlefied[(int)item.actor.TilePosition.x, (int)item.actor.TilePosition.y].transform.position + item.offset + Vector2.right;
        yield return new WaitForSeconds(.25f);









        var sp = new Consumeable("Orange Potion", "Items/SP_POTION")
        { rarity = Item.Rarity.Common, GoldValue = 10, Uses = 1, SPregen = 3, Description = "A Potion that feel special. Give 3 SP." };  //TODO alonso desc traduction
        Protags[1].Grab(sp);





        StartCoroutine(Transition(Color.clear));
        yield return new WaitForSeconds(1.0f);
        BattleStartGameObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(.2f);
        while (t < .4f)
        {
            t += Time.smoothDeltaTime;
            BattleStartGameObject.localPosition += Vector3.up * 150 * (1 + t * 50) * Time.smoothDeltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(.1f);
        CurrentBattle.Paused = false;
        CurrentBattle.Proceed();
        GM.CanInteract = true;
        BattleStartGameObject.gameObject.SetActive(false);




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
            Inventory[i] = Instantiate(InventoryCeil, CharacterInventory.transform).transform.GetChild(0).GetComponent<Image>();

        Skills = new GameObject[100];
        for (int i = 0; i < Skills.Length; i++)
            Skills[i] = Instantiate(SkillsPrefab, SkillList.transform);

        uiStatus.main.SetActive(false);
    }
    private void Awake()
    {
        if (!GM) GM = this;
        else Destroy(this.gameObject);
        LOG += "-" + System.Security.Principal.WindowsIdentity.GetCurrent().Name+ "" + System.DateTime.Now + "-\n";
        Protags = new List<Actor> 
    {
        new Player("Nana",new Stat{ AGI  =2 , END =1, INT =6, LUC =2 , STR = 1, WIS =5 }, true, "Mage")
        { inventory = Actor.Inventory.Light, Class = new Profession(new Stat(),Profession.ProfessionType.Mage),Description = "A being from the realm of Idea. It'll figuratively and literally take arms against evil. Dislike doing his taxes."},
        new Player("Mathew", new Stat{ STR = 16, AGI = 2, END =4, LUC =3 ,WIS = 1, INT = 0},true,"Barbarian")
        { inventory = Actor.Inventory.Light,Description = "A romantic fighter that seek his purpose in combat. Has a Master in Philosophy."}
    }; //TODO alonso desc traduction

        DontDestroyOnLoad(this.gameObject);
        audi = GetComponent<AudioSource>();


        // Protags[0].SetProfession(Profession.Madoshi);
        Language.Initialize();
    }

    public void Start()
    {

       
        GM.InitializeUI();
        GM.Cam.enabled = false;

        foreach (var item in Protags)
        {
            item.Heal();
        }
        GenerateOverworld(Main);
        TextAndUI.worldCamera = OverworldCam;
       
        //14 6
        //var nGroup = new List<Monster>();

        // for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
        //    nGroup.Add(new Monster("Kuku " + i, new Stat { AGI = 4, END = 3, LUC = 20, STR = 2 }, false, "~Kuku"));
        //   StartBattle(MonsterControllerFactory.SpawnMonsters(), new Map(new Vector(38, 9)), 0);
        

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



        // CreateNewItemOnField(Item.Gold, new Vector(2, 5));

        GM.onHoverGO.SetActive(false);
    }

    public static Events[,] EventList = new global::Events[1000, 1000];
    public static InGameActor IGA;
    public bool CanInteract = true;
    void GenerateOverworld(Tilemap r)
    {

        var e = new Vector(r.cellBounds.size.x, r.cellBounds.size.y);
        Map = new Overworld(e);

        foreach (var item in Events)
        {
            if (item.size.magnitude > Main.size.magnitude)
            {
                print(item.name + " invalid size, Resizing");
                Main.size = item.size;
                Main.ResizeBounds();
            }
            TilemapManager.LoadEvents(item);

        }
        if (Overworld.SpawnPoints.Count > 0)
        {
            var z = GenerateInGameActor(Protags[0]);
            z.actor.DefaultPos = Overworld.SpawnPoints[0];
            z.actor.Teleport(Map.AtPos(z.actor.DefaultPos));
            z.transform.localScale = Vector3.one * .75f;
            z.offset = new Vector2(-29f, -30.7f);
            z.Indicator.enabled = false;
            z.BattleSprite = false;
            IGA = z;
        }

        var ev1 = new TextBox(new Vector(28, 31), LanguageDao.GetLanguage("epic", language));
        AddEvent(ev1);
        UpdateEvents();
        OverWorldGO.SetActive(true);
    }
    public static void UpdateEvents()
    {
        for (int x = 0; x < Map.Width; x++)
        {
            for (int y = 0; y < Map.Length; y++)
            {

                Map.AtPos(x, y).Event = EventList[x, y];
            }
        }
    }
    public static void AddEvent(Events e)
    {
        EventList[(int)e.VID.x, (int)e.VID.y] = e;
        print("New event:" + "[" + e.Name + "] " + e.VID);


        /* redundant 
         * 
         * for (int x = 0; x < Map.Width; x++)
         {
             for (int y = 0; y < Map.Length; y++)
             {

                 if (new Vector(x,y) == e.VID) Map.AtPos(x, y).Event = EventList[x, y];
             }
         }*/
        UpdateEvents();
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
        GameEnd.anchorMax = new Vector2(0.5f, 1);
        GameEnd.anchorMin = new Vector2(0.5f, 1);
        GameEnd.anchoredPosition = Vector2.zero + Vector2.up * 25;

        GameEnd.gameObject.SetActive(true);

        spoils_Gold.text = CurrentBattle.GoldEarnedThisBattle.ToString("0000") + " " + LanguageDao.GetLanguage("gold", GameManager.language);
        var s = CurrentBattle.BattleTime; var m = 0;
        while ((s - 60) > 0)
        {
            s -= 60;
            m++;
        }
        spoils_BattleTime.text = LanguageDao.GetLanguage("battletime", GameManager.language) + " " + m.ToString("00") + ":" + s.ToString("00");
        spoils_BattleTime.enabled = false;
        spoils_Gold.enabled = false;
        spoils_grade.text = CurrentBattle.Grade;
        spoils_grade.enabled = false;
        GM.onHoverGO.SetActive(false);
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
        TextAndUI.worldCamera = OverworldCam;
        //------------------------------------------------------
        audi.Stop();
        OverWorldGO.SetActive(true);

        if (IGA)
        {
            IGA.actor.Teleport(Map.AtPos(Overworld.PlayerPos));
            IGA.gameObject.SetActive(true);

            IGA.actor.Defending = false;
            if (IGA.actor.HP <= 0) IGA.actor.HP = 1;
        }


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
        CanInteract = false;
        InfoBar.transform.parent.gameObject.SetActive(true);
        // StartCoroutine(_freezecam(.5f));
        yield return new WaitForSeconds(1f);
        InfoBar.transform.parent.gameObject.SetActive(false);
        CanInteract = true;
        yield break;
    }

    public Text Textbox;


    public static void ShowText(string t)
    {


        if (GM._lasttext != null)
            GM.StopCoroutine(GM._lasttext);
        GM._lasttext = GM._ShowText(t);
        GM.StartCoroutine(GM._lasttext);
    }
    IEnumerator _lasttext;
    IEnumerator _ShowText(string t)
    {
        CanInteract = false;
        Textbox.gameObject.GetComponentInChildren<Image>().enabled = false;
        Textbox.gameObject.transform.parent.gameObject.SetActive(true);
        Textbox.text = "";
        for (int i = 0; i < t.Length; i++)
        {

            if (t[i] == '\\')
            {
                if (i + 1 < t.Length)
                {
                    if (t[i + 1] == 'n')
                    {
                        Textbox.text += "\n";
                        continue;
                    }
                }
            }
            else if (t[i] == 'n')
            {
                if (i - 1 > 0)
                    if (t[i - 1] == '\\') continue;
            }

            Textbox.text += t[i];
            yield return new WaitForSeconds(.05f);
            if (t[i] == '.' || t[i] == '!' || t[i] == ',') yield return new WaitForSeconds(.1f);
        }
        Textbox.text = t;
        yield return new WaitForSeconds(.25f);
        Textbox.gameObject.GetComponentInChildren<Image>().enabled = true;
        while (!Input.anyKey)
        {
            yield return null;
        }
        Textbox.gameObject.transform.parent.gameObject.SetActive(false);
        Textbox.gameObject.GetComponentInChildren<Image>().enabled = false;
        CanInteract = true;
        yield break;



    }
    public static List<Vector> PathUI = new List<Vector>();

    /// <summary>
    /// Create an estimate Area of Effect at the position of the cursor.
    /// </summary>
    /// <param name="range">range.</param>
    /// <param name="cursorPos">Position.</param>
    /// <returns></returns>
    public static int EstimateAOE(int range, Vector cursorPos)
    {
        PathUI.Clear();


        int curX = (int)cursorPos.x;
        int curY = (int)cursorPos.y;

        for (int j = curY - range; j <= curY + range; j++)
        {

            if (j >= 0 && j <= CurrentBattle.map.Width)
            {

                for (int i = curX - range; i <= curX + range; i++)
                {
                    if (i >= 0 && i <= CurrentBattle.map.Length)
                    {

                        if ((Mathf.Abs(i - curX) + Mathf.Abs(curY - j)) < range)
                            PathUI.Add(new Vector(i, j));
                    }

                }
            }
        }
        for (int h = 0; h < Battlefied.GetLength(0); h++)
            for (int j = 0; j < Battlefied.GetLength(1); j++)
                foreach (var ff in Battlefied[h, j].Sprite)
                    ff.enabled = PathUI.Contains(Battlefied[h, j].tile.Position);



        //print("Using AOE Skill: Executing... Range:" + range + "   Distance:" + (int)Vector.Distance(cursorPos, SelectedActor.TilePosition));

        return (int)Vector.Distance(cursorPos, SelectedActor.TilePosition);
    }

 
    private Actor[] GetTargets()
    {
        List<Actor> targets = new List<Actor>();
        if (PathUI.Count > 0)
        {
            foreach (Vector tile in PathUI)
            {

                if (CurrentBattle.map.AtPos(tile).Actor != null)
                {
                    targets.Add(CurrentBattle.map.AtPos(tile).Actor);
                    print("Attacking : " + CurrentBattle.map.AtPos(tile).Actor.Name + "at" + tile);
                }
            }
        }
        else { return null; }


        return targets.ToArray();
    }
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


        GM.SpCostUI.text = ((int)(PathUI.Count / SelectedActor.GetStats.AGI)).ToString("00") + " " + LanguageDao.GetLanguage("sp", GameManager.language);

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


        GM.SpCostUI.text = ((int)(PathUI.Count / Whom.GetStats.AGI)).ToString("00") + " " + LanguageDao.GetLanguage("sp", GameManager.language);

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


        GM.SpCostUI.text = ((int)(PathUI.Count / Whom.GetStats.AGI)).ToString("00") + " " + LanguageDao.GetLanguage("sp", GameManager.language);

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

        OnHover.transform.parent.gameObject.SetActive(ActorAtCursor != null || SelectedActor != null || Tabmenu);
        CharacterInventory.gameObject.SetActive(ActorAtCursor != null || SelectedActor != null);
        if (Cursor.gameObject.activeSelf) Cursor.SetBool("Hover", ActorAtCursor != null || Tabmenu);
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
        else if (SelectedItem != null)
        {
            ShowUI(SelectedActor);
            if (PathUI.Count == 1) ChangeGridColor(Color.cyan);
            else ChangeGridColor(Color.cyan + Color.blue);

        }
        else if (SelectedSkill != null)
        {
            ShowUI(SelectedActor);
            if (SelectedSkill.Reach >= 0)
            {
                ChangeGridColor(Color.yellow);
            }
            else
            {

                ChangeGridColor(Color.yellow);

            }
        }
        else
        {
            if (SelectedActor != null) ShowUI(SelectedActor);
            ChangeGridColor(GridColor);
        }

        if (!CurrentBattle.ActingThisTurn.IsInPlayerTeam) ChangeGridColor(Color.gray);
        Cursor.gameObject.SetActive(CurrentBattle.ActingThisTurn.IsInPlayerTeam);
        TabButtons.SetActive(!Tabmenu && SelectedActor != null && SelectedActor == CurrentBattle.ThisTurn.Order[0]);
    }

    public Slider[] Bar;
    /// <summary>
    /// Show the On-Screen info about this actor
    /// </summary>
    /// <param name="a">Actor in question</param>
    public void ShowUI(Actor a)
    {

        if (a == null)
        {
            print(a + " IS NULL ");
            return;
        }
        /*OnHover.text =
"* " + a.Name + " *\n\nlvl"
+ a.GetLevel.ToString("00") + "\n[ hp  "
+ a.HP.ToString("00") + " ]\n[ mp "
+ a.MP.ToString("00") + " ]\n[ sp  "
+ a.SP.ToString("00") + " ]";*/ 

        OnHover.text = "[" + a.Name + "]" + "  " + LanguageDao.GetLanguage("lvl", GameManager.language) + " " + a.GetLevel;
        Bar[0].GetComponent<RectTransform>().sizeDelta = new Vector2(70 + a.HP * 2, 20);
        Bar[1].GetComponent<RectTransform>().sizeDelta = new Vector2(70 + a.MP * 2, 20);



        if (a.GetStats.MaximumMP == 0)
        {
            Bar[1].gameObject.SetActive(false);
        }
        else
        {
            Bar[1].gameObject.SetActive(true);
            Bar[1].value = a.MP / a.GetStats.MaximumMP;
        }
        Bar[0].value = a.HP / a.GetStats.MaximumHP;

        for (int i = 0; i < 100; i++)
        {
            if (i < a.inventory.items.Length)
            {

                if (a.inventory.items[i] != null)
                {
                    Inventory[i].transform.parent.gameObject.SetActive(true);
                    Inventory[i].enabled = true;


                    Inventory[i].sprite = LoadSprite(a.inventory.items[i].ResourcePath);

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
        else if (SelectedSkill != null)
        {
            if (SelectedSkill.Reach >= 0)
            {
                if (HasSelectedActor && curtile.Actor != null)
                {

                    if (GameManager.EstimathPath(SelectedActor, GameManager.CursorPos, 99) > SelectedSkill.Reach)
                    {
                        GiveInfo(LanguageDao.GetLanguage("cantreach", GameManager.language));
                        return;
                    }else
                    {
                        Tabmenu = false;
                        GetInGameFromActor(SelectedActor).UseSkill(curtile.Actor, SelectedSkill);
                        audiSFX.PlayOneShot(click);

                        CloseInventory();

                        return;
                    }

                }
                else if (!HasSelectedActor)
                {
                    GiveInfo(LanguageDao.GetLanguage("notargets", GameManager.language));
                    return;
                }
            }
            else if (GameManager.EstimateAOE(SelectedSkill.areaOfEffectRange, CursorPos) <= (SelectedSkill.Reach * -1))
            {
                print("Using AOE Skill:" + SelectedSkill.Name + " at " + SelectedActor.TilePosition);
                Tabmenu = false;
                GetInGameFromActor(SelectedActor).UseSkill(SelectedSkill, GetTargets());
                audiSFX.PlayOneShot(click);

                CloseInventory();

                return;
            }
            else
            {
                GiveInfo(LanguageDao.GetLanguage("cantreach", GameManager.language));
                return;
            }

        }

        if (Tabmenu) return;

        if (curtile.Actor != null && SelectedActor == null) { SelectedActor = curtile.Actor; audiSFX.PlayOneShot(click); return; }
        else if (SelectedActor == curtile.Actor) SelectedActor = null;

        if (HasSelectedActor)
        {
            if (CurrentBattle.ThisTurn.Order.Count > 0)
                if (GetInGameFromActor(SelectedActor).MyTurn && SelectedActor.Controllable)
                    if (curtile.Actor == null) SelectedActor.Move(curtile);
                    else if (curtile.Actor != null && SelectedActor.CanUseSkill(Skill.Base))
                    {
                        audiSFX.PlayOneShot(click);
                        if (SelectedActor.inventory.HasWeapon)
                            foreach (var item in SelectedActor.inventory.GetWeapons)
                            {
                                if (item != null) GetInGameFromActor(SelectedActor).Attack(curtile.Actor, Skill.Weapon(item));
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
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 5, 12 * Time.smoothDeltaTime);
        if (freezeCam) campos = Cam.transform.position;
        Cam.transform.position = Vector3.Lerp(Cam.transform.position, campos, 10 * Time.smoothDeltaTime);
    }



    private void Update()
    {


        //If there is no battle the scripts is hibernating, having no impact whatsoever on the performances  
        if (BattleMode) BattleModeLogic();
        else OverWordLogic();

    }

    void BattleModeLogic()
    {

        Cursorlogic();

        if (SelectedSkill != null)
        {
            if (SelectedSkill.Reach >= 0)
            {
                EstimathPath(SelectedActor, CursorPos, 99);
            }
            else
            {
                EstimateAOE(SelectedSkill.areaOfEffectRange, CursorPos);
            }
        }
        else
            EstimathPath(CursorPos);

        var curtile = CurrentBattle.map.AtPos(CursorPos);
        BattlePlayerInput(curtile);
        OnCursorUpdate(curtile);

        MiniMenuLogic();
        CameraUpdate();
    }
    void OverWordLogic()
    {
        var campos = IGA.transform.position;
        campos.z = Cam.transform.position.z;

        OverworldCam.transform.position = Vector3.Lerp(OverworldCam.transform.position, campos, 10 * Time.smoothDeltaTime);

        //For Smoother Effect, this is going to be in InGameActor
        /*
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        var inputs = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0);
        if (timer >= .11f && inputs)
        {
            var u = new Vector(h, v);
            var i = Protags[0].TilePosition + u;
            var e = new Vector(Mathf.Clamp(i.x, 0, Map.Width - 1), Mathf.Clamp(i.y, 0, Map.Length - 1));
            Protags[0].Move(Map.AtPos(e));
            timer = 0;
        }*/
    }
    void Cursorlogic()
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
                Position = SkillsCursorPos.transform.position;//Skills[skillUI].transform.position;

            if (SelectedItem != null || SelectedSkill != null) Position = GameManager.Battlefied[(int)CursorPos.x, (int)CursorPos.y].transform.position;
        }
        else CursorPos = CurrentBattle.ActingThisTurn.TilePosition;


        Cursor.transform.position = Vector3.Lerp(Cursor.transform.position, Position + CursorOffset, 9 * Time.smoothDeltaTime);

    }

    public static int GetDistance(Vector a, Vector b)
    {
        var e = 0;

        var f = a - b;
        e += (int)Mathf.Abs(f.x) + (int)Mathf.Abs(f.y);
        return e;
    }

    public void BattlePlayerInput(Map.Tile curtile)
    {

        if (!CanInteract) return;
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");


        var inputs = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0);
        if (CurrentBattle.IsPlayerTurn) if (timer >= .10f && inputs && (!Tabmenu || SelectedItem != null || SelectedSkill != null))
            {


                var u = new Vector(h, -v);

                if (HasSelectedActor)
                {

                    var dis = GetDistance(SelectedActor.TilePosition, CursorPos + u) - 1;

                    if (SelectedSkill != null)
                    {
                        var absoluteReach = Mathf.Abs(SelectedSkill.Reach);
                        if (dis >= (absoluteReach))
                        {
                            return;
                        }

                    }
                    else
                    {
                        if (dis >= ((SelectedActor.GetStats.MaximumSP * SelectedActor.GetStats.AGI) - SelectedActor.TileWalkedThisTurn))
                            return;
                    }


                }
                OnCursorExit(CurrentBattle.map.AtPos(CursorPos));


                CursorPos += u;
                timer = 0;
                audiSFX.PlayOneShot(select);
                OnCursorEnter(CurrentBattle.map.AtPos(CursorPos));
            }
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
            if (Input.GetKeyDown(KeyCode.Tab) && (!inventorySelected && !SkillsSelected && !uiStatus.main.activeSelf))
            {
                ToggleTabMenu();
            }
        }
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


        if (!BattleMode && !HasSelectedActor || (HasSelectedActor && SelectedActor != CurrentBattle.ActingThisTurn)) Tabmenu = false;
        if (SelectedActor == null) Tabmenu = false;
        TabButtons.SetActive(!Tabmenu && SelectedActor != null);
        MiniMenu.gameObject.SetActive(Tabmenu);
        inventorySelected = false;
        SkillsSelected = false;
        SkillList.SetActive(false);
        SelectedItem = null;
        if (!Tabmenu && HasSelectedActor) CursorPos = SelectedActor.TilePosition;
        TabChoice = 4;
        if (!Tabmenu)
            InfoBar.transform.parent.gameObject.SetActive(false);

        if (Tabmenu)
            audiSFX.PlayOneShot(click);




        if (!a) StartCoroutine(_freezecam(.20f));
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
    public GameObject[] MiniMenuBTN,OverWorldMenuBTN;
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
        StartCoroutine(_freezecam(1.0f));
    }

    void SkillDescOnInfo(Skill a)
    {
        InfoBar.transform.parent.gameObject.SetActive(true);
        InfoBar.text = a.Description;
    }
    void ShowItemDescInfo(Item a)
    {
        if (a == null) { InfoBar.transform.parent.gameObject.SetActive(false); return; }
        InfoBar.transform.parent.gameObject.SetActive(true);
        InfoBar.text = a.Description;
    }
    /// <summary>
    /// Logic about the menu - run on update
    /// </summary>
    void MiniMenuLogic()
    {

        if (!Tabmenu || SelectedActor == null) return;
        if (!uiStatus.main.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                audiSFX.PlayOneShot(select);
                invUIItem++; TabChoice--;
                skillUI--;
                skillUI = Mathf.Clamp(skillUI, 0, SelectedActor.Class.UsableSkill.Length - 1);
                if (SkillsSelected && SelectedSkill == null)
                {

                    SkillDescOnInfo(SelectedActor.Class.UsableSkill[skillUI]);
                }

                if (inventorySelected && SelectedItem == null)
                {
                    invUIItem = Mathf.Clamp(invUIItem, 0, SelectedActor.inventory.items.Length - 1);
                    ShowItemDescInfo(SelectedActor.inventory.items[invUIItem]);
                }

            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                audiSFX.PlayOneShot(select);
                invUIItem--; TabChoice++;
                skillUI++;
                skillUI = Mathf.Clamp(skillUI, 0, SelectedActor.Class.UsableSkill.Length - 1);
                if (SkillsSelected && SelectedSkill == null)
                {
                    SkillDescOnInfo(SelectedActor.Class.UsableSkill[skillUI]);
                }
                if (inventorySelected && SelectedItem == null)
                {
                    invUIItem = Mathf.Clamp(invUIItem, 0, SelectedActor.inventory.items.Length - 1);
                    ShowItemDescInfo(SelectedActor.inventory.items[invUIItem]);
                }

            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                audiSFX.PlayOneShot(select);
                invUIItem--;
                if (inventorySelected && SelectedItem == null)
                {
                    invUIItem = Mathf.Clamp(invUIItem, 0, SelectedActor.inventory.items.Length - 1);
                    ShowItemDescInfo(SelectedActor.inventory.items[invUIItem]);
                }

                if (TabChoice == 4) TabChoice = 0;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                audiSFX.PlayOneShot(select);
                invUIItem++;
                if (inventorySelected && SelectedItem == null)
                {
                    invUIItem = Mathf.Clamp(invUIItem, 0, SelectedActor.inventory.items.Length - 1);
                    ShowItemDescInfo(SelectedActor.inventory.items[invUIItem]);
                }

                if (TabChoice < 4) TabChoice = 4; ;
            }
        }


        if (TabChoice > 4) TabChoice = 0;
        if (TabChoice < 0) TabChoice = 4;
        if (skillUI < 0) skillUI = SelectedActor.Class.UsableSkill.Length - 1;
        if (skillUI > SelectedActor.Class.UsableSkill.Length - 1) skillUI = 0;


        invUIItem = Mathf.Clamp(invUIItem, 0, SelectedActor.inventory.items.Length - 1);
        skillUI = Mathf.Clamp(skillUI, 0, SelectedActor.Class.UsableSkill.Length - 1);
        //Hard coded, for getting less fields
        SkillList.transform.localPosition = Vector3.Lerp(SkillList.transform.localPosition, new Vector3(-205.8f, -41 + skillUI * 82), 15 * Time.smoothDeltaTime);


        if (Tabmenu)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (uiStatus.main.activeSelf)
                {
                    uiStatus.main.SetActive(false);
                    
                }

                if (inventorySelected)
                {


                    if (SelectedItem != null) { ShowItemDescInfo(SelectedItem); SelectedItem = null; }
                    else if (inventorySelected) { inventorySelected = false; InfoBar.transform.parent.gameObject.SetActive(false); } 
                    else CloseInventory();
                }
                else if (SkillsSelected)
                {


                    if (SelectedSkill != null)
                    {
                        SkillList.SetActive(true);
                        InfoBar.transform.parent.gameObject.SetActive(true);
                        InfoBar.text = SelectedSkill.Description;
                        SelectedSkill = null;

                    }
                    else if (SkillsSelected)
                    {
                        SkillList.SetActive(false);
                        SkillsSelected = false;
                        InfoBar.transform.parent.gameObject.SetActive(false);
                    }
                    else CloseInventory();

                }
                


            }


        if (SelectedSkill == null && SelectedItem == null)
            if (Input.GetKeyDown(KeyCode.Space) && Tabmenu )
            {
                if (inventorySelected && SelectedActor.inventory.items[invUIItem] != null && SelectedItem == null)
                {
                    var e = SelectedActor.inventory.items[invUIItem];
                    SelectedItem = e;
                    ShowItemDescInfo(SelectedActor.inventory.items[invUIItem]);
                    audiSFX.PlayOneShot(click);
                    return;
                }
                else
                if (SkillsSelected && SelectedActor.Class.UsableSkill[skillUI] != null && SelectedSkill == null)
                {
                    var e = SelectedActor.Class.UsableSkill[skillUI];
                    if (PathUI.Count > 0)
                        PathUI.Clear();

                    audiSFX.PlayOneShot(click);

                    StartCoroutine(_freezecam(.25f));
                    SelectedSkill = e;
                    CursorPos = SelectedActor.TilePosition;
                    SkillList.SetActive(false);

                    InfoBar.text = LanguageDao.GetLanguage("selecttarget", GameManager.language);
                    if (e.Targets == Skill.TargetType.Self)
                        InfoBar.text = LanguageDao.GetLanguage("applyyou", GameManager.language);
                    if (e.Targets == Skill.TargetType.AnAlly)
                        InfoBar.text = LanguageDao.GetLanguage("applyally", GameManager.language);
                    if (e.Targets == Skill.TargetType.Enemy || e.Targets == Skill.TargetType.Anyone)
                        InfoBar.text = "Select targets";
                    if (e.Targets == Skill.TargetType.Ally)
                        InfoBar.text = "Select allies";



                    return;
                }
                if (TabChoice == 0)
                {
                    skillUI = 0;

                    audiSFX.clip = click;
                    audiSFX.Play();

                    SkillDescOnInfo(SelectedActor.Class.UsableSkill[skillUI]);
                    SkillsSelected = true;
                    for (int i = 0; i < Skills.Length; i++)
                    {
                        if (i < SelectedActor.Class.UsableSkill.Length)
                        {
                            Skills[i].GetComponent<InGameSkill>().ShowSkill(SelectedActor.Class.UsableSkill[i], SelectedActor);
                            Skills[i].gameObject.SetActive(true);
                        }
                        else Skills[i].gameObject.SetActive(false);
                    }
                    SkillList.SetActive(true);
                }
                else if (TabChoice == 1) {
                    inventorySelected = true;

                    audiSFX.PlayOneShot(click); invUIItem = 0;
                    if (SelectedActor.inventory.items.Length > 0) ShowItemDescInfo(SelectedActor.inventory.items[invUIItem]);
                }
                else if (TabChoice == 2 && SelectedActor.SP > 0)
                {
                    audiSFX.PlayOneShot(click);
                    GetInGameFromActor(SelectedActor).EnterDefenseMode();
                    ShowTabMenu(false);

                }

                else 
                if (TabChoice == 3 && SelectedActor.SP > 0)
                {
                    audiSFX.PlayOneShot(click);
                    uiStatus.main.SetActive(false);
                    GetInGameFromActor(SelectedActor).EndTurn();
                    ShowTabMenu(false);
                }
                else if(TabChoice == 4 && !uiStatus.main.activeSelf)
                {
                    audiSFX.PlayOneShot(click);
                    uiStatus.GetInfo(SelectedActor);
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
        InfoBar.transform.parent.gameObject.SetActive(false);   
        invUIItem = 0;
        uiStatus.main.SetActive(false);
    }


    /// <summary>
    /// Return true if there is a actor selected
    /// </summary>
    public bool HasSelectedActor
    {
        get { return SelectedActor != null; }
    }
}
