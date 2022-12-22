using Super_Snake.Metrics.Activations;
using Super_Snake.Metrics.Base;
using Super_Snake.Metrics.Crossovers;
using Super_Snake.Metrics.Mutations;
using Super_Snake.Metrics.Selections;
using Super_Snake.SnakeCore;
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
using System.Windows.Shapes;

namespace Super_Snake
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        SnakeTrainer _trainer;
        int _inputNodes = 12;
        int _outputNodes = 4;
        int _gameFieldSize;
        int _stepSize;
        bool _isNew;

        public Settings(bool isNew, int gameFieldSize, int stepSize)
        {
            InitializeComponent();

            _gameFieldSize = gameFieldSize;
            _stepSize = stepSize;

            _isNew = isNew;

            ActivationType.Items.Add("ReLU");
            ActivationType.Items.Add("Sigmoid");

            CrossoverType.Items.Add("OnePoint");
            CrossoverType.Items.Add("Uniform");
            CrossoverType.Items.Add("Special");

            MutationType.Items.Add("Close");
            MutationType.Items.Add("Whole");

            SelectionType.Items.Add("Pool");
            SelectionType.Items.Add("Custom");

            if (isNew)
            {
                SetDefault();
            }
            else
            {
                Alert1.Visibility = Visibility.Visible;
                Alert2.Visibility = Visibility.Visible;
                _trainer = ((MainWindow)Application.Current.MainWindow).Trainer;
                SetValues();
                Lock();
            }

            DrawNeuralNet();
        }
        private void LayersUp_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(LayersText.Content) >= 5)
                return;

            LayersText.Content = Convert.ToInt32(LayersText.Content) + 1;
            DrawNeuralNet();
        }
        private void LayersDown_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(LayersText.Content) <= 0)
                return;

            LayersText.Content = Convert.ToInt32(LayersText.Content) - 1;
            DrawNeuralNet();
        }
        private void NeuronsUp_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(NeuronsText.Content) >= 20)
                return;

            NeuronsText.Content = Convert.ToInt32(NeuronsText.Content) + 1;
            DrawNeuralNet();
        }
        private void NeuronsDown_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(NeuronsText.Content) <= 1)
                return;

            NeuronsText.Content = Convert.ToInt32(NeuronsText.Content) - 1;
            DrawNeuralNet();
        }

        private void EightDirections_Checked(object sender, RoutedEventArgs e)
        {
            _inputNodes += 12;
            DrawNeuralNet();
        }

        private void EightDirections_Unchecked(object sender, RoutedEventArgs e)
        {
            _inputNodes -= 12;
            DrawNeuralNet();
        }

        private void Angle_Checked(object sender, RoutedEventArgs e)
        {
            _inputNodes += 1;
            DrawNeuralNet();
        }

        private void Angle_Unchecked(object sender, RoutedEventArgs e)
        {
            _inputNodes -= 1;
            DrawNeuralNet();
        }

        private void HeadTale_Checked(object sender, RoutedEventArgs e)
        {
            _inputNodes += 8;
            DrawNeuralNet();
        }

        private void HeadTale_Unchecked(object sender, RoutedEventArgs e)
        {
            _inputNodes -= 8;
            DrawNeuralNet();
        }

        private void PopulationUp_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(PopulationLengthText.Content) >= 10000)
                return;

            PopulationLengthText.Content = Convert.ToInt32(PopulationLengthText.Content) + 100;
        }
        private void PopulationDown_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(PopulationLengthText.Content) <= 100)
                return;

            PopulationLengthText.Content = Convert.ToInt32(PopulationLengthText.Content) - 100;
        }
        private void EnergyUp_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(EnergyText.Content) >= 1000)
                return;

            EnergyText.Content = Convert.ToInt32(EnergyText.Content) + 5;
        }
        private void EnergyDown_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(EnergyText.Content) <= 50)
                return;

            EnergyText.Content = Convert.ToInt32(EnergyText.Content) - 5;
        }
        private void RewardUp_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(RewardText.Content) >= 500)
                return;

            RewardText.Content = Convert.ToInt32(RewardText.Content) + 5;
        }
        private void RewardDown_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(RewardText.Content) <= 25)
                return;

            RewardText.Content = Convert.ToInt32(RewardText.Content) - 5;
        }
        private void MutationUp_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDouble(MutationText.Content) >= 0.95)
                return;

            MutationText.Content = Math.Round(Convert.ToDouble(MutationText.Content) + 0.01, 2);
        }
        private void MutationDown_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDouble(MutationText.Content) <= 0)
                return;

            MutationText.Content = Math.Round(Convert.ToDouble(MutationText.Content) - 0.01, 2);
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (_isNew)
            {
                _trainer = new SnakeTrainer(
                    new Random(),
                    Convert.ToDouble(MutationText.Content),
                    _inputNodes,
                    Convert.ToInt32(NeuronsText.Content),
                    _outputNodes,
                    Convert.ToInt32(LayersText.Content),
                    BiasCheck.IsChecked ?? false,
                    (BaseActivation)MetricsConverter(ActivationType.Text),
                    (BaseCrossover)MetricsConverter(CrossoverType.Text),
                    (BaseMutation)MetricsConverter(MutationType.Text),
                    (BaseSelection)MetricsConverter(SelectionType.Text),
                    _gameFieldSize,
                    _stepSize,
                    3,
                    Convert.ToInt32(EnergyText.Content),
                    Convert.ToInt32(RewardText.Content),
                    EightDirectionsCheck.IsChecked ?? false,
                    UnlimitedVisionCheck.IsChecked ?? false,
                    AngleCheck.IsChecked ?? false,
                    headTaleCheck.IsChecked ?? false,
                    Convert.ToInt32(PopulationLengthText.Content)
                    );

                ((MainWindow)Application.Current.MainWindow).Trainer = _trainer;
                ((MainWindow)Application.Current.MainWindow).FileText.Text = "Новый файл";
            }

            else
            {
                _trainer.Activation = (BaseActivation)MetricsConverter(ActivationType.Text);
                _trainer.Crossover = (BaseCrossover)MetricsConverter(CrossoverType.Text);
                _trainer.Mutation = (BaseMutation)MetricsConverter(MutationType.Text);
                _trainer.Selection = (BaseSelection)MetricsConverter(SelectionType.Text);

                _trainer.PopulationSize = Convert.ToInt32(PopulationLengthText.Content);
                _trainer.BaseEnergy = Convert.ToInt32(EnergyText.Content);
                _trainer.EnergyReward = Convert.ToInt32(RewardText.Content);
                _trainer.MutationRate = Convert.ToDouble(MutationText.Content);

                _trainer.Update();
            }

            Close();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void SetDefault()
        {
            LayersText.Content = "2";
            NeuronsText.Content = "18";
            BiasCheck.IsChecked = true;

            EightDirectionsCheck.IsChecked = true;
            UnlimitedVisionCheck.IsChecked = true;
            AngleCheck.IsChecked = false;
            headTaleCheck.IsChecked = false;

            ActivationType.SelectedValue = "ReLU";
            CrossoverType.SelectedValue = "Special";
            MutationType.SelectedValue = "Close";
            SelectionType.SelectedValue = "Pool";

            PopulationLengthText.Content = "2000";
            EnergyText.Content = "200";
            RewardText.Content = "100";
            MutationText.Content = 0.05;
        }

        void SetValues()
        {
            LayersText.Content = _trainer.HLayers;
            NeuronsText.Content = _trainer.HNodes;
            BiasCheck.IsChecked = _trainer.Bias;

            EightDirectionsCheck.IsChecked = _trainer.EightDirections;
            UnlimitedVisionCheck.IsChecked = _trainer.UnlimitedVision;
            AngleCheck.IsChecked = _trainer.AppleAngle;
            headTaleCheck.IsChecked = _trainer.HeadTaleDirections;

            ActivationType.SelectedValue = _trainer.Activation.GetType().Name;
            CrossoverType.SelectedValue = _trainer.Crossover.GetType().Name;
            MutationType.SelectedValue = _trainer.Mutation.GetType().Name;
            SelectionType.SelectedValue = _trainer.Selection.GetType().Name;

            PopulationLengthText.Content = _trainer.PopulationSize;
            EnergyText.Content = _trainer.BaseEnergy;
            RewardText.Content = _trainer.EnergyReward;
            MutationText.Content = _trainer.MutationRate;
        }

        void Lock()
        {
            LayersUp.IsEnabled = false;
            LayersDown.IsEnabled = false;

            NeuronsUp.IsEnabled = false;
            NeuronsDown.IsEnabled = false;

            BiasCheck.IsEnabled = false;

            EightDirectionsCheck.IsEnabled = false;
            UnlimitedVisionCheck.IsEnabled = false;
            AngleCheck.IsEnabled = false;
        }

        object MetricsConverter(string metricName)
        {
            if (metricName == "ReLU")
                return new ReLU();

            if (metricName == "Sigmoid")
                return new Sigmoid();

            if (metricName == "OnePoint")
                return new OnePoint();

            if (metricName == "Uniform")
                return new Uniform();

            if (metricName == "Special")
                return new Special();

            if (metricName == "Close")
                return new Close();

            if (metricName == "Whole")
                return new Whole();

            if (metricName == "Custom")
                return new Custom();

            if (metricName == "Pool")
                return new Pool();

            return new object();
        }

        void DrawNeuralNet()
        {
            NeuralNetCanvas.Children.Clear();
            double size = (_inputNodes <= 25) ? 30 : 22;
            double horizontalInterval = 80;
            double verticalInterval = 0;
            NeuralNetCanvas.Height = (size + verticalInterval) * Math.Max(Convert.ToInt32(NeuronsText.Content), _inputNodes);
            NeuralNetCanvas.Width = (size + horizontalInterval) * (Convert.ToInt32(LayersText.Content) + 1) + size;

            double indentation = 0;

            if (_inputNodes < Convert.ToInt32(NeuronsText.Content))
                indentation = (Convert.ToInt32(NeuronsText.Content) - _inputNodes) / 2 * size;

            for (int i = 0; i < _inputNodes; i++)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = Brushes.Green;
                ellipse.StrokeThickness = 2;
                ellipse.Stroke = Brushes.White;

                ellipse.Height = size;
                ellipse.Width = size;

                ellipse.Margin = new Thickness { Top = (size + verticalInterval) * i + indentation };

                NeuralNetCanvas.Children.Add(ellipse);
            }

            indentation = 0;
            if (Convert.ToInt32(NeuronsText.Content) < _inputNodes)
                indentation = (_inputNodes - Convert.ToInt32(NeuronsText.Content)) / 2 * size;

            for (int i = 1; i < Convert.ToInt32(LayersText.Content) + 1; i++)
            {
                for (int j = 0; j < Convert.ToInt32(NeuronsText.Content); j++)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = Brushes.Red;
                    ellipse.StrokeThickness = 2;
                    ellipse.Stroke = Brushes.White;

                    ellipse.Height = size;
                    ellipse.Width = size;

                    ellipse.Margin = new Thickness { Top = (size + verticalInterval) * j + indentation, Left = (size + horizontalInterval) * i };

                    NeuralNetCanvas.Children.Add(ellipse);
                }
            }

            indentation = (Math.Max(_inputNodes, Convert.ToInt32(NeuronsText.Content)) - 4) / 2 * size;

            for (int i = 0; i < 4; i++)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = Brushes.Blue;
                ellipse.StrokeThickness = 2;
                ellipse.Stroke = Brushes.White;

                ellipse.Height = size;
                ellipse.Width = size;

                ellipse.Margin = new Thickness { Top = (size + verticalInterval) * i + indentation, Left = (size + horizontalInterval) * (Convert.ToInt32(LayersText.Content) + 1) };

                NeuralNetCanvas.Children.Add(ellipse);
            }

            int index1 = 0;
            int index2 = _inputNodes;
            int length1 = _inputNodes;
            int length2 = (Convert.ToInt32(LayersText.Content) > 0) ? Convert.ToInt32(NeuronsText.Content) : 4;

            for (int i = 0; i < Convert.ToInt32(LayersText.Content) + 1; i++)
            {
                if (i > 0)
                {
                    length1 = Convert.ToInt32(NeuronsText.Content);
                }

                if (i == Convert.ToInt32(LayersText.Content))
                {
                    length2 = 4;
                }

                int store = index2;

                for (int j = 0; j < length1; j++)
                {
                    index2 = store;
                    for (int k = 0; k < length2; k++)
                    {
                        Line line = new Line();

                        line.X1 = ((Ellipse)NeuralNetCanvas.Children[index1]).Margin.Left + size;
                        line.Y1 = ((Ellipse)NeuralNetCanvas.Children[index1]).Margin.Top + size / 2;
                        line.X2 = ((Ellipse)NeuralNetCanvas.Children[index2]).Margin.Left;
                        line.Y2 = ((Ellipse)NeuralNetCanvas.Children[index2]).Margin.Top + size / 2;
                        line.StrokeThickness = 0.8;
                        line.Stroke = Brushes.Green;

                        NeuralNetCanvas.Children.Add(line);

                        index2++;
                    }

                    index1++;
                }
            }
        }
    }
}
