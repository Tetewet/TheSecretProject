using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Mono.Data.Sqlite;

public static class LanguageDao
{
    public static string DatabasePath { get; set; }

    public static Dictionary<string, string> GetAll(string languageCode)
    {
        var languageDictionary = new Dictionary<string, string>();
        IDbConnection dbConnection = new SqliteConnection(DatabasePath);

        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT name, " + languageCode + " FROM language";
        IDataReader dataReader = dbCommand.ExecuteReader();

        while(dataReader.Read())
        {
            languageDictionary.Add(dataReader.GetString(0), dataReader.GetString(1));
        }

        dataReader.Close();
        dbConnection.Close();
        return languageDictionary;
    }
}
