using Super_Snake.DNA;
using Super_Snake.Metrics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.Metrics.Mutations
{
    public class Close : BaseMutation
    {
        public Close()
        {
            ObjType = 1;
        }

        public override void Mutate(Matrix matrix, double mutationRate, Random random)
        {
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    double rand = random.NextDouble();
                    if (rand < mutationRate)
                    {
                        matrix[i, j] += Gaussian.RandomGaussian() / 5;

                        if (matrix[i, j] > 1)
                        {
                            matrix[i, j] = 1;
                        }
                        if (matrix[i, j] < -1)
                        {
                            matrix[i, j] = -1;
                        }
                    }
                }
            }
        }
    }
}
