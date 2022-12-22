using Newtonsoft.Json;
using Super_Snake.DNA;
using Super_Snake.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Super_Snake.SnakeCore
{
    public class Snake
    {
        [JsonProperty]
        Random _random;
        Canvas _gameField;
        SolidColorBrush _snakeBodyBrush = Brushes.White;
        SolidColorBrush _snakeHeadBrush = Brushes.PaleGreen;
        SolidColorBrush _appleBrush = Brushes.Green;

        [JsonProperty]
        bool _eightDirections;
        [JsonProperty]
        bool _unlimitedVision;
        [JsonProperty]
        bool _appleAngle;
        [JsonProperty]
        bool _headTaleDirections;

        [JsonProperty]
        int _decisionsIter = 0;
        [JsonProperty]
        int _applesIter = 0;
        [JsonProperty]
        bool _replay = false;

        [JsonProperty]
        int _baseEnergy;
        [JsonProperty]
        int _energyReward;

        [JsonProperty]
        int _gameFieldSize;
        [JsonProperty]
        int _stepSize;

        [JsonProperty]
        int _baseSnakeLength;
        [JsonProperty]
        int _snakeLength;

        public List<SnakePart> Body { get; set; }
        public List<Apple> Apples { get; set; }
        public List<Directions> Decisions { get; set; }
        public NeuralNet Brain { get; set; }
        public bool IsAlive { get; set; }

        public double Score { get; set; }
        public int Energy { get; set; }
        public double EnergySpent { get; set; }
        public double Fitness { get; set; }
        public double NormalizedFitness { get; set; }
        public (int, int) StartIndeces { get; set; }
        public double[] Vision { get; set; }

        public SnakePart Head => Body.Last();
        public Apple CurrentApple
        {
            get
            {
                if (_replay)
                    return Apples[_applesIter];

                return Apples.Last();
            }

            set
            {
                if (_replay)
                    Apples[_applesIter] = value;
            }
        }

        public Snake()
        {

        }

        public Snake(int gameFieldSize, int stepSize, int baseSnakeLength, int baseEnergy, int energyReward, NeuralNet brain,
            bool eightDirections, bool unlimitedVision, bool appleAngle, bool headTaleDirections, Random random)
        {
            _random = random;
            Brain = brain;
            _eightDirections = eightDirections;
            _unlimitedVision = unlimitedVision;
            _appleAngle = appleAngle;
            _headTaleDirections = headTaleDirections;

            Update(gameFieldSize, stepSize, baseSnakeLength, baseEnergy, energyReward);

            Body = new List<SnakePart>();
            Apples = new List<Apple>();
            Decisions = new List<Directions>();
            Initialize();
        }

        public Snake(Canvas gameField, int gameFieldSize, int stepSize, int baseSnakeLength, List<Apple> apples, List<Directions> decisions,
            bool eightDirections, bool unlimitedVision, bool appleAngle, bool headTaleDirections, (int, int) indeces)
        {
            _replay = true;
            _gameField = gameField;
            _gameFieldSize = gameFieldSize;
            _stepSize = stepSize;
            _baseSnakeLength = baseSnakeLength;
            StartIndeces = indeces;


            _eightDirections = eightDirections;
            _unlimitedVision = unlimitedVision;
            _appleAngle = appleAngle;
            _headTaleDirections = headTaleDirections;

            Body = new List<SnakePart>();
            Apples = apples;
            Decisions = decisions;

            Initialize();
            DrawSnake();
        }

        public void Update(int gameFieldSize, int stepSize, int baseSnakeLength, int baseEnergy, int energyReward)
        {
            _gameFieldSize = gameFieldSize;
            _stepSize = stepSize;
            _baseSnakeLength = baseSnakeLength;
            _baseEnergy = baseEnergy;
            _energyReward = energyReward;
        }

        public void Initialize()
        {
            IsAlive = true;
            Energy = _baseEnergy;
            _snakeLength = _baseSnakeLength;
            Body.Clear();

            if (!_replay)
            {
                StartIndeces = (_random.Next(_gameFieldSize / _stepSize), _random.Next(3, _gameFieldSize / _stepSize - 1));
            }

            for (int i = 0; i < _snakeLength; i++)
            {
                Body.Add(new SnakePart() { Position = new Point(_stepSize * StartIndeces.Item1, _stepSize * StartIndeces.Item2 + i * _stepSize) });
            }
            Head.IsHead = true;
            PlaceApple();
        }

        bool AppleCollide(Point position)
        {
            if (position.X == CurrentApple.Position.X &&
                position.Y == CurrentApple.Position.Y)
            {
                return true;
            }

            return false;
        }

        bool BodyCollide(Point position)
        {
            foreach (SnakePart snakeBodyPart in Body.Take(Body.Count - 1))
            {
                if ((position.X == snakeBodyPart.Position.X) &&
                    (position.Y == snakeBodyPart.Position.Y))
                {
                    return true;
                }
            }

            return false;
        }

        bool WallCollide(Point position)
        {
            if ((position.Y < 0) || (position.Y >= _gameFieldSize) ||
                (position.X < 0) || (position.X >= _gameFieldSize))
            {
                return true;
            }

            return false;
        }

        public void CollisionCheck()
        {
            if (!IsAlive)
                return;

            if (AppleCollide(Head.Position))
            {
                Eat();
            }

            else if (BodyCollide(Head.Position))
            {
                Die();
            }

            else if (WallCollide(Head.Position))
            {
                Die();
            }

            else if (Energy <= 0 && !_replay)
            {
                Die();
            }
        }

        public void Move(Directions direction)
        {
            EnergySpent++;
            Energy--;

            while (Body.Count >= _snakeLength)
            {
                if (_replay)
                {
                    _gameField.Children.Remove(Body[0].UiElement);
                }
                Body.RemoveAt(0);
            }

            if (_replay)
            {
                (Head.UiElement as Rectangle).Fill = Brushes.White;
            }

            Head.IsHead = false;

            double nextX = Head.Position.X;
            double nextY = Head.Position.Y;
            switch (direction)
            {
                case Directions.Left:
                    nextX -= _stepSize;
                    break;
                case Directions.Right:
                    nextX += _stepSize;
                    break;
                case Directions.Top:
                    nextY -= _stepSize;
                    break;
                case Directions.Bottom:
                    nextY += _stepSize;
                    break;
            }

            Body.Add(new SnakePart()
            {
                Position = new Point(nextX, nextY),
                IsHead = true
            });

            if (_replay)
            {
                DrawSnake();
            }
            CollisionCheck();
        }

        public void DrawSnake()
        {
            foreach (SnakePart snakePart in Body)
            {
                if (snakePart.UiElement == null)
                {
                    snakePart.UiElement = new Rectangle()
                    {
                        Width = _stepSize,
                        Height = _stepSize,
                        StrokeThickness = 0.5,
                        Stroke = Brushes.Transparent,
                        Fill = (snakePart.IsHead ? _snakeHeadBrush : _snakeBodyBrush)
                    };
                    _gameField.Children.Add(snakePart.UiElement);
                    Canvas.SetTop(snakePart.UiElement, snakePart.Position.Y);
                    Canvas.SetLeft(snakePart.UiElement, snakePart.Position.X);
                }
            }
        }

        public void PlaceApple()
        {
            if (!_replay)
            {
                Apples.Add(new Apple() { Position = GetNextApplePosition() });
            }
            else
            {
                CurrentApple = new Apple
                {
                    Position = CurrentApple.Position,
                    UiElement = new Rectangle()
                    {
                        Width = _stepSize,
                        Height = _stepSize,
                        Fill = _appleBrush
                    }
                };
                _gameField.Children.Add(CurrentApple.UiElement);
                Canvas.SetTop(CurrentApple.UiElement, CurrentApple.Position.Y);
                Canvas.SetLeft(CurrentApple.UiElement, CurrentApple.Position.X);
            }
        }

        private Point GetNextApplePosition()
        {
            int maxX = _gameFieldSize / _stepSize;
            int maxY = maxX;
            int foodX = _random.Next(0, maxX) * _stepSize;
            int foodY = _random.Next(0, maxY) * _stepSize;

            foreach (SnakePart snakePart in Body)
            {
                if ((snakePart.Position.X == foodX) && (snakePart.Position.Y == foodY))
                    return GetNextApplePosition();
            }

            return new Point(foodX, foodY);
        }

        private void Eat()
        {
            _snakeLength++;
            Score++;
            Energy += _energyReward;
            if (Energy > 500)
            {
                Energy = 500;
            }

            if (_replay)
            {
                _gameField.Children.Remove(CurrentApple.UiElement);
                _applesIter++;
            }

            PlaceApple();
        }

        private void Die()
        {
            IsAlive = false;
        }

        public Snake Clone()
        {
            Snake clone = new Snake(_gameFieldSize, _stepSize, _baseSnakeLength, _baseEnergy, _energyReward, Brain.Clone(),
                _eightDirections, _unlimitedVision, _appleAngle, _headTaleDirections, _random);
            clone.Brain = Brain.Clone();
            clone.Score = Score;
            clone.Fitness = Fitness;
            clone.NormalizedFitness = NormalizedFitness;
            clone.EnergySpent = EnergySpent;

            var cloneApples = new List<Apple>();
            var cloneDecisions = new List<Directions>();
            foreach (var apple in Apples)
            {
                cloneApples.Add(apple);
            }
            foreach (var decision in cloneDecisions)
            {
                cloneDecisions.Add(decision);
            }

            clone.Apples = cloneApples;
            clone.Decisions = cloneDecisions;

            return clone;
        }

        public Snake CloneForReplay(Canvas gameField, int gameFieldSize, int stepSize)
        {
            Snake replaySnake = new Snake(gameField, gameFieldSize, stepSize, _baseSnakeLength, Apples, Decisions,
                _eightDirections, _unlimitedVision, _appleAngle, _headTaleDirections, StartIndeces);

            return replaySnake;
        }

        public Snake Crossover(Snake parent)
        {
            NeuralNet childBrain = Brain.DoCrossover(parent.Brain);
            Snake child = new Snake(_gameFieldSize, _stepSize, _baseSnakeLength, _baseEnergy, _energyReward, childBrain,
                _eightDirections, _unlimitedVision, _appleAngle, _headTaleDirections, _random);
            return child;
        }

        public void Mutate(double mutationRate)
        {
            Brain.Mutate(mutationRate);
        }

        public void CalculateFitness()
        {
            Fitness = EnergySpent + (Math.Pow(2, Score) + Math.Pow(Score, 2.1) * 500) - (0.25 * Math.Pow(EnergySpent, 1.3) * Math.Pow(Score, 1.2));

            //if (Score < 10)
            //{
            //    Fitness = EnergySpent * EnergySpent * Math.Pow(2, Score);
            //}
            //else
            //{
            //    Fitness = EnergySpent * EnergySpent;
            //    Fitness *= Math.Pow(2, 10);
            //    Fitness *= (Score - 9);
            //}

            //Fitness = EnergySpent * 2 + Math.Pow(2, Score + 1);
        }

        public void Look()
        {
            int inputs = 12;
            if (_eightDirections)
                inputs += 12;
            if (_headTaleDirections)
                inputs += 8;
            if (_appleAngle)
                inputs++;

            int index = 0;

            Vision = new double[inputs];
            double[] temp = LookInDirection(-1, 0);
            Vision[index++] = temp[0];
            Vision[index++] = temp[1];
            Vision[index++] = temp[2];

            temp = LookInDirection(0, -1);
            Vision[index++] = temp[0];
            Vision[index++] = temp[1];
            Vision[index++] = temp[2];

            temp = LookInDirection(1, 0);
            Vision[index++] = temp[0];
            Vision[index++] = temp[1];
            Vision[index++] = temp[2];

            temp = LookInDirection(0, 1);
            Vision[index++] = temp[0];
            Vision[index++] = temp[1];
            Vision[index++] = temp[2];


            if (_eightDirections)
            {
                temp = LookInDirection(-1, -1);
                Vision[index++] = temp[0];
                Vision[index++] = temp[1];
                Vision[index++] = temp[2];
                temp = LookInDirection(1, -1);
                Vision[index++] = temp[0];
                Vision[index++] = temp[1];
                Vision[index++] = temp[2];
                temp = LookInDirection(1, 1);
                Vision[index++] = temp[0];
                Vision[index++] = temp[1];
                Vision[index++] = temp[2];
                temp = LookInDirection(-1, 1);
                Vision[index++] = temp[0];
                Vision[index++] = temp[1];
                Vision[index++] = temp[2];
            }

            if (_headTaleDirections)
            {
                temp = new double[]{ 0, 0, 0, 0};
                temp[(int)GetHeadDirection()] = 1;
                Vision[index++] = temp[0];
                Vision[index++] = temp[1];
                Vision[index++] = temp[2];
                Vision[index++] = temp[3];

                temp = new double[] { 0, 0, 0, 0 };
                temp[(int)GetTaleDirection()] = 1;
                Vision[index++] = temp[0];
                Vision[index++] = temp[1];
                Vision[index++] = temp[2];
                Vision[index++] = temp[3];
            }

            if(_appleAngle)
            {
                Vision[index++] = GetAppleAngle();
            }
        }

        public double[] LookInDirection(int iIndex, int jIndex)
        {
            double[] result = { 0, 0, 0 };
            double distance = 0;
            int i = 0;
            int j = 0;

            while (true)
            {
                distance++;
                i += iIndex * _stepSize;
                j += jIndex * _stepSize;

                if (AppleCollide(new Point(Head.Position.X + i, Head.Position.Y + j)))
                    result[0] = 1;

                if (BodyCollide(new Point(Head.Position.X + i, Head.Position.Y + j)))
                    result[1] = 1;

                if (WallCollide(new Point(Head.Position.X + i, Head.Position.Y + j)))
                {
                    result[2] = 1 / distance;
                    break;
                }

                if (!_unlimitedVision)
                {
                    break;
                }
            }

            return result;
        }



        double GetAppleAngle()
        {
            double x1 = Head.Position.X;
            double y1 = Head.Position.Y;
            double x2 = 0;
            double y2 = 0;

            switch (GetHeadDirection())
            {
                case Directions.Left:
                    x2 = x1 - 10;
                    y2 = y1;
                    break;
                case Directions.Top:
                    x2 = x1;
                    y2 = y1 - 10;
                    break;
                case Directions.Right:
                    x2 = x1 + 10;
                    y2 = y1;
                    break;
                case Directions.Bottom:
                    x2 = x1;
                    y2 = y1 + 10;
                    break;
                default:
                    break;
            }
            double x3 = x1;
            double y3 = y1;
            double x4 = CurrentApple.Position.X;
            double y4 = CurrentApple.Position.Y;

            double angle = Math.Atan2(y2 - y1, x2 - x1) - Math.Atan2(y4 - y3, x4 - x3);

            return Math.Sin(angle);
        }

        Directions GetHeadDirection()
        {
            if (Decisions.Count == 0)
            {
                if (Head.Position.X - _stepSize == Body[Body.Count - 2].Position.X &&
                    Head.Position.Y == Body[Body.Count - 2].Position.Y)
                {
                    return Directions.Right;
                }
                else if (Head.Position.X == Body[Body.Count - 2].Position.X &&
                    Head.Position.Y - _stepSize == Body[Body.Count - 2].Position.Y)
                {
                    return Directions.Bottom;
                }
                else if (Head.Position.X + _stepSize == Body[Body.Count - 2].Position.X &&
                    Head.Position.Y == Body[Body.Count - 2].Position.Y)
                {
                    return Directions.Left;
                }
                else
                {
                    return Directions.Top;
                }
            }
            else
            {
                return Decisions.Last();
            }
        }

        Directions GetTaleDirection()
        {
            if (Body[0].Position.X - _stepSize == Body[1].Position.X &&
                Body[0].Position.Y == Body[1].Position.Y)
            {
                return Directions.Right;
            }
            else if (Body[0].Position.X == Body[1].Position.X &&
                Body[0].Position.Y - _stepSize == Body[1].Position.Y)
            {
                return Directions.Bottom;
            }
            else if (Body[0].Position.X + _stepSize == Body[1].Position.X &&
                Body[0].Position.Y == Body[1].Position.Y)
            {
                return Directions.Left;
            }
            else
            {
                return Directions.Top;
            }
        }

        public void Think()
        {
            double[] result = Brain.Output(Vision);
            int maxIndex = 0;
            double max = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] > max)
                {
                    max = result[i];
                    maxIndex = i;
                }
            }

            Decisions.Add((Directions)maxIndex);
        }
    }
}
