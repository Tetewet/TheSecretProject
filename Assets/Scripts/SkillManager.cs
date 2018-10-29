using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

    static class SkillManager
    {
       
        private static List<List<Skill>> skills = new List<List<Skill>>(16);

        static void Start()
        {
            for (int i = 0; i < Enum.GetNames(typeof(ProfessionType)).Length; i++)
            {
                skills.Add(new List<Skill>(5));

            }
            string[] arr = XDocument.Load(@"skills.xml").Descendants("NodeName")
                     .Select(element => element.Value).ToArray();
            foreach (List<Skill> skillList in skills)
            {
                skillList.Sort((x, y) => x.Level.CompareTo(y.Level));
            }
         
            // foreach (List<Skill> skillList in skills)
            //{
            //  foreach (Skill skill in skillList)
            //{
            //  print(skill.Name + " " + skill.Level );
            //    }
        }
        return skill;
    }

