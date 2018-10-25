using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public bool MyTurn = false;
    // private Skill SkillToUse = null;

    float AITImer = 0;

    public bool OverrideStats = false;
    public InGameActorStats ActorStats;
    [System.Serializable]
    public struct InGameActorStats
    {

        public int AGI, STR, INT, LUC, WIS, END;
        public InGameActorStats(stat s)
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
        //Debug
       /* if (OverrideStats)
        {
            if (!isAI)
                InitializedActor(new Player(Name, new stat { AGI = ActorStats.AGI, STR = ActorStats.STR, LUC = ActorStats.LUC, END = ActorStats.END, INT = ActorStats.INT, WIS = ActorStats.WIS }, isAI), "");
            else
            {
                var e = new Monster(Name, new stat { AGI = ActorStats.AGI, STR = ActorStats.STR, LUC = ActorStats.LUC, END = ActorStats.END, INT = ActorStats.INT, WIS = ActorStats.WIS }, isAI);
                InitializedActor(e, "");
                e.ExpGain = ActorStats.EXPGain;
            }
        }
        else
        {
            if (!isAI)
                InitializedActor(new Player(Name, new stat { AGI = 2, STR = 6, INT = 5, LUC = 5, WIS = 5, END = 1 }, isAI), "");
            else
                InitializedActor(new Monster(Name, new stat { AGI = 4, STR = 1, INT = 1, LUC = 1, WIS = 1, END = 1 }, isAI), "");
        }*/

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
        if (AITImer > .4f) EndTurn();

    }
    //Action and Attack
    public void AI(Battle.Turn Turn = null)
    {

        
        if (!MyTurn) return;

        AITImer = 0;
        Attack(GameManager.GM.InGameActors[Random.Range(0,GameManager.Protags.Count)].actor, Skill.Base);

    }


    Actor cachedactor;Item cacheditem; public SpriteRenderer OnActorItem;

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
        if (!Actor.CanUseSkill(s, actor)) { Error("Not enough ressource");return; }

        if ((r == Skill.TargetType.AnAlly) && (!GameManager.CurrentBattle.IsTeamWith(actor, to) || to == this.actor)) { Error("Can only Target an ally"); return; }
        if ((r == Skill.TargetType.Enemy || r == Skill.TargetType.OneEnemy) && (GameManager.CurrentBattle.IsTeamWith(actor, to) || to == actor)) { Error("Can only target a enemy"); return; }
        if (r == Skill.TargetType.Self && to != null) { Error("Can only target yourself"); return; }


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

        actor.TileWalkedThisTurn = 0;

       if(sprity[0]!= null)
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

          
    }
    /// <summary>
    /// Used by the Animation. Will Ends the Turn
    /// </summary>
    public void AnimatedAttack()
    {
        if (!MyTurn) return;
        AITImer = 0;
        Actor[] e = new Actor[1];
        e[0] = temptarget;

        actor.Use(tempattack, e);
        TimeSincedAttack = 0;
        if (actor.SP <= 0) EndTurn();


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
        if (!CanPerformAction(b) || attacking) yield break;
       
        if (!MyTurn ||  actor.IsDefeat) {
            attacking = false;
            yield break;
        }

        AITImer = 0;
        attacking = true;
        tempattack = b;
        temptarget = a;
        actor.Move(a.TilePosition, true);
       
        foreach (var item in GameManager.PathUI)
        {
            if (GameManager.CurrentBattle.map.AtPos(item).Actor != null)
            {
                var f = GameManager.CurrentBattle.map.AtPos(item).Actor;
                if (f == actor) continue;
                if (f == a) continue;

                if (f != actor || f != a)
                {
                    attacking = false;
                    tempattack = null;
                    temptarget = null;
                    yield break;
                }
                

            }
      
        }
        // while (Vector.Distance(actor.TilePosition, a.TilePosition) > b.Reach)
        while (GameManager.EstimathPath(actor,a.TilePosition) > b.Reach)
        {
            
            if (!MyTurn)
            {
                attacking = false;
                yield break;
            }
            yield return null;
        }
        TurnSprite((temptarget.TilePosition - actor.TilePosition).x < 0);
        yield return new WaitForSeconds(.3f);

        if (Actor.CanUseSkill(Skill.Base, actor))       
            foreach (var item in anim) item.SetTrigger(b.Type.ToString());
        attacking = false;
        yield break;
    }


    //Actor Related - Can Swap Actor on a whim
    public void InitializedActor(Actor a, RuntimeAnimatorController b =null )
    {
        
        actor = a;
        a.OnTurn += OnTurn;
        a.OnExpGain += OnExpGain;
        a.OnDamage += OnDamage;
        a.OnKillActor += OnKillingSomone;
        Indicator.color = ActorColor;

        isAI = !actor.Controllable;
        if (a.AnimatorPath.Contains("~")) InverseSprite = true;
        this.name = actor.Name;
        Name = actor.Name;
        if (b) foreach (var item in anim) item.runtimeAnimatorController = b;
        
 
     
    }

    private void OnDestroy()
    {
        actor.OnTurn -= OnTurn;
        actor.OnExpGain -= OnExpGain;
        actor.OnDamage -= OnDamage;
        actor.OnKillActor -= OnKillingSomone;
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
    private void OnKillingSomone(Actor a)
    {
        GameManager.CursorPos = a.TilePosition;

    }

    private void OnExpGain(float x)
    {
        StartCoroutine(UpDateEXP());
    }

    public GameObject UIPrefab;
    private void OnDamage(float z, Skill x)
    {
        if(!actor.IsDefeat)
        anim[0].SetTrigger("Attacked");          
        else anim[0].SetTrigger("IsDeath");
        StartCoroutine(ColorBlink(Color.red,.1f));
        StartCoroutine(ShowDamage(z));

    }
    IEnumerator ShowDamage(float z)
    {
        float x = 0;
        var b = Instantiate(UIPrefab, cv.transform ).GetComponent<Text>();

        b.transform.position += Vector3.down;
        
        if (z > 0) b.color = Color.red;
        else if (z < 0) b.color = Color.green;
        else b.color = Color.white;
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
     
        InitializedActor(a, (RuntimeAnimatorController)Resources.Load<RuntimeAnimatorController>(path) );
  
    }


    float timer = 0;float timeSinceTurn = 2;float TimeSincedAttack = 0;
    private void FixedUpdate()
    {
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

    public void OnEnterTile()
    {
        transform.position = (Vector2)GameManager.Battlefied[(int)actor.TilePosition.x, (int)actor.TilePosition.y].transform.position + offset;

    

        while (actor.Path.Count > 0)
        {

            if (timer < .14f) return;
            AITImer = 0;
            var target = GameManager.CurrentBattle.map.AtPos(actor.Path.Peek());



            if (target.Actor != null && target.Actor != actor )
            {
             
                actor.CantMove(actor.Path.Peek());


                //Since PathFinding Is dumb, AI skip turn on stuck
                if (isAI && MyTurn) EndTurn();
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
    public void Sprite()
    {
        var e = GameManager.Battlefied[(int)actor.TilePosition.x, (int)actor.TilePosition.y];

        var walking = DistanceToPos > 0f;

    
        foreach (var item in anim)
            item.SetBool("Walking", walking);
        anim[0].SetBool("Defend",actor.Defending);
        var g = transform.position.x - e.transform.position.x;

        for (int i = 0; i < sprity.Length; i++)
        {
            var item = sprity[i];
            if (InverseSprite)
            {
                if (g > 0) item.flipX = !true;
                if (g < 0) item.flipX = !false;
            }
            else
            {
                if (g > 0) item.flipX = true;
                if (g < 0) item.flipX = false;
            }


            if (i > 0)
            {
                item.sprite = sprity[0].sprite;
            }
        }
        Position = new Vector2(actor.TilePosition.x, actor.TilePosition.y);
        DistanceToPos = (Vector3.Distance(transform.position, e.transform.position + new Vector3(offset.x, offset.y)) - 90.9f) * 100;
        var x = 1;
        if (GameManager.InBattleMode && GameManager.CurrentBattle.BattleTime < 2) x = 5;
        this.transform.position = Vector3.Lerp(transform.position, (Vector2)e.transform.position + offset, Speed * x* Time.smoothDeltaTime / (DistanceToPos + .1f));
        if (DistanceToPos <= 0.0025f) OnEnterTile();


        sprity[0].sortingOrder = 2 + (int)actor.TilePosition.y;

        Indicator.gameObject.SetActive(GameManager.SelectedActor == actor);
        Indicator.transform.position = e.transform.position;

    }
    public void TurnSprite(bool x)
    {
        for (int i = 0; i < sprity.Length; i++)
        {
            var item = sprity[i];
            if (InverseSprite)
                x = !x;
           item.flipX = x;     
        }
    }
        private void Update()
    {

        if (actor == null) return;
        Sprite();
        bar.text = actor.Name;


        if (actor.SP > 0 && MyTurn && isAI && !attacking) AI();
        if (MyTurn && timeSinceTurn > 1 && TimeSincedAttack > 1) if (actor.SP <= 0) EndTurn();
         
    }


}
