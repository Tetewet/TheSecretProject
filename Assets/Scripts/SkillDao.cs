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
                reader.GetInt32(14),//Area of Effect
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
            string name = reader.GetString((int)Language.languageCode).Split(':')[0];
            string desc = reader.GetString((int)Language.languageCode).Split(':')[1];
           

            Skill newSkill = new Skill(
                name,//name
                desc, //description
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



    public static Skill[] InsertValues()//Still WIP
    {
        List<Skill> skills = new List<Skill>();
        string conn = "URI=file:" + Application.dataPath + "/Databases/Skills.db"; //Path to database.
        using (SqliteConnection dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            using (SqliteCommand dbcmd = dbconn.CreateCommand())
            {
                string sqlQuery = "SELECT * " + "FROM Skills ";
               // Debug.Log("Profession DB : Class " + profession + " has being loaded! ");
                dbcmd.CommandText = sqlQuery;
                using (SqliteDataReader reader = dbcmd.ExecuteReader())
                {
                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            string col0 = reader.GetString(0);//name
                            string col1 = reader.GetString(1); //description
                            string sqlInsert = "Update  Skills Set Name= '" + col0 + "::' ,Description='" + col1 + "::'";
                            using (SqliteCommand command = new SqliteCommand(sqlInsert, dbconn))
                            {
                                command.ExecuteNonQuery();
                            }

                        }
                        reader.NextResult();
                    }
                    reader.Close();
                }
                dbcmd.Dispose();
            }
            dbconn.Close();
            
        }
        

        return skills.ToArray();
    }

}

