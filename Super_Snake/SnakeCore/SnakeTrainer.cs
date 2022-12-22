using Newtonsoft.Json;
using Super_Snake.DNA;
using Super_Snake.Metrics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Super_Snake.SnakeCore
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SnakeTrainer
    {
        double _fitnessSum = 0;

        [JsonProperty]
        public Random Random { get; set; }
        [JsonProperty]
        public double MutationRate { get; set; }

        [JsonProperty]
        public int INodes { get; set; }
        [JsonProperty]
        public int HNodes { get; set; }
        [JsonProperty]
        public int ONodes { get; set; }
        [JsonProperty]
        public int HLayers { get; set; }
        [JsonProperty]
        public bool Bias { get; set; }

        [JsonProperty]
        public BaseActivation Activation { get; set; }
        [JsonProperty]
        public BaseCrossover Crossover { get; set; }
        [JsonProperty]
        public BaseMutation Mutation { get; set; }
        [JsonProperty]
        public BaseSelection Selection { get; set; }

        [JsonProperty]
        public int GameFieldSize { get; set; }
        [JsonProperty]
        public int StepSize { get; set; }
        [JsonProperty]
        public int BaseSnakeLength { get; set; }

        [JsonProperty]
        public int BaseEnergy { get; set; }
        [JsonProperty]
        public int EnergyReward { get; set; }

        [JsonProperty]
        public bool EightDirections { get; set; }
        [JsonProperty]
        public bool UnlimitedVision { get; set; }
        [JsonProperty]
        public bool AppleAngle { get; set; }
        [JsonProperty]
        public bool HeadTaleDirections { get; set; }

        [JsonProperty]
        public int Generation { get; set; }
        [JsonProperty]
        public int PopulationSize { get; set; }
        public bool IsProcessing { get; set; }
        public bool  IsReady { get; set; }

        [JsonProperty]
        public int BestScore { get; set; }
        [JsonProperty]
        public Snake[] Snakes { get; set; }
        public Snake SnakeLegend { get; set; }
        public List<Snake> BestSnakes { get; set; }


        public SnakeTrainer(Random random, double mutationRate, int iNodes, int hNodes, int oNodes, int hLayers, bool bias,
            BaseActivation activation, BaseCrossover crossover, BaseMutation mutation, BaseSelection selection,
            int gameFieldSize, int stepSize, int baseSnakeLength, int baseEnergy, int energyReward,
            bool eightDirections, bool unlimitedVision, bool appleAngle, bool headTaleDirections, int populationSize)
        {
            Random = random;
            MutationRate = mutationRate;

            INodes = iNodes;
            HNodes = hNodes;
            ONodes = oNodes;
            HLayers = hLayers;

            Bias = bias;

            Activation = activation;
            Crossover = crossover;
            Mutation = mutation;
            Selection = selection;

            GameFieldSize = gameFieldSize;
            StepSize = stepSize;
            BaseSnakeLength = baseSnakeLength;
            BaseEnergy = baseEnergy;
            EnergyReward = energyReward;
            EightDirections = eightDirections;
            UnlimitedVision = unlimitedVision;
            AppleAngle = appleAngle;
            HeadTaleDirections = headTaleDirections;
            PopulationSize = populationSize;
            Snakes = new Snake[PopulationSize];
            BestSnakes = new List<Snake>();

            IsReady = true;

            for (int i = 0; i < PopulationSize; i++)
            {
                NeuralNet brain = new NeuralNet(iNodes, hNodes, oNodes, hLayers, bias,
             activation, crossover, mutation, Random);

                Snake snake = new Snake(gameFieldSize, stepSize, baseSnakeLength, baseEnergy, energyReward, brain,
            eightDirections, unlimitedVision, appleAngle, headTaleDirections, Random);

                Snakes[i] = snake;
            }
        }

        public void Update()
        {
            foreach (var snake in Snakes)
            {
                snake.Brain.Update(Activation, Crossover, Mutation);
                snake.Update(GameFieldSize, StepSize, BaseSnakeLength, BaseEnergy, EnergyReward);
                snake.Initialize();
            }
        }

        public void TrainLoop()
        {

            IsProcessing = true;
            IsReady = false;
            while (true)
            {
                if (!IsProcessing)
                {
                    IsReady = true;
                    return;
                }
                while (!Done())
                {
                    for (int i = 0; i < PopulationSize; i++)
                    {
                        if (Snakes[i].IsAlive == true)
                        {
                            Snakes[i].Look();
                            Snakes[i].Think();
                            Snakes[i].Move(Snakes[i].Decisions.Last());
                        }
                    }
                }
                СalculateFitness();
                FindBestSnake();
                Evolution();
            }
        }

        public void Train()
        {
            IsProcessing = true;
            IsReady = false;
            while (!Done())
            {
                for (int i = 0; i < PopulationSize; i++)
                {
                    if (Snakes[i].IsAlive == true)
                    {
                        Snakes[i].Look();
                        Snakes[i].Think();
                        Snakes[i].Move(Snakes[i].Decisions.Last());
                    }
                }
            }
            СalculateFitness();
            FindBestSnake();
            Evolution();
            IsProcessing = false;
        }

        bool Done()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                if (Snakes[i].IsAlive)
                    return false;
            }

            Generation++;
            return true;
        }

        void Evolution()
        {
            Snake[] newSnakes = new Snake[PopulationSize];
            СalculateFitnessSum();
            CalculateNormalizedFitness();

            newSnakes[0] = SnakeLegend;
            for (int i = 1; i < PopulationSize; i++)
            {
                Snake child = SelectParent().Crossover(SelectParent());
                child.Mutate(MutationRate);
                newSnakes[i] = child;
            }

            Snakes = newSnakes;
        }

        Snake SelectParent() => Selection.Select(Snakes, _fitnessSum, Random);

        void СalculateFitness()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                Snakes[i].CalculateFitness();
            }
        }

        void СalculateFitnessSum()
        {
            _fitnessSum = 0;
            for (int i = 0; i < PopulationSize; i++)
            {
                _fitnessSum += Snakes[i].Fitness;
            }
        }

        void CalculateNormalizedFitness()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                Snakes[i].NormalizedFitness = Snakes[i].Fitness / _fitnessSum;
            }
        }

        void FindBestSnake()
        {
            BestSnakes.Add(Snakes.FirstOrDefault(x => x.Fitness == Snakes.Max(y => y.Fitness)));


            if (BestSnakes.Last().Score > BestScore)
                BestScore = (int)BestSnakes.Last().Score;

            if (SnakeLegend == null || BestSnakes.Last().Fitness > SnakeLegend.Fitness)
                SnakeLegend = BestSnakes.Last();

        }

        void ResetSnakes()
        {
            foreach (var snake in Snakes)
            {
                snake.Apples.Clear();
                snake.Decisions.Clear();
                snake.Score = 0;
                snake.EnergySpent = 0;
                snake.Initialize();
            }
        }
    }
}
