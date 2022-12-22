using Microsoft.Win32;
using Newtonsoft.Json;
using Super_Snake.Metrics.Activations;
using Super_Snake.Metrics.Base;
using Super_Snake.Metrics.Crossovers;
using Super_Snake.Metrics.Mutations;
using Super_Snake.Metrics.Selections;
using Super_Snake.SnakeCore;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Super_Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _gameFieldSize = 800;
        int _stepSize = 20;
        SnakePlayer _player;


        public SnakeTrainer Trainer { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _player = new SnakePlayer(GameField);
            FileText.Text = "Не выбран";
            Trainer = new SnakeTrainer(
                    new Random(),
                    0.05,
                    12,
                    20,
                    4,
                    2,
                    true,
                    new ReLU(),
                    new Special(),
                    new Close(),
                    new Pool(),
                    _gameFieldSize,
                    _stepSize,
                    3,
                    200,
                    90,
                    false,
                    true,
                    false,
                    false,
                    500
                    );
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.T:
                    Test();
                    break;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (Trainer == null || !Trainer.IsReady)
            {
                return;
            }
            new Settings(true, _gameFieldSize, _stepSize).Show();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (Trainer == null || !Trainer.IsReady)
            {
                return;
            }
            OpenFileDialog load = new OpenFileDialog();
            load.Filter = "Open file(*.json)|*.json|Все файлы(*.*)|*.*";

            if (load.ShowDialog() == true)
            {
                Trainer = JsonConvert.DeserializeObject<SnakeTrainer>(File.ReadAllText(load.FileName));
                FileText.Text = load.FileName;
                SetProperties();
            }

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Trainer == null || !Trainer.IsReady)
            {
                return;
            }

            string ser = JsonConvert.SerializeObject(Trainer);
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Save file(*.json)|*.json|Все файлы(*.*)|*.*";

            if (save.ShowDialog() == true)
            {
                File.WriteAllText(save.FileName, ser);
                FileText.Text = save.FileName;
            }
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            if (Trainer == null || !Trainer.IsReady || _player.IsProcessing)
            {
                return;
            }

            CurrentScoreText.Content = "0";
            if (ReplayCheck.IsChecked == true)
            {
                if (Trainer.BestSnakes.Count == 0)
                    return;

                int index = Convert.ToInt32(Replays.Text.Split(':')[1]);
                await _player.Play(Trainer.BestSnakes[index],_gameFieldSize,_stepSize);
            }
            else 
            {
                new Task(() => Trainer.TrainLoop()).Start();
                //new Task(() => _player.PlayLoop(Trainer.BestSnakes, _gameFieldSize, _stepSize)).Start();

                _player.PlayLoop(Trainer.BestSnakes, _gameFieldSize, _stepSize);
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void Replay_Checked(object sender, RoutedEventArgs e)
        {
            Stop();
            Replays.IsEnabled = true;
            Replays.ItemsSource = Trainer.BestSnakes.Select((snake, index) => $"Поколение: {index}");
            Replays.SelectedIndex = 0;
        }

        private void Replay_Unchecked(object sender, RoutedEventArgs e)
        {
            Replays.IsEnabled = false;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (Trainer == null || !Trainer.IsReady || _player.IsProcessing)
            {
                return;
            }

            new Settings(false, _gameFieldSize, _stepSize).Show();
        }

        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            if (Trainer == null || Trainer.BestSnakes.Count == 0)
            {
                return;
            }
            var charts = new Charts(Trainer.BestSnakes.Select(x=>x.Fitness).ToArray());
            charts.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void SetProperties()
        {
            GenerationText.Content = Trainer.Generation;
            BestScoreText.Content = Trainer.BestScore;
        }

        void Stop()
        {
            if (Trainer == null)
            {
                return;
            }
            _player.IsProcessing = false;
            Trainer.IsProcessing = false;
        }

        void Test()
        {
            if (Trainer.Generation >= 3)
            {
                Stop();
                CurrentScoreText.Content = (Trainer.Snakes.Last().Brain == Trainer.Snakes[Trainer.Snakes.Length - 1].Brain);
            }
        }
    }
}
