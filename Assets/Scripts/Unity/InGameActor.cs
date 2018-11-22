using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}


public class InGameActor : MonoBehaviour {

    public string Name;
    public Text bar;
    public Color ActorColor;
    public SpriteRenderer Indicator;
    public Vector2 Position;
    public Actor actor;
    public Animator[] anim;
    public SpriteRenderer[] sprity;
    public Image ExpBar;
    public float StepDuration = 0.1f;
    public float Speed = 5;
    public Vector2 offset;
    public bool InverseSprite = false;
    public float DistanceToPos;
    public bool isAI = false;
    private Canvas cv;

    [Header("Equipements")]
    public Transform[] Wep;


    public bool MyTurn = false;
    // private Skill SkillToUse = null;

    float AITImer = 0;

   
    public InGameActorStats ActorStats;
    [System.Serializable]
    public struct InGameActorStats
    {

        public int AGI, STR, INT, LUC, WIS, END;
        public InGameActorStats(Stat s)
        {
            AGI = s.AGI;
            STR = s.STR;
            INT = s.INT;
            LUC = s.LUC;
            WIS = s.WIS;
            END = s.END;
            EXPGain = 0;
        }
        public float EXPGain;
    }
    public bool BattleSprite = true;

    public static Actor[] ToActors(InGameActor[] s)
    {
        var e = new Actor[s.Length];
        for (int i = 0; i < e.Length; i++)
            e[i] = s[i].actor;
        return e;

    }
    private void Awake()
    {
        cv = GetComponentInChildren<Canvas>();
  


    }

    private void Start()
    {
        ExpBar.transform.parent.gameObject.SetActive(false);
        ExpBar.fillAmount = 0;
 
        //StartCoroutine(UpDateEXP());
    }
    bool attacking = false;

    void StupidAI()
    {
        if (!isAI || !MyTurn) return;
        AITImer += Time.fixedDeltaTime;
        if (AITImer > .5f) EndTurn();

    }
    //Action and Attack
    public void AI(Battle.Turn Turn = null)
    {


        if (!MyTurn) return;

        
        if(AITImer > .3f && actor.Path.Count <= 1)
        {
            var e = GameManager.GM.InGameActors[Random.Range(0, GameManager.Protags.Count)].actor;
        
                  Attack(e, Skill.Base);
        }
      

    }


    Actor cachedactor; Item cacheditem; public SpriteRenderer OnActorItem;

    public void UseItem(Actor to, Item t)
    {
        if (anim[0].GetCurrentAnimatorStateInfo(0).IsName("UseItem")) return;

        anim[0].SetTrigger("UseItem");

        GameManager.GM.ShowTabMenu(false);


        /* SHOW, DONT TELL.  THIS SHOULD BE REMOVE 
         * 
         * 
         * 
        if(to == actor)  GameManager.GiveInfo(actor.Name + " uses " + t.Name + " on itself.");
        else GameManager.GiveInfo(actor.Name + " uses " + t.Name + " on " + to.Name);*/
        cachedactor = to;
        cacheditem = t;
        if (OnActorItem != null) OnActorItem.sprite = GameManager.LoadSprite(t.ResourcePath);
    }



    public void UseSkill(Actor to, Skill s)
    {
        var r = s.Targets;
        if (!actor.CanUseSkill(s)) { Error("Not enough ressource"); return; } //TODO Language.db

        if ((r == Skill.TargetType.AnAlly) && (!GameManager.CurrentBattle.IsTeamWith(actor, to) || to == this.actor)) { Error("Can only Target an ally"); return; }
        if ((r == Skill.TargetType.Enemy || r == Skill.TargetType.OneEnemy) && (GameManager.CurrentBattle.IsTeamWith(actor, to) || to == actor)) { Error("Can only target a enemy"); return; }
        if (r == Skill.TargetType.Self && to != actor) { Error("Can only target yourself"); return; }


        TurnSprite((to.TilePosition - actor.TilePosition).x < 0);
        GameManager.GM.ActionFreeze();
        actor.Use(s, to);

        GameManager.GM.ShowTabMenu(false);

    }
    void Error(string s)
    {
        GameManager.GiveInfo(s);
    }

    public void AnimatorUseItem()
    {
        GameManager.GM.ShowUI(cachedactor);
        actor.Use(cacheditem, cachedactor);
        actor.ConsumeSP(1);
        GameManager.SetActor(actor);

    }

    /// <summary>
    /// Is called once at the start of the turn
    /// </summary>
    /// <param name="Turn"></param>
    public void OnTurn(Battle.Turn Turn)
    {
        if (!gameObject.activeSelf) return;

        actor.TileWalkedThisTurn = 0;

        if (sprity[0] != null)
            sprity[0].color = Color.white;
        MyTurn = true;

        if (!isAI)
        {
            GameManager.CursorPos = actor.TilePosition;
            GameManager.GM.OnPressed(actor.CurrentTile);


        }
        else AI(Turn);


    }


    public bool CanPerformAction(Skill s)
    {
        
        if (actor.HP < s.HpCost) { print("Not enough HP: " + actor.HP + "/" + s.HpCost); return false; }
        if (actor.MP < s.MpCost) { print("Not enough MP: " + actor.MP + "/" + s.MpCost); return false; }
        if (actor.SP < s.SpCost) { print("Not enough SP: " + actor.SP + "/" + s.SpCost); return false; }
        
        return true;
    }

    public void Attack(Actor a, Skill b)
    {
        if (MyTurn)
        {
            normattack = InitiateAttack(a, b);
            StartCoroutine(normattack);
        }
        AITImer = 0;

    }
    /// <summary>
    /// Used by the Animation. Will Ends the Turn
    /// </summary>
    public void AnimatedAttack()
    {
        if (!MyTurn) return;
        GameManager.GM.Cam.orthographicSize = 5.2f;
        AITImer = 0;
        Actor[] e = new Actor[1];
        e[0] = temptarget;
        actor.Use(tempattack, e);
        TimeSincedAttack = 0;
        AITImer = 0;

    }
    public void StopAnimatedAttack()
    {
         
        if (!MyTurn) return;
        AITImer = 0;
        GameManager.GM.ActionFreeze();
        if (actor.SP <= 0) EndTurn();
        StopCoroutine(normattack);
    }

    void StopAttacking()
    {
        if (normattack != null)
        {
            StopCoroutine(normattack);
            attacking = false;
            tempattack = null;
            temptarget = null;
        }
    }
    /// <summary>
    /// Ends the turn officially. Do not use _OnTurn
    /// </summary>

    public void EndTurn()
    {
        MyTurn = false;

        StopAttacking();
        StartCoroutine(_EndTurn());
    }

    public void EnterDefenseMode()
    {

        actor.SP--;
        actor.Defending = true;
        EndTurn();
    }
    private IEnumerator _EndTurn()
    {
        yield return new WaitForSeconds(.5f);
        if (isAI) yield return new WaitForSeconds(.5f);

        actor.Path.Clear();

        if (actor.IsDefeat) yield break;
    
        sprity[0].color = Color.gray;
        timeSinceTurn = 0;
        attacking = false;
        print(actor.Name + " " + " ends his turn.");
        GameManager.SelectedActor = null;
        GameManager.CurrentBattle.EndTurn();
    
        yield break;
    }
    IEnumerator UpDateEXP()
    {

        var e = 0f;
        ExpBar.transform.parent.gameObject.SetActive(true);
       
        while (e < 2 )
        {
            ExpBar.fillAmount = Mathf.Lerp(ExpBar.fillAmount, (actor.GetEXP / actor.RequiredEXP) + .05f, 10 * Time.smoothDeltaTime);

      
            e += Time.fixedDeltaTime;
            yield return null;
        }
        print(actor.Name + " lvl" + actor.GetLevel + " "+ actor.GetEXP + " / " + actor.RequiredEXP);
        ExpBar.transform.parent.gameObject.SetActive(false);
        yield break;
    }
    private Skill tempattack;
    Actor temptarget;

    IEnumerator normattack;
    public IEnumerator InitiateAttack(Actor a, Skill b)
    {

        if (!GameManager.BattleMode) yield break;
        if (attacking) yield break;
        if (!CanPerformAction(b))
        {
            EndTurn();
            yield break;
        }


            if (!MyTurn ||  actor.IsDefeat) {
            attacking = false;
            yield break;
        }

        AITImer = 0;
        attacking = true;
        tempattack = b;
        temptarget = a;
        actor.Move(a.TilePosition, true);
 

        foreach (var item in actor.Path)
        {
            
            if (GameManager.CurrentBattle.map.AtPos(item).Actor != null)
            {
              
                var f = GameManager.CurrentBattle.map.AtPos(item).Actor;
                
                if (f == actor) continue;
                if (f == a) continue;
                
               
                if ((f != actor && f != a) || f.IsTeamWith(actor))
                {
           
                    Error(f.Name + " is blocking " + name); //TODO language.db
                    attacking = false;
                    tempattack = null;
                    temptarget = null;
                    StopAttacking();
                    
                    yield break;
                }
                

            }
      
        }
        TurnSprite((temptarget.TilePosition - actor.TilePosition).x < 0);

        // while (Vector.Distance(actor.TilePosition, a.TilePosition) > b.Reach)
        //while (GameManager.EstimathPath(actor,a.TilePosition) > b.Reach)
        print(actor.Path.Count - b.Reach);
        while (actor.Path.Count > b.Reach)
        {

            if (!MyTurn)
            {
                attacking = false;
                print("RAN OUT OF TIME!");
                yield break;
            }
            yield return null;
        }
        TurnSprite((temptarget.TilePosition - actor.TilePosition).x < 0);
        yield return new WaitForSeconds(.3f);

        if (actor.CanUseSkill(b))       
            foreach (var item in anim) item.SetTrigger(b.DmgType.ToString());
        attacking = false;
        yield break;
    }


    public List<InGameWeapon> IGW= new List<InGameWeapon>();
    //Actor Related - Can Swap Actor on a whim

    AnimatorOverrideController animcont;
    AnimationClipOverrides clipov;
    public void InitializedActor(Actor a, string ActorName, RuntimeAnimatorController b =null  )
    {
        
        actor = a;
        a.OnTurn += OnTurn;
        a.OnExpGain += OnExpGain;
        a.OnDamage += OnDamage;
        a.OnKillActor += OnKillingSomeone;
        a.OnEquip += OnEquip;
        a.OnBlocked += OnBlocked;
        Indicator.color = ActorColor;


       

        isAI = !actor.Controllable;
        if (a.AnimatorPath.Contains("~")) InverseSprite = true;
        this.name = actor.Name;
        Name = actor.Name;
        if (b) foreach (var item in anim) item.runtimeAnimatorController = b;
        string[] amn = new string[b.animationClips.Length];
   

        animcont = new AnimatorOverrideController(anim[0].runtimeAnimatorController);
        anim[0].runtimeAnimatorController = animcont;
        clipov = new AnimationClipOverrides(animcont.overridesCount);
        animcont.GetOverrides(clipov);
        for (int i = 0; i < animcont.runtimeAnimatorController.animationClips.Length; i++)
        {
            amn[i] = animcont.runtimeAnimatorController.animationClips[i].name;
            clipov[amn[i]] = Resources.Load<AnimationClip>("Sprites/Animation/Actors/" + ActorName + "/" + amn[i]);

        }



        animcont.ApplyOverrides(clipov);

        if (a.inventory.HasWeapon)
        {
            for (int i = 0; i < a.inventory.GetWeapons.Count; i++)
                if (i < Wep.Length) OnEquip(a.inventory.GetWeapons[i]);
                    

        }

    }

    private void OnBlocked(float z, Skill x)
    {
        if (!gameObject.activeSelf) return;

        StartCoroutine(ColorBlink(Color.cyan, .1f));
        StartCoroutine(ShowDamage(0));
    }

    private void OnEquip(Equipement e)
    {
        if (!gameObject.activeSelf) return;

        if (e is Weapon)
        {
 
            for (int i = 0; i < Wep.Length; i++)
            {
 
                if (Wep[i].childCount == 0)
                {
                    var g = InGameWeapon.GenerateInGameWeapon(e as Weapon).GetComponent<InGameWeapon>();


                  IGW.Add(  g);
                    g.transform.parent = Wep[i].transform;
                    g.transform.localPosition = Vector3.zero;
                    g.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    g.transform.localScale = Vector3.one;
                    break;
                }
          
            }
           
        }
    }

    private void OnDestroy()
    {
        actor.OnTurn -= OnTurn;
        actor.OnExpGain -= OnExpGain;
        actor.OnDamage -= OnDamage;
        actor.OnKillActor -= OnKillingSomeone;
        actor.OnEquip -= OnEquip;
        actor.OnBlocked -= OnBlocked;
    }

    IEnumerator ColorBlink(Color c, float x)
    {
        var e = new Color[sprity.Length];
        
        for (int i = 0; i < sprity.Length; i++)
            e[i] = sprity[i].color;

        foreach (var item in sprity)
        {
            item.color = c;
        }
        yield return new WaitForSeconds(x/2);
        var v = 0f;
        while (v < x/2)
        {
            for (int i = 0; i < sprity.Length; i++)
                sprity[i].color = Color.Lerp(sprity[i].color, e[i], 5* Time.smoothDeltaTime);

            v += Time.fixedDeltaTime;
            yield return null;
        }
    
      

        for (int i = 0; i < sprity.Length; i++)
            sprity[i].color = e[i];

        yield break;
    }
    private void OnKillingSomeone(Actor a)
    {
        if (!gameObject.activeSelf) return;

        GameManager.CursorPos = a.TilePosition;
        GameManager.GM.ActionFreeze();

    }

    private void OnExpGain(float x)
    {
        if (!gameObject.activeSelf) return;
        StartCoroutine(UpDateEXP());
    }

    public GameObject UIPrefab;
    private void OnDamage(float z, Skill x)
    {
        if (!gameObject.activeSelf) return;

        if (!actor.IsDefeat)
        anim[0].SetTrigger("Attacked");          
        else anim[0].SetTrigger("IsDeath");
        StartCoroutine(ColorBlink(Color.red,.1f));
        StartCoroutine(ShowDamage(z));

    }
    IEnumerator ShowDamage(float z)
    {
        float x = 0;
        var b = Instantiate(UIPrefab, cv.transform ).GetComponent<Text>();
        b.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        b.transform.position += Vector3.down;

        if (z == 0) b.color = Color.cyan;
        else if (z > 0) b.color = Color.white;
        else if (z < 0) b.color = Color.green;
        
        b.text = z.ToString("0");
        while (x < 1f)
        {
            b.transform.position += Vector3.up * Time.smoothDeltaTime;
            x += Time.fixedDeltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(.5f);
        Destroy(b);
        yield break;
    }

    public void SetDeath()
    {
         
        foreach (var item in sprity)
            item.enabled = false;

        this.gameObject.SetActive(false);
   
        if (GameManager.CurrentBattle.Foes.Count == 0) GameManager.CurrentBattle.EndTurn();


    }

    public void InitializedActor(Actor a, string path = "")
    {
     
        InitializedActor(a, actor.AnimatorPath,(RuntimeAnimatorController)Resources.Load<RuntimeAnimatorController>(path) );
  
    }


    float timer = 0;float timeSinceTurn = 2;float TimeSincedAttack = 0;
    private void FixedUpdate()
    {
        inputtimer += Time.fixedDeltaTime;
        timer += Time.fixedDeltaTime;
        if (MyTurn)
        {
            timeSinceTurn += Time.fixedDeltaTime;
        }
        TimeSincedAttack += Time.fixedDeltaTime;
  
        if(MyTurn)
        StupidAI();

        if(actor!=null)
        ActorStats = new InGameActorStats(actor.GetStats);
    }

    public void BattleOnEnterTile()
    {
        
        transform.position = (Vector2)GameManager.Battlefied[(int)actor.TilePosition.x, (int)actor.TilePosition.y].transform.position + offset;

    
        //Check for next tile
        while (actor.Path.Count > 0)
        {

            if (timer < .14f) return;
            AITImer = 0;
            var target = GameManager.CurrentBattle.map.AtPos(actor.Path.Peek());



            if (target.Actor != null && target.Actor != actor  )
            {
             
                actor.CantMove(actor.Path.Peek());


                //Since PathFinding Is dumb, AI skip turn on stuck
                if (isAI && MyTurn)
                {
                    if (temptarget != null && tempattack != null && target.Actor == temptarget)
                    {
                        if (actor.CanUseSkill(tempattack))
                            foreach (var item in anim) item.SetTrigger(tempattack.DmgType.ToString());
                        else EndTurn();
                        attacking = false;
                    }
                    else EndTurn();
                } 
                    timer = 0;
                return;
            }

            var c = GameManager.CurrentBattle.map.AtPos(actor.Path.Peek());
            if (actor.SP > 0 && c.Actor == null)
            {
                
                actor.CurrentTile.OnQuitting();
                actor.CurrentTile = GameManager.CurrentBattle.map.AtPos(actor.Path.Dequeue());
                actor.CurrentTile.Enter(actor);
            }
          

            timer = 0;
        }

    }
    public void OverWorldOnEnterTile()
    {
       
        transform.position = (Vector2)GameManager.GM.Main.GetCellCenterWorld(new Vector3Int((int)actor.TilePosition.x, (int)actor.TilePosition.y, 0)) + offset;
        while (actor.Path.Count > 0)
        {

      //      if (timer < .01f) return;
            AITImer = 0;
            var target = GameManager.Map.AtPos(actor.Path.Peek());
            if (target.Actor != null && target.Actor != actor)
            {

                actor.CantMove(actor.Path.Peek());
                timer = 0;
                return;
            }
 
            if (GameManager.Map.AtPos(actor.Path.Peek()).Actor == null)
            {

                var b = actor.CurrentTile.Position;
                actor.CurrentTile.OnQuitting();
                actor.CurrentTile = GameManager.Map.AtPos(actor.Path.Dequeue());
                actor.CurrentTile.Enter(actor);
                b -= actor.CurrentTile.Position;
                b = new Vector(-b.x,-b.y);
                b += actor.CurrentTile.Position;
                tileInFront = GameManager.Map.AtPos(new Vector(Mathf.Clamp(b.x, 0, GameManager.Map.Width - 1), Mathf.Clamp(b.y, 0, GameManager.Map.Length - 1)));
            }
   

            timer = 0;
        }

    }
    public void BattleModeSprite()
    {
     

            BattleTile e = GameManager.Battlefied[(int)actor.TilePosition.x, (int)actor.TilePosition.y];
        var walking = DistanceToPos > 0f;
        foreach (var item in anim)
            item.SetBool("Walking", walking);
        anim[0].SetBool("Defend", actor.Defending);



        var g = transform.position.x - e.transform.position.x;
        if (!InverseSprite)
        {
            if (g > 0) transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
            else if (g < 0)transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {

            if (g > 0) transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            else if (g < 0) transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        for (int i = 0; i < sprity.Length; i++)
        {
            var item = sprity[i];
 
            if (i > 0)
            {
                item.sprite = sprity[0].sprite;
                item.flipX= sprity[0].flipX;
            }
        }
        Position = new Vector2(actor.TilePosition.x, actor.TilePosition.y);
        DistanceToPos = (Vector3.Distance(transform.position, e.transform.position + new Vector3(offset.x, offset.y)) - 90.9f) * 100;
        var x = 1;
         
        this.transform.position = Vector3.Lerp(transform.position, (Vector2)e.transform.position + offset, Speed * x* Time.smoothDeltaTime / (DistanceToPos + .1f));
         if (DistanceToPos <= 0.0025f) BattleOnEnterTile();


        sprity[0].sortingOrder = 2 + (int)actor.TilePosition.y;

        Indicator.gameObject.SetActive(GameManager.SelectedActor == actor);
        Indicator.transform.rotation = Quaternion.Euler(Vector3.zero);
        Indicator.transform.position = e.transform.position;
    }



    float inputtimer;
    Map.Tile tileInFront;
 
    public void OverWorldSprite()
    {

        var map = GameManager.Map;
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        var inputs = (Mathf.Abs(h) > 0.2f || Mathf.Abs(v) > 0.2f);
        if (inputs && inputtimer > .1f && GameManager.GM.CanInteract)
        {
            
            var u = new Vector(h, v);
 
           var i = GameManager.Protags[0].TilePosition + u;
            var q = new Vector(Mathf.Clamp(i.x, 0, map.Width - 1), Mathf.Clamp(i.y, 0, map.Length - 1));
        
            GameManager.Protags[0].Move(map.AtPos(q));
     

            tileInFront = map.AtPos(new Vector(Mathf.Clamp(i.x, 0, map.Width - 1), Mathf.Clamp(i.y, 0, map.Length - 1)));




            inputtimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (tileInFront != null)
            {
                print(tileInFront);
                tileInFront.OnPressed(actor);
            } 
            map.AtPos(actor.TilePosition).OnPressed(actor);

        }
        var e =  new Vector3Int((int)actor.TilePosition.x, (int)actor.TilePosition.y,0);
        var cp = GameManager.GM.Main.GetCellCenterWorld(e);
        DistanceToPos = Vector3.Distance(transform.position, (Vector2)cp + offset);

        var t = transform.position.x - ((Vector2)cp + offset).x;
        if (!InverseSprite)
        {
            if (t > 0) transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            else if (t < 0) transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {

            if (t > 0) transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            else if (t < 0) transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        sprity[0].sortingOrder = 2 + (int)actor.TilePosition.y;
        Position = new Vector2(actor.TilePosition.x, actor.TilePosition.y);
 
        var g = transform.position.x - e.x;

        var walking = DistanceToPos > 0f;

        foreach (var item in anim)
            item.SetBool("Walking", walking);

        anim[0].SetBool("Defend", actor.Defending);

        for (int i = 0; i < sprity.Length; i++)
        {
            var item = sprity[i];

            if (i > 0)
            {
                item.sprite = sprity[0].sprite;
                item.flipX = sprity[0].flipX;
            }
        }
         this.transform.position = Vector3.Lerp(transform.position, (Vector2)cp + offset, Speed* Time.smoothDeltaTime / (DistanceToPos + .1f));
        if (DistanceToPos <= .06f) OverWorldOnEnterTile();

        //  this.transform.position = Vector3.Lerp(transform.position, (Vector2)tilepos + offset, Speed * Time.fixedDeltaTime);



    }
    public void TurnSprite(bool x)
    {

        if (!InverseSprite)
        {
            if (x) transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            else   transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {

            if (x) transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            else  transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        /* for (int i = 0; i < sprity.Length; i++)
         {
             var item = sprity[i];
             if (InverseSprite)
                 x = !x;
            item.flipX = x;     
         }*/
    }
        private void Update()
    {

        if (actor == null) return;

        if (GameManager.BattleMode)
            BattleModeSprite();
        else if(!BattleSprite)
            OverWorldSprite();
        bar.text = actor.Name;


        if (actor.SP > 0 && MyTurn && isAI && !attacking) AI();
        if (MyTurn && timeSinceTurn > 1 && TimeSincedAttack > 1) if (actor.SP <= 0) EndTurn();
         
    }


}
