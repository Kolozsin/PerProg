using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeProgPer
{
    public class SkillTree
    {
        public GraphNeighbourList TheGraph;
        public List<Skill> skillist;


        public SkillTree(int size, int starter)
        {
            TheGraph = new GraphNeighbourList(size, starter);
            this.skillist = new List<Skill>();
            for (int i = 0; i < starter; i++)
            {
                skillist.Add(new Skill(0, 0));
            }
            for (int i = starter; i < size; i++)
            {
                skillist.Add(new Skill(0, Utillek.rnd.Next(1, Enum.GetNames(typeof(SkillType)).Length)));
            }
        }
    }
}
