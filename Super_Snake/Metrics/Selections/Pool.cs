using Super_Snake.DNA;
using Super_Snake.Metrics.Base;
using Super_Snake.SnakeCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.Metrics.Selections
{
    public class Pool : BaseSelection
    {
        public Pool()
        {
            ObjType = 2;
        }

        public override Snake Select(Snake[] snakes, double fitnessSum, Random random)
        {
            int index = 0;
            double r = random.NextDouble();

            while (r >= 0)
            {
                r -= snakes[index].NormalizedFitness;
                index++;
            }
            index--;

            return snakes[index];
        }
    }
}
