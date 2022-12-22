using Super_Snake.Metrics.Base;
using Super_Snake.SnakeCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.Metrics.Selections
{
    public class Custom : BaseSelection
    {
        public Custom()
        {
            ObjType = 1;
        }

        public override Snake Select(Snake[] snakes, double fitnessSum, Random random)
        {
            int rand = random.Next((int)fitnessSum);
            double summation = 0;
            for (int i = 0; i < snakes.Length; i++)
            {
                summation += snakes[i].Fitness;
                if (summation > rand)
                {
                    return snakes[i];
                }
            }
            return snakes[0];
        }
    }
}
