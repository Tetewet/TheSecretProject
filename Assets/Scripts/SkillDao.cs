using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;

static class SkillDao
{
    public static Skill[] GetSkillsByActor(Actor actor)
    {
        List<Skill> skills = new List<Skill>();
        string conn = "URI=file:" + Application.dataPath + "/Databases/Skills.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * " + "FROM Skills " + "WHERE ProfessionType = " + (int)actor.Class.type;
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            Skill newSkill = new Skill(
                reader.GetString(0),//name
                reader.GetString(1), //description
                (Profession.ProfessionType)reader.GetInt32(2),//profession
                (DamageType)reader.GetInt32(3),//damage type
                reader.GetInt32(4),//reach
                reader.GetFloat(5),//damage amount
                reader.GetFloat(6), //base crit chance
                (Skill.TargetType)reader.GetInt32(7),// target
                reader.GetInt32(8),//hpCost
                reader.GetInt32(9),//mpCost
                reader.GetInt32(10), //spCost
                reader.GetInt32(11),//level
                reader.GetInt32(12),//element
                reader.GetInt32(13),//effects
                reader.GetInt32(11) < actor.GetLevel);//unlocked


            skills.Add(newSkill);

        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        ;

        return skills.ToArray();
    }
    public static Skill[] GetSkillsByProfession(Profession.ProfessionType profession)
    {
        List<Skill> skills = new List<Skill>();
        string conn = "URI=file:" + Application.dataPath + "/Databases/Skills.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * " + "FROM Skills " + "WHERE ProfessionType = " + (int)profession;
        Debug.Log("Profession DB : Class " + profession + " has being loaded! ");
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            //       for (int i = 0; i < 12; i++)
            // {
            //      Debug.Log(reader.GetDataTypeName(i));
            //
            //  }
           
            Skill newSkill = new Skill(
                reader.GetString(0),//name
                reader.GetString(1), //description
                profession,//profession
                (DamageType)reader.GetInt32(3),//damage type
                reader.GetInt32(4),//reach
                reader.GetFloat(5),//damage amount
                reader.GetFloat(6), //base crit chance
                (Skill.TargetType)reader.GetInt32(7),// target
                reader.GetInt32(8),//hpCost
                reader.GetInt32(9),//mpCost
                reader.GetInt32(10), //spCost
                reader.GetInt32(11),//level
                reader.GetInt32(12),//element
                reader.GetInt32(13),//effects
                reader.GetInt32(14));//area of effect range
            skills.Add(newSkill);

        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        ;

        return skills.ToArray();
    }

}

