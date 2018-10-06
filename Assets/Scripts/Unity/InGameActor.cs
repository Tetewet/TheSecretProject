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
    public float DistanceToPos;

  
  
    private void Awake()
    {
        //Debug
        
        actor = new Player(Name, new stat { AGI = 1, STR = 1, INT = 5, LUC = 5, WIS = 5, END = 1 });
        actor.Heal();
        actor.Class = new Profession(new stat { AGI =2});
        Indicator.color = ActorColor;
        this.name = actor.Name;
    }
    float timer = 0;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        bar.text = actor.Name;
        
    }
    public void OnEnterTile()
    {
        transform.position = (Vector2)GameManager.Battlefied[(int)actor.TilePosition.x, (int)actor.TilePosition.y].transform.position + offset;


    }
    public void Sprite()
    {
        var e = GameManager.Battlefied[(int)actor.TilePosition.x, (int)actor.TilePosition.y];

        var walking = DistanceToPos > 0f;
        foreach (var item in anim)
            item.SetBool("Walking", walking);
        var g = transform.position.x - e.transform.position.x;
        foreach (var item in sprity)
        {
            if (g > 0) item.flipX = true;
            if (g < 0) item.flipX = false;
        }

        Position = new Vector2(actor.TilePosition.x, actor.TilePosition.y);
        DistanceToPos = (Vector3.Distance(transform.position, e.transform.position + new Vector3(offset.x, offset.y)) - 90.9f) * 100;
        this.transform.position = Vector3.Lerp(transform.position, (Vector2)e.transform.position + offset, Speed * Time.smoothDeltaTime / (DistanceToPos + .1f));
        if (DistanceToPos < 0.09f) OnEnterTile();


        sprity[0].sortingOrder = 2 + (int)actor.TilePosition.y;

        Indicator.gameObject.SetActive(GameManager.SelectedActor == actor);
        Indicator.transform.position = e.transform.position;

    }
    private void Update()
    {
        Sprite();

    }
}
