using System;
using System.Collections.Generic;
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
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace rekensommen_voor_basisschool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   Random Rand = new Random();
        List<int> multipliers = new List<int>();
        List<SumInformation> sumCollection = new List<SumInformation>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Create_Assignment(object sender, RoutedEventArgs e)
        {
            Reset_All();
            if (maxP.Text == "" || int.Parse(maxP.Text) < 5 || amount.Text == "" || int.Parse(amount.Text) < 5)
            {
                Warning.Content = "beide velden moeten ingevuld en minstens vijf zijn!";
            }
            else
            {
                Enable_Fields();
                GeneratedSums(int.Parse(maxP.Text));
                Create_Sum_Items();
            }
        }

        private void Calc_Score(object sender, RoutedEventArgs e)
        {
            bool allFilled = Are_all_filled();
            if (allFilled)
            {
                Set_Labels(Answer_Check());
                Enable_Fields();
            }
        }

        public void GeneratedSums(int max)
        {
            List<string> comboCheck = new List<string>();
            int i = 0;
            int totalQuestions = Int32.Parse(amount.Text);
            if (totalQuestions > max*max)
            {
                totalQuestions = max * max;
                amount.Text = totalQuestions.ToString();
                Warning.Content = "maximum aantal unieke vragen bereikt \npas het hoogste getal aan om meer vragen te maken.";
            }
            while (i<totalQuestions)
            {
                int a = Rand.Next(1, (max+1));
                multipliers.Add(a);
                int b = Rand.Next(1, (max + 1));
                string Calc = a.ToString() + " x " + b.ToString();
                if (!comboCheck.Contains(Calc))
                {
                    comboCheck.Add(Calc);
                    sumCollection.Add(new SumInformation() {sum = Calc, correctAns =a*b });
                    i += 1;
                }
            }
        }        

        private void Create_Sum_Items()
        {
            int i = 0;
            foreach (SumInformation item in sumCollection)
            {
                Label label = new Label();
                Questions.Children.Add(label);
                label.Content = item.sum;
                TextBox textbox = new TextBox();
                textbox.PreviewTextInput += OnlyIntegersInput;
                CommandManager.AddPreviewExecutedHandler(textbox, NoPasting);
                textbox.Margin = new Thickness(0, 5, 0, 3);
                Answers.Children.Add(textbox);                
                Label check = new Label();
                Check.Children.Add(check);
                i += 1;
            }            
        }

        private bool Are_all_filled()
        {
            int i = 0;
            foreach (TextBox item in Answers.Children)
            {
                if (item.Text == "")
                {
                    foreach (SumInformation field in sumCollection)
                    {
                        field.answer = "";
                    }
                    Warning.Content = "vul eerst alle vragen in!";
                    return false;
                }
                else
                {
                    sumCollection[i].answer = item.Text;
                }
                i += 1;
            }
            return true;
        }

        private int Answer_Check()
        {
            int numberCorrect = 0;
            foreach (SumInformation item in sumCollection)
            {
                if (item.answer=="")
                {
                    return 0;
                }
                else
                {
                    if(item.correctAns == Int32.Parse(item.answer))
                    {
                        item.isCorrect = "goed";
                        numberCorrect += 1;
                    }
                    else
                    {
                        item.isCorrect = "fout";
                    }
                }             
            }
            return numberCorrect;
        }

        private void Set_Labels(int allCorrect)
        {
                int i = 0;
                foreach (Label item in Check.Children)
                {
                    item.Content = sumCollection[i].isCorrect;
                    i += 1;
                }

            score.Content = (allCorrect * 10 / Int16.Parse(amount.Text)).ToString();

        }

        private void Reset_All()
        {
            Questions.Children.Clear();
            Answers.Children.Clear();
            Check.Children.Clear();
            multipliers.Clear();
            sumCollection.Clear();
            score.Content = "";
            Warning.Content = "";
        }

        private void Enable_Fields()
        {
            if (maxP.IsEnabled)
            {
                maxP.IsEnabled = false;
                amount.IsEnabled = false;
                CalcScore.IsEnabled = true;
                assignmentCreator.IsEnabled = false;                
            }
            else
            {
                maxP.IsEnabled = true;
                amount.IsEnabled = true;
                CalcScore.IsEnabled = false;
                assignmentCreator.IsEnabled = true;
            }

        }

        private void NoPasting(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Paste || e.Command == ApplicationCommands.Cut)
            {
                e.Handled = true;
            }
        }
        
        private void OnlyIntegersInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
    public class SumInformation
    {
        public int correctAns { get; set; }
        public string set { get; set; }
        public string sum { get; set; }
        public string isCorrect { get; set; }
        public string answer { get; set; }
    }
}