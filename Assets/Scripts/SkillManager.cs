using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class SkillManager
    {
        private List<Skill> skillsToOrganize = new List<Skill>();
        private List<List<Skill>> skills = new List<List<Skill>>(16);

        void Start()
        {
            for (int i = 0; i < Enum.GetNames(typeof(ProfessionType)).Length; i++)
            {
                skills.Add(new List<Skill>(5));

            }
            foreach (Skill skill in skillsToOrganize)
            {
                // Debug.Log((int)skill.Profession);

                skills[(int)skill.Profession].Add(skill);
            }
            foreach (List<Skill> skillList in skills)
            {
                skillList.Sort((x, y) => x.Level.CompareTo(y.Level));
            }
            skillsToOrganize = null;
            // foreach (List<Skill> skillList in skills)
            //{
            //  foreach (Skill skill in skillList)
            //{
            //  print(skill.Name + " " + skill.Level );
            //    }
        }
    }
}
