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
        PlayerPrefs.SetString("lang", GameManager.language.ToLower());
        LoadLanguage(PlayerPrefs.GetString("lang"));
        PlayerPrefs.Save();
        //var languageTranslation = LanguageDao.GetLanguage(languageCode.ToString().ToLower());
    }

    private static void LoadLanguage(string lang)
    {
        Text[] bigTranslator = GameManager.GM.TextAndUI.GetComponentsInChildren<Text>();
        Debug.Log(GameManager.GM);
        Debug.Log(GameManager.GM.TextAndUI);
      //  Debug.Log(bigTranslator[0]);
        for (int i = 0; i < bigTranslator.Length; i++) 
        {
            bigTranslator[i].text = LanguageDao.GetLanguage(bigTranslator[i].name, ref lang);

        }

    }
}
//FindObjectOfAll<Text>()
//get all the UI stuff here, and change it here (much simpler).
