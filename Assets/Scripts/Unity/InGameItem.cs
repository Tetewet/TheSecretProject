using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameItem : MonoBehaviour {
    public Item item;
    public Vector2 Offset;
    public Vector2 Position;
    public SpriteRenderer[] sprity;
    public void OnBeingGrabbed(Actor a)
    {
        this.gameObject.SetActive(false);
    }
    public void OnDisposed()
    {
        Destroy(this.gameObject);
    }
    private void Start()
    {
        var e = GameManager.LoadSprite(item.ResourcePath);
        foreach (var item in sprity) item.sprite = e;
        item.Ongrabbed += OnBeingGrabbed;
        item.onDispose += OnDisposed;

    }
    public void Update()
    { 
            
        if(item != null && GameManager.Battlefied[(int)Position.x, (int)Position.y])
        {
            Position = new Vector2(item.TilePosition.x, item.TilePosition.y);
            this.transform.position = GameManager.Battlefied[(int)Position.x, (int)Position.y].transform.position + new Vector3(Offset.x, Offset.y) ;
        }

    }
}
