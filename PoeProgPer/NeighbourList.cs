﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeProgPer
{
    public static class Utillek
    {
        public static  Random rnd = new Random();

        public static void BackTrack(int kezdo,ref  List<int>[] Vertice,int Koltheto,List<int> E,ref List<int> Opt,ref List<Skill> Skillist, int Josagertek)
        {
            int i = 0;
            List<int> ee = new List<int>();
            foreach (var item in E)
            {
                ee.Add(item);
            }
            if (Koltheto != 0)
            {
                ee.Add(kezdo);
                Koltheto--;
                while (i < Vertice[kezdo].Count)
                {                 
                    if (!ee.Contains(Vertice[kezdo][i]))
                    {
                        BackTrack(Vertice[kezdo][i], ref Vertice, Koltheto, ee, ref Opt, ref Skillist,Josagertek);
                    }
                    i++;
                }
                }
          
            else
            {
                if (Josag(ee, ref Skillist) >= Josagertek)
                {
                    Opt = new List<int>();
                    foreach (var item in ee)
                    {
                        Opt.Add(item);
                    }
                }
            }
        }

        public static int Josag(List<int> e,ref List<Skill> SkillList)
        {
            int output = 0;
            foreach (var item in e)
            {
                output +=SkillList[item].Fitness;
            }
            return output;
        }

        public static void Hozzarendeles(List<Skill> Skillist, List<Skill> chosen)
        {
            chosen.Sort((x,y)=> x.Type.CompareTo(y.Type) );
            foreach (var item in Skillist)
            {
                item.Fitness = chosen[item.Type].Fitness;
            }
        }
    

    }


    public class GraphNeighbourList
    {
        int N;
        public List<int>[] Vertice;
        public int stPointCounter;

        public GraphNeighbourList(int CountOfGraph, int StartPointCounter)
        {

            stPointCounter = StartPointCounter;
            N = CountOfGraph;
            Vertice = new List<int>[N];
            for (int i = 0; i < N; i++)
            {
                Vertice[i] = new List<int>();
            }
            MakingOurGraph();
        }

        //this method will genereate the graph (adding edges)
        private void MakingOurGraph()
        {
            for (int i = 0; i < stPointCounter; i++)
            {
                AddEdge(i, Utillek.rnd.Next(stPointCounter, N));
            }
            for (int i = stPointCounter; i < N; i++)
            {
                if (i == (int)N / 2)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j)
                            AddEdge(i, j);
                    }
                }
                else
                {
                    for (int j = 0; j < 2; j++)
                    {
                        int seged = i;
                        while (seged == i)
                        {
                            seged = Utillek.rnd.Next(stPointCounter, N);
                        }
                        AddEdge(i, seged);
                    }
                    
                }
            }
        }

        //Add edges between the two param Vertice
        public void AddEdge(int from, int to)
        {
            if (!EdgeExists(from, to))
            {
                this.Vertice.ElementAt(from).Add(to);
                this.Vertice.ElementAt(to).Add(from);
            }
        }

        public List<int> NeighboursofAVertice(int n)
        {
            List<int> output = new List<int>();
            foreach (var item in this.Vertice[n])
            {
                output.Add(item);
            }
            return output;
        }

        //True if the edge exists
        bool EdgeExists(int from, int to)
        {
            if (this.Vertice.ElementAt(from) != null && this.Vertice.ElementAt(to) != null)
                if (this.Vertice.ElementAt(from).Contains(to) || this.Vertice.ElementAt(to).Contains(from))
                {
                    return true;
                }
            return false;
        }
    }
}
