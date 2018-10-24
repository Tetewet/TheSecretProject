﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



    public class Skill
    {
        public enum TargetType
        {
            Self = 0,
            AnAlly = 1,
            OneEnemy = 2,
            Enemy = 3,
            Anyone = 4
        }

        static Random SkillRandom = new Random();
        public string Name = "";
        public DamageType Type;
        public int Reach = 1;
        //What percentage of the stats it uses; 1 = 100%, .2 = 20% of STR or INT - A la pokemon
        public float Damage = 1;
        protected float BaseCritChance = 5;
        public TargetType Targets;


        //Requirement    
        public float MpCost = 0, HpCost = 0;
        public int SpCost = 0;
        public static Skill Base
        {

            get
            {
                var e = new Skill();
                e.Name = "Attack";
                e.SpCost = 2;
                e.Reach = 1;
                e.Type = DamageType.Physical;
                e.Damage = .5f;
                e.Targets = TargetType.OneEnemy;
                return e;
            }
        }
        public virtual void Activate(Actor Target, stat Stats = new stat(), Actor f = null)
        {

            var x = Damage;
            if (Type == DamageType.Magical) x *= Stats.INT;
            else if (Type == DamageType.Physical) x *= Stats.STR;

            if ((Stats.LUC * 2 + BaseCritChance) > SkillRandom.Next(0, 100)) Damage *= 1.50f;

            Target.TakeDamage(x, this, f);
        }
        public void Activate(Actor[] a, Actor f = null)
        {
            foreach (var item in a) { Activate(item); }
        }





    }
