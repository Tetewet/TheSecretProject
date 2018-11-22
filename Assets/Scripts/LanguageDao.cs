using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class LanguageDao
{
    public static string DatabasePath { get; set; }

    public static Dictionary<string, string> GetAll(string langCode)
    {
        //IDbConnection dbConnection = new SqliteConnection(DatabasePath);
        var languageDictionary = new Dictionary<string, string>();

        return languageDictionary;
    }
}
