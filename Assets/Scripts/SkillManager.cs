using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;

static class SkillManager
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
            Skill newSkill = new Skill(reader.GetString(0),reader.GetString(1),(Profession.ProfessionType)reader.GetInt32(2),(DamageType)reader.GetInt32(3),reader.GetInt32(4),reader.GetFloat(5),reader.GetFloat(6),(Skill.TargetType)reader.GetInt32(7),reader.GetInt32(8),reader.GetInt32(9),reader.GetInt32(10),reader.GetInt32(11), reader.GetInt32(11) < actor.GetLevel);
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
        Debug.Log(profession);
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
    //       for (int i = 0; i < 12; i++)
      // {
        //      Debug.Log(reader.GetDataTypeName(i));
        //
        //  }

            Skill newSkill = new Skill(reader.GetString(0), reader.GetString(1), profession, (DamageType)reader.GetInt32(3), reader.GetInt32(4), reader.GetFloat(5), reader.GetFloat(6), (Skill.TargetType)reader.GetInt32(7),  reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10), reader.GetInt32(11));
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

