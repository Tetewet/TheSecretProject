using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTile : MonoBehaviour {
    public Vector2 Value;
    public Map.Tile tile;
    public Image[] Sprite; 
    public Image ItemSlot;

    public void FixedUpdate()
    {

      //  if(tile!=null)transform.localPosition = Vector3.up * tile.Heigth;
    }

}
