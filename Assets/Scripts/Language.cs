using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum LanguageCode
{
    fr,
    en,
    es
}

public class Language
{
    public static LanguageCode languageCode;

    public static void Initialize()
    {
        LanguageDao.DatabasePath = "URI=file:" + Application.dataPath + "/Databases/Languages.db";
        PlayerPrefs.SetString("lang", languageCode.ToString().ToLower());
        PlayerPrefs.Save();
        LoadLanguage(PlayerPrefs.GetString("lang"));
        //var languageTranslation = LanguageDao.GetLanguage(languageCode.ToString().ToLower());
    }

    private static void LoadLanguage(string lang)
    {
        Text[] bigTranslator = GameManager.GM.TextAndUI.GetComponentsInChildren<Text>();
        for (int i = 0; i < bigTranslator.Length; i++) 
        {
            bigTranslator[i].text = LanguageDao.GetLanguage(bigTranslator[i].name, lang);
        }
    }
}
//FindObjectOfAll<Text>()
//get all the UI stuff here, and change it here (much simpler).
