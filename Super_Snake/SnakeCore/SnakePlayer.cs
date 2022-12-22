using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Super_Snake.SnakeCore
{
    public class SnakePlayer
    {
        Canvas _gameField;
        Snake _previousSnake;
        public int Speed { get; set; }
        public bool IsProcessing { get; set; }


        public SnakePlayer(Canvas gameField)
        {
            _gameField = gameField;
            Speed = 65;
        }

        public async Task PlayLoop(List<Snake> snakes,int gameFieldSize, int stepSize)
        {
            
            int index = (snakes.Count() > 0) ? snakes.IndexOf(snakes.Last()) : 0;
            IsProcessing = true;

            while (true)
            {
                while (snakes.Count <= index)
                {
                    if (!IsProcessing)
                    {
                        Stop();
                        break;
                    }
                    await Task.Delay(1);
                }


                if (_previousSnake == null || _previousSnake != snakes[index])
                    _previousSnake = snakes[index];
                else if (_previousSnake == snakes[index])
                {
                    index++;
                    continue;
                }

                Snake chosenSnake = snakes[index].CloneForReplay(_gameField, gameFieldSize, stepSize);

                for (int i = 0; i < chosenSnake.Decisions.Count; i++)
                {
                    if (!IsProcessing)
                    {
                        Stop();
                        break;
                    }
                    chosenSnake.Move(chosenSnake.Decisions[i]);
                    await Task.Delay(Speed);
                    ((MainWindow)Application.Current.MainWindow).SetProperties();
                }
                _gameField.Children.Clear();
                index++;
            }
        }

        public async Task Play(Snake snake, int gameFieldSize, int stepSize)
        {
            IsProcessing = true;
            Snake chosenSnake = snake.CloneForReplay(_gameField, gameFieldSize, stepSize);
            for (int i = 0; i < chosenSnake.Decisions.Count; i++)
            {
                if (!IsProcessing)
                {
                    Stop();
                    break;
                }
                chosenSnake.Move(chosenSnake.Decisions[i]);
                await Task.Delay(Speed);
            }
            _gameField.Children.Clear();
            IsProcessing = false;
        }

        void Stop()
        {
            _gameField.Children.Clear();
        }
    }
}
