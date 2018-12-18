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

                    if (tile.name.Contains("F_B"))
                        til.collider = Map.Tile.ColliderType.All;
                    else
                    if (tile.name.Contains("U_B"))
                        til.collider = Map.Tile.ColliderType.Up;
                    else
                    if (tile.name.Contains("D_B"))
                        til.collider = Map.Tile.ColliderType.Down;
                    else
                    if (tile.name.Contains("R_B"))
                        til.collider = Map.Tile.ColliderType.Right;
                    else
                    if (tile.name.Contains("L_B"))
                        til.collider = Map.Tile.ColliderType.Left;
                    else
                    if (tile.name.Contains("SPAWN"))
                        Overworld.SpawnPoints.Add(new Vector(x, y));
                    else if (tile.name.Contains("ENM"))
                    {
                        var e = tile.name.Split('-')[1];
                        GameManager.AddEvent(new BattleEvent(new Vector(x, y), MonsterControllerFactory.SpawnMonsters(),float.Parse(e)/100));

                    }

                    Debug.Log(til);
                }
                else
                {
                   // Debug.Log("x:" + x + " y:" + y + " tile: (null)");

                }
            }
        }
    }
}
