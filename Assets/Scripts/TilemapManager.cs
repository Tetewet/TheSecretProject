using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour {

    public Tilemap Events ;


    private void Start()
    {
        LoadMap();
    }
    public void LoadMap()
    {

        print("Loading the map....");
        print("Map size: " + Events.size );

       
        
        foreach (var item in Events.GetTilesBlock(new BoundsInt(Vector3Int.zero,Events.size)))
        {

            if(item!= null)
            {
                print(item.name + " -- " + item.ToString());
 
            }
    
        }
      
    }
}
