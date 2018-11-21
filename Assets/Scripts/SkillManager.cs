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

   

    public static Skill[] GetSkillsByProfession(Actor actor)
    {
    string conn = "URI=file:" + Application.dataPath + "/Database/Skills.db"; //Path to database.
    IDbConnection dbconn;
    dbconn = (IDbConnection)new SqliteConnection(conn);
    dbconn.Open(); //Open connection to the database.
    IDbCommand dbcmd = dbconn.CreateCommand();
    string sqlQuery = "SELECT value,name, randomSequence " + "FROM PlaceSequence";
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader();
   
    reader.Close();
    reader = null;
    dbcmd.Dispose();
    dbcmd = null;
    dbconn.Close();
    dbconn = null;
    Skill[] skills = new Skill[1];
        

        return skills;
    }
}

