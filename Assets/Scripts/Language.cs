using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum LanguageCode
{
    fr,
    en,
    es
}

public class Language
{
    [SerializeField] LanguageCode languageCode;
    private static Dictionary<string, string> languageDictionary = new Dictionary<string, string>();

    private void Awake()
    {
        LanguageDao.DatabasePath = "URI=file:" + Application.dataPath + "/Databases/Languages.db";
        //languageDictionary = LanguageDao.GetLanguage(languageCode.ToString().ToLower());
    }

    public static string Get(string name)
    {
        return languageDictionary[name];
    }
}
