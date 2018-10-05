using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class InGameActor : MonoBehaviour {

    public string Name;
    public Color ActorColor;
    public SpriteRenderer Indicator;
    public Vector2 Position;
    public Actor actor;
    public Animator[] anim;
    public SpriteRenderer[] sprity;
    public float StepDuration = 0.1f;
    public float Speed = 5;
    public Vector2 offset;
    public void Move(Vector2 v){

        actor.Move(new Vector(v.x, v.y));
    }
    private void Awake()
    {
        //Debug
        actor = new Player("Nana", new stat { AGI = 1, STR = 1, INT = 5, LUC = 5, WIS = 5, END = 1 });
        actor.Class = new Profession(new stat { AGI =2});
        Indicator.color = ActorColor;
        this.Name = actor.ToString();
    }
    float timer = 0;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
       
        
    }
    private void Update()
    {
        
        actor.ConsumeSP(-10);
        Position = GameManager.Battlefied[(int)actor.transform.TilePosition.x, (int)actor.transform.TilePosition.y].transform.position;
        this.transform.position = Vector3.Lerp(transform.position, Position + offset, Speed * Time.smoothDeltaTime);


        //Debug

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        var walking = (Mathf.Abs(h) >= .3f || Mathf.Abs(v) >= .3f);
        foreach (var item in anim)
            item.SetBool("Walking", walking);
        if (timer>= StepDuration && walking)
        {

         

                var u = new Vector(Mathf.Clamp((int)(h)  ,-1,1), Mathf.Clamp(-(int)(v)  , -1,1));
          

                 foreach (var item in sprity)
                {

                if (h < 0) item.flipX = true;
                if (h > 0) item.flipX = false;
                }
        
            actor.Move(u);
            timer = 0;
        }
        Indicator.transform.position = Position;


    }
}
