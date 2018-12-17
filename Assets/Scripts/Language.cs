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

    private void Awake()
    {
        LanguageDao.DatabasePath = "URI=file:" + Application.dataPath + "/Databases/Languages.db";
        //var languageTranslation = LanguageDao.GetLanguage(languageCode.ToString().ToLower());
    }


}
//FindObjectOfAll<Text>()
//get all the UI stuff here, and change it here (much simpler).
