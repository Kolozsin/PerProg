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
                try
                {
                    Skill skill = vm.Availeable[this.AvaileableListBox.SelectedIndex];
                    vm.Availeable.Remove(skill);
                    skill.Fitness = int.Parse(this.FitnessText.Text);
                    vm.Chosen.Add(skill);
                }
                catch (Exception)
                {
                }
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

        private void DoWork_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in vm.Availeable)
            {
                vm.Chosen.Add(item);
            }
            MakeTheTree();
            Utillek.Hozzarendeles(sk.skillist, vm.Chosen.ToList());
            List<int> E = new List<int>();
            List<int> opt = new List<int>();
            List<int> E1 = new List<int>();
            List<int> opt1 = new List<int>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int szamlal = sk.TheGraph.stPointCounter;
            List<int>[] optimals = new List<int>[szamlal];
            
            for (int i = 0; i < szamlal; i++)
            {
                optimals[i] = new List<int>();
            }
            
            Task[] tasks = new Task[szamlal];
            int j = 0;
            while( j < szamlal)
            {
                int l = j;
                tasks[l] = (Task.Factory.StartNew(() => Utillek.BackTrack(l, ref sk.TheGraph.Vertice, 10, E, ref optimals[l], ref sk.skillist, 0), TaskCreationOptions.LongRunning));
                j++;
            }

            //a következő kódsorral lehet tesztelni, hogy egyébként mennyi idő alatt futna le párhuzamosítás nélkül 2 kezdőpont esetén a program
            //tasks[0] = (Task.Factory.StartNew(()=>Utillek.BackTrack(0, ref sk.TheGraph.Vertice, 15, E, ref optimals[0], ref sk.skillist,0), TaskCreationOptions.LongRunning));
            //tasks[1] = (Task.Factory.StartNew(() => Utillek.BackTrack(1, ref sk.TheGraph.Vertice, 15, E1, ref optimals[1], ref sk.skillist,0), TaskCreationOptions.LongRunning));


            Task.WaitAll(tasks);
            sw.Stop();
            List<int> best = new List<int>();
            int totalbest = 0;
            foreach (var item in optimals)
            {
                int k = Utillek.Josag(item, ref sk.skillist);
                if (k > totalbest)
                {
                    totalbest = k;
                    best = item;
                }
            }
            string kimenet = "";
            foreach (var item in best)
            {
                kimenet += item + " ";
            }
            MessageBox.Show(sw.Elapsed.ToString() + "\n" + kimenet,totalbest.ToString());
            
        }

        private void MakeTheTree()
        {
           
            sk = new SkillTree(20, 4);
            List<int> Megoldas = new List<int>();
            List<int> Optimalis = new List<int>();
        }

       
    }

    public class ViewModel
    {
        public static ViewModel vm;
       public  BindingList<Skill> Availeable;
        public BindingList<Skill> Chosen;

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
