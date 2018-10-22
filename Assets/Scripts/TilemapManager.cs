using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour {

    public Tilemap ForeGround, BackGround, Paralax;
    public TileBase tb;
 
    // Use this for initialization
    void Start () {

        ForeGround.SetTile(Vector3Int.zero, tb);
     
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
