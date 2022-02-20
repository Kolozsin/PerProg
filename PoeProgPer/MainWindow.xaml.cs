using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PoeProgPer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel vm;
        SkillTree sk;


        public MainWindow()
        {
            this.vm = ViewModel.GetViewModel();
            DataContext = vm;
            InitializeComponent();
            this.AvaileableListBox.ItemsSource = vm.Availeable;
            this.ChosenListBox.ItemsSource = vm.Chosen;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (AvaileableListBox.SelectedItem != null && this.FitnessText.Text != "")
            {
                Skill skill = vm.Availeable[this.AvaileableListBox.SelectedIndex];
                try
                {                   
                    vm.Availeable.Remove(skill);
                    skill.Fitness = int.Parse(this.FitnessText.Text);
                    vm.Chosen.Add(skill);
                }
                catch (Exception excep)
                {
                    MessageBox.Show("There was an exception : "+ excep.ToString());
                    if (!vm.Availeable.Contains(skill)){ 
                        vm.Availeable.Add(skill);
                        vm.Chosen.Remove(skill);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Select a Skill Type and make sure that there is a fitness associated with it");
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (ChosenListBox.SelectedItem != null)
            {
                try
                {
                    Skill skill = vm.Chosen[this.ChosenListBox.SelectedIndex];
                    vm.Chosen.Remove(skill);
                    skill.Fitness = 0;
                    vm.Availeable.Add(skill);
                }
                catch (Exception)
                {
                }
            }

        }

        private void MakeTheTree()
        {
            sk = new SkillTree(vm.GraphSize, 4);
            string msg = "";
            for (int i = 0; i < sk.TheGraph.Vertice.Length && i < 50; i++)
            {
                msg += i + 1 + "(" + sk.skillist.ElementAt(i).ToString() + "): ";
                foreach (var item in sk.TheGraph.Vertice[i])
                {
                    msg += item + 1 + "-";
                }
                msg += "\n";
            }
            this.Graph_Content.Text = msg;
            MessageBox.Show("Created the following Graph: \n" + msg);
        }

        private void init() {
            foreach (var item in vm.Availeable)
            {
                vm.Chosen.Add(item);
            }
            Utils.Hozzarendeles(sk.skillist, vm.Chosen.ToList());
            vm.E = new List<int>();
            vm.opt = new List<int>();
            vm.E1 = new List<int>();
            vm.opt1 = new List<int>();
            vm.sw = new Stopwatch();

            vm.szamlal = sk.TheGraph.stPointCounter;
            vm.optimals = new List<int>[vm.szamlal];

            for (int i = 0; i < vm.szamlal; i++)
            {
                vm.optimals[i] = new List<int>();
            }
            vm.skillPoints = int.Parse(this.SkillPoints.Text);
            vm.tasks = new Task[vm.szamlal];
        }

        private async void DoWork_Click(object sender, RoutedEventArgs e)
        {            
            init();
            vm.tasks = new Task[vm.szamlal];
            int j = 0;
            int josag = 0;
            vm.sw.Start();
            while ( j < vm.szamlal)
            {
                int l = j;
                vm.tasks[l] = (Task.Factory.StartNew(() => Utils.BackTrack(l, ref sk.TheGraph.Vertice, vm.skillPoints, vm.E, ref vm.optimals[l], ref sk.skillist, ref josag), TaskCreationOptions.LongRunning));
                j++;
            }
            await Task.WhenAll(vm.tasks);
            vm.sw.Stop();
            createResult();
           
           /*
            Task.WaitAll(vm.tasks);
            vm.sw.Stop();
            createResult();
            */
        }        

        private void DoWorkNo_Click(object sender, RoutedEventArgs e)
        {
            init();
            int josag = 0;
            vm.sw.Start();
            vm.tasks = new Task[1];
            for (int j = 0; j < vm.szamlal; j++)
            {
                int l = j;
                vm.tasks[0] = (Task.Factory.StartNew(() => Utils.BackTrack(l, ref sk.TheGraph.Vertice, vm.skillPoints, vm.E, ref vm.optimals[l], ref sk.skillist, ref josag), TaskCreationOptions.LongRunning));
                //Task.WaitAll(vm.tasks);
                Task.WaitAll(vm.tasks);
            }
            vm.sw.Stop();
            createResult();
        }

        private void CreateGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.GraphSize = int.Parse(this.GraphSizeText.Text);
                MakeTheTree();
            }
            catch (Exception)
            {
                MessageBox.Show("Please give a number as a graph size");
            }
        }

        private void createResult()
        {
            List<int> best = new List<int>();
            int totalbest = 0;
            foreach (var item in vm.optimals)
            {
                int k = Utils.Goodness(item, ref sk.skillist);
                if (k > totalbest)
                {
                    totalbest = k;
                    best = item;
                }
            }
            string kimenet = "";
            foreach (var item in best)
            {
                kimenet += item + 1 + " ";
            }
            MessageBox.Show(vm.sw.Elapsed.ToString() + "\n" + kimenet, totalbest.ToString());
            this.Last_run.Content = vm.sw.Elapsed.ToString() + "\n" + kimenet;
            vm.Availeable.Clear();
            foreach (var item in vm.Chosen)
            {
                item.Fitness = 0;
                vm.Availeable.Add(item);
            }
            vm.Chosen.Clear();
            sk.resetskillList();
        }

    }

    public class ViewModel
    {
        public static ViewModel vm;
        public  BindingList<Skill> Availeable;
        public BindingList<Skill> Chosen;
        public int GraphSize;
        public List<int> E;
        public List<int> opt;
        public List<int> E1;
        public List<int> opt1;
        public Stopwatch sw;
        public List<int>[] optimals;
        public Task[] tasks;
        public int szamlal;
        public int skillPoints;

    public ViewModel()
        {
            InitAvaileable();
        }

        private void InitAvaileable()
        {
            Availeable = new BindingList<Skill>();
            Chosen = new BindingList<Skill>();
            for (int i = 0; i < Enum.GetNames(typeof(SkillType)).Length; i++)
            {
                Skill sk = new Skill( 0, i);
                Availeable.Add(sk);
            }
        }

        public static ViewModel GetViewModel()
        {
            if (vm == null)
            {
                vm = new ViewModel();
            }
            return vm;
        }
    }
}
