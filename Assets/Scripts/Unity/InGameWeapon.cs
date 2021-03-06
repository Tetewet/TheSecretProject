﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InGameWeapon : MonoBehaviour {
  
    public static igm.Color ColorToIGM(Color c)
    {
        return new igm.Color(c.r, c.b, c.g, c.a);
    }
    public static Color IGMtoColor(igm.Color c)
    {
        return new Color(c.R, c.G, c.B, c.A);
    }
    //Even if there is more type of weapon, most of 'em are composed of 3 elements. The blade of the staff would be the jewel at the tip, for exemple.
    public SpriteRenderer Blade, Hilt, Enchant;
    public Weapon weapon;

 
    public static GameObject GenerateInGameWeapon(Weapon w)
    {
        var path =  "Sprites/Weapon/" + w.WeaponType.ToString() + "/";
        var s = Instantiate(Resources.Load<GameObject>(path + "PREFAB"),Vector3.zero,Quaternion.identity).GetComponent<InGameWeapon>();      
        s.Enchant.enabled = s.Hilt.enabled = s.Blade.enabled = false;
        var B = Resources.LoadAll<Sprite>(path + "Blades/" );
        var H = Resources.LoadAll<Sprite>( path + "Hilts/");
        var E = Resources.LoadAll<Sprite>(path + "Enchants/");
        s.weapon = w;
        if (!w.HasEquipementData)
        {
            //Random For now to test the system
            var b = UnityEngine.Random.Range(0, B.Length - 1);
            var h = UnityEngine.Random.Range(0, H.Length - 1);
            var e = UnityEngine.Random.Range(0, E.Length - 1);

            s.weapon.SaveSpriteData(new Weapon.EquipmentSpriteData()
            {
                Blade = new Weapon.EquipmentSpriteData.Component(ColorToIGM(Color.white), b),
                Hilt = new Weapon.EquipmentSpriteData.Component(ColorToIGM(Color.white), h),
                Enchant = new Weapon.EquipmentSpriteData.Component(ColorToIGM(Color.white), e)
            });
            print("Create new Equipement data for " + w.Name + ":\n " + s.weapon.GetSpriteData.ToString());
        }



        ///Add logic here to have sprites reflect by their attributs
     Action<SpriteRenderer, Sprite[]> setWeapon= (x,y) =>
        {
            x.sprite = y[w.GetSpriteData.Blade.SpriteIndex];
            x.enabled = true;
            x.color = IGMtoColor(w.GetSpriteData.Blade.Color);
        };


        if (B.Length > 0)
        {
            s.Blade.sprite = B[w.GetSpriteData.Blade.SpriteIndex];
            s.Blade.enabled = true;
            s.Blade.color = IGMtoColor(w.GetSpriteData.Blade.Color);
        };

        if (H.Length > 0)
        {
            s.Hilt.sprite = H[w.GetSpriteData.Hilt.SpriteIndex];
            s.Hilt.enabled = true;
            s.Hilt.color = IGMtoColor(w.GetSpriteData.Hilt.Color);
        };


        if (E.Length > 0)
        {
            s.Enchant.sprite = E[w.GetSpriteData.Hilt.SpriteIndex];
            s.Enchant.enabled = true;
            s.Enchant.color = IGMtoColor(w.GetSpriteData.Hilt.Color);
        };


        s.gameObject.transform.localScale = Vector3.one;
        return s.gameObject;
    }

}
