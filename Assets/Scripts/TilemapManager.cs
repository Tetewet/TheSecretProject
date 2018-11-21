using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapManager  {


 
    public static void LoadEvents(Tilemap t)
    {

        Debug.Log("Loading the map....");
        Debug.Log("Map size: " + t.cellBounds.size);
       
        t.gameObject.SetActive(false);
        BoundsInt bounds = t.cellBounds;
        TileBase[] allTiles = t.GetTilesBlock(bounds);
         
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    var til = GameManager.Map.Tiles[x, y];
                 
                    if(tile.name.Contains("F_B"))
                        til.collider = Map.Tile.ColliderType.All;
                    if (tile.name.Contains("U_B"))
                        til.collider = Map.Tile.ColliderType.Up;
                    if (tile.name.Contains("D_B"))
                        til.collider = Map.Tile.ColliderType.Down;
                    if (tile.name.Contains("R_B"))
                        til.collider = Map.Tile.ColliderType.Right;
                    if (tile.name.Contains("L_B"))
                        til.collider = Map.Tile.ColliderType.Left;
                    if (tile.name.Contains("SPAWN"))
                        Overworld.SpawnPoints.Add(new Vector(x, y));


                    Debug.Log(til);
                    //   Debug.Log("x:" + x + " y:" + y + " Name:" + tile.name);

                }
                else
                {
                   // Debug.Log("x:" + x + " y:" + y + " tile: (null)");

                }
            }
        }





    }
}
