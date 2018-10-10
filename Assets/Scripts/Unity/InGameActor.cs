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
    public float StepDuration = 0.1f;
    public float Speed = 5;
    public Vector2 offset;
    public bool InverseSprite = false;
    public float DistanceToPos;
    public bool IsFoe = false;

   public bool MyTurn = false;
   // private Skill SkillToUse = null;
  
    private void Awake()
    {
        //Debug
        if(!IsFoe)
       InitializedActor(new Player(Name, new stat { AGI = 2, STR = 6, INT = 5, LUC = 5, WIS = 5, END = 1 },IsFoe),"");
     else
            InitializedActor(new Monster(Name, new stat { AGI = 4, STR = 1, INT = 1, LUC = 1, WIS = 1, END = 1 }, IsFoe), "");


    }

    private void Start()
    {

        
    }
    bool attacking = false;

    //Action and Attack
    public void AI(Battle.Turn Turn = null)
    {
        Attack(GameManager.GM.Actors[0].actor, Skill.Base);
    }
    public void OnTurn(Battle.Turn Turn)
    {
        actor.TileWalkedThisTurn = 0;
        sprity[0].color = Color.white;
        MyTurn = true;
     
        if (!IsFoe)
        {
            GameManager.CursorPos = actor.TilePosition;
            GameManager.GM.OnPressed(actor.CurrentTile);
           
         
        }
        else AI(Turn);


    }
    public bool CanPerformAction(Skill s)
    {
        if (actor.HP <  s.HpCost) { print("Not enough HP: " + actor.HP + "/" +s.HpCost); return false; } 
        if (actor.MP <  s.MpCost) { print("Not enough MP: " + actor.MP + "/" + s.MpCost); return false; }
        if (actor.SP <  s.SpCost) { print("Not enough SP: " + actor.SP + "/" + s.SpCost); return false; }

        return true;
    }
    public void Attack(Actor a, Skill b)
    {
       if( MyTurn) StartCoroutine(InitiateAttack(a, b));       
    }
    /// <summary>
    /// Used by the Animation. Will Ends the Turn
    /// </summary>
   public void AnimatedAttack()
    {
        if (!MyTurn) return;
        Actor[] e = new Actor[1];
        e[0] = temptarget;
       
        actor.Use(tempattack, e);
        TimeSincedAttack = 0;
       if (actor.SP <= 0 ) EndTurn();
       
        
    }
    private void EndTurn( )
    {
        MyTurn = false;
        StartCoroutine(_EndTurn());
    }
    private IEnumerator _EndTurn()
    {
        yield return new WaitForSeconds(1);

        actor.Path.Clear();
     
        GameManager.CurrentBattle.EndTurn();
        GameManager.SelectedActor = null;
        sprity[0].color = Color.gray;
        timeSinceTurn = 0;
        print(actor.Name + " " + " ends his turn.");
        yield  break;
    }
    private Skill tempattack;
    Actor temptarget;
    public IEnumerator InitiateAttack(Actor a, Skill b)
    {
        if (!CanPerformAction(b) || attacking) yield break;
       
        if (!MyTurn) {
            attacking = false;
            yield break;
        }


        attacking = true;
        tempattack = b;
        temptarget = a;
        actor.Move(a.TilePosition, true);
        while (Vector.Distance(actor.TilePosition, a.TilePosition) > b.Reach)
        {
            yield return null;
        }
        TurnSprite((temptarget.TilePosition - actor.TilePosition).x < 0);
        yield return new WaitForSeconds(.1f);
        foreach (var item in anim) item.SetTrigger(b.Type.ToString());
        attacking = false;
        yield break;
    }


    //Actor Related - Can Swap Actor on a whim
    public void InitializedActor(Actor a, RuntimeAnimatorController b =null)
    {
        actor = a;
        a.OnTurn += OnTurn;
        a.OnDamage += OnDamage;
        Indicator.color = ActorColor;
        this.name = actor.Name;    
        if (b) foreach (var item in anim) item.runtimeAnimatorController = b;
        
        actor.Heal();
    }

    private void OnDamage(float z, Skill x)
    {
        anim[0].SetTrigger("Attacked");
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
     
        
    }

    public void OnEnterTile()
    {
        transform.position = (Vector2)GameManager.Battlefied[(int)actor.TilePosition.x, (int)actor.TilePosition.y].transform.position + offset;

    

        while (actor.Path.Count > 0)
        {

            if (timer < .14f) return;
            var target = GameManager.CurrentBattle.map.AtPos(actor.Path.Peek());



            if (target.Actor != null && target.Actor != actor )
            {
             
                actor.CantMove(actor.Path.Peek());
         
                    timer = 0;
                return;
            }
        
            if (actor.SP > 0)
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
          

            if(i > 0)
            {
                item.sprite = sprity[0].sprite;
            }
        }
        Position = new Vector2(actor.TilePosition.x, actor.TilePosition.y);
        DistanceToPos = (Vector3.Distance(transform.position, e.transform.position + new Vector3(offset.x, offset.y)) - 90.9f) * 100;
        this.transform.position = Vector3.Lerp(transform.position, (Vector2)e.transform.position + offset, Speed * Time.smoothDeltaTime / (DistanceToPos + .1f));
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


        if (actor.SP > 0 && MyTurn && IsFoe && !attacking) AI();
        if (MyTurn && timeSinceTurn > 1 && TimeSincedAttack > 1) if (actor.SP <= 0) EndTurn();
         
    }
}
