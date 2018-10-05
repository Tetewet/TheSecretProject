using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static bool InBattleMode = true;
    public static Vector3 VecTo3(Vector v)
    {
        return new Vector3(v.x, v.y, 0);
    }
    public static BattleTile[,] Battlefied;
    public GridLayoutGroup grid;
    public GameObject panel;
    public static Battle CurrentBattle;
 
    public string MapName;

    public InGameActor Madoshi;

    private void Awake()
    {
        CurrentBattle = new Battle();
        CurrentBattle.map = new Map(new Vector(15, 6));

        GenerateMap(CurrentBattle.map);
       
    }
    public void Start()
    {
        Madoshi.actor.Move(CurrentBattle.map.AtPos(0, 0));
    }
    public void GenerateMap(Map t)
    {
       
        grid.constraintCount = t.Tiles.GetLength(0);
        Battlefied = new BattleTile[t.Tiles.GetLength(0), t.Tiles.GetLength(1)];
        for (int x = 0; x < t.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < t.Tiles.GetLength(1); y++)
            {
                var e = Instantiate(panel, grid.transform).GetComponent<BattleTile>();
                e.tile = t.Tiles[x, y];
                var v = t.Tiles[x, y].Position;
                Battlefied[x, y] = e;
                e.Value = new Vector2(v.x, v.y);
            }
      
        }
        MapName = t.ToString();
    }
   /* [System.Serializable]
    public class MapWrapper 
    {
        public Tile[,] Tiles;
        public MapWrapper(Map a)
        {

            Tiles = new Tile[(int)a.Tiles.GetLength(0), (int)a.Tiles.GetLength(1)];
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    Tiles[x, y].tile.Position = new Vector(x, y);
                }
            }
        }
        public Tile AtPos(Vector v)
        {
            Tile g = new Tile();
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {

                    int a = (int)v.x;
                    int b = (int)v.y;
                    if (v.y > y) b = y;
                    if (v.x > x) a = x;
                    g = Tiles[a, b];

                }
            }
            return g;
        }
        [System.Serializable]
        public struct Tile
        {

           public Map.Tile tile;


        }
    }
    */
    
}
