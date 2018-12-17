using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Mono.Data.Sqlite;

public static class LanguageDao
{
    public static string DatabasePath { get; set; }

    /*
    //public static Dictionary<string, string> GetAll(string languageCode)
    //{

    //    var languageDictionary = new Dictionary<string, string>();
    //    IDbConnection dbConnection = new SqliteConnection(DatabasePath);

    //    dbConnection.Open();
    //    IDbCommand dbCommand = dbConnection.CreateCommand();
    //    dbCommand.CommandText = "SELECT name, " + languageCode + " FROM language"; //modifier la logique, pour pouvoir appeler la fonction de commande lors de l'appel de la fonction
    //    IDataReader dataReader = dbCommand.ExecuteReader();

    //    //il faut rajouter une autre methode, qui va retourner le commandtext, et lorsqu'on appelle cette methode, on lui affiche le commandTexte qu'il faut

    //    dataHandler(dataReader);
    //    dataReader =>
    //    {

    //        while (dataReader.Read())
    //        {
    //            languageDictionary.Add(dataReader.GetString(0), dataReader.GetString(1));
    //        }
    //    });

    //    dataReader.Close();
    //    dbConnection.Close();
    //    return languageDictionary;
    //}*/

    public static string GetLanguage(string name, string languageCode)
    {
        string languageTranslation;
        IDbConnection dbConnection = new SqliteConnection(DatabasePath);
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT " + languageCode + " FROM language WHERE name = " + name + ";";
        IDataReader dbReader = dbCommand.ExecuteReader();

        languageTranslation = dbReader.GetString(1);

        dbReader.Close();
        dbConnection.Close();
        return languageTranslation;
    }

    public static string ReadCommand(string enterCommand)
    {
        return enterCommand;
    }
}
