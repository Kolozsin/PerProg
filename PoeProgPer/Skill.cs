using System;

namespace PoeProgPer
{
    public enum SkillType { Kezdo ,Fire, Cold, Totem, Electric, Chaos, Health, AttackSpeed, Elixir, Dexterity, Strength, Intelligence }

    public class Skill
    {
        public Skill( int fitness, int type)
        {
            //Name = name;
            Fitness = fitness;
            Type = type;
        }



        //public string Name { get; set; }

        public int Fitness { get; set; }

        private int type;

        public int Type
        {
            get { return type; }
            set {
                int k = Enum.GetNames(typeof(SkillType)).Length;
                if (value > k)
                {
                    type = k % value;
                }
                else
                {
                    type = value;
                }
            }
        }


        public override string ToString()
        {
            if (this.Fitness == 0)
                return ((SkillType)type).ToString();
            return  (SkillType)this.Type + " - " + this.Fitness;
        }


    }
}