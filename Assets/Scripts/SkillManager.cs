using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

static class SkillManager
{

    static void Start()
    {
       
        string[] arr = XDocument.Load(@"skills.xml").Descendants("NodeName")
                 .Select(element => element.Value).ToArray();
        
    }

    public static Skill[] GetSkillsByProfession(Actor actor)
    {
        Profession.ProfessionType professionType = actor.Class.type;
        XDocument xml = XDocument.Load(@"skills.xml");
        XNamespace df = xml.Root.Name.Namespace;
        var elements = from c in xml.Descendants(df + professionType.ToString())
                       select c;
        Skill[] skill = new Skill[5];
        for (int i = 0; i < skill.Length; i++) {
            skill[i] = new Skill() {
                Name = elements.Select(e => e.Attribute("id")).First().Value,
                ProfType = (Profession.ProfessionType)Enum.Parse(typeof(Profession.ProfessionType),(string)elements.Select(e => e.Attribute("profession")).First().Value),
                DmgType = (DamageType)Enum.Parse(typeof(DamageType), (string)elements.Select(e => e.Attribute("type")).First().Value),
                Reach = int.Parse(elements.Select(e => e.Attribute("reach")).First().Value),
                Damage =  float.Parse(elements.Select(e => e.Attribute("damage")).First().Value),
                BaseCritChance = float.Parse(elements.Select(e => e.Attribute("baseCritChance")).First().Value),
                Targets = (Skill.TargetType)Enum.Parse(typeof(Skill.TargetType), (string)elements.Select(e => e.Attribute("targets")).First().Value),
                MpCost = int.Parse(elements.Select(e => e.Attribute("mpcost")).First().Value),
                HpCost = int.Parse(elements.Select(e => e.Attribute("hpcost")).First().Value),
                SpCost = int.Parse(elements.Select(e => e.Attribute("spcost")).First().Value),
                Level = int.Parse(elements.Select(e => e.Attribute("level")).First().Value)



            };
        }
        return skill;
    }
}

