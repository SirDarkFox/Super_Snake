using Super_Snake.DNA;
using Super_Snake.Metrics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.Metrics.Mutations
{
    public class Whole : BaseMutation
    {
        public Whole()
        {
            ObjType = 2;
        }

        public override void Mutate(Matrix matrix, double mutationRate, Random random)
        {
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    matrix[i, j] = random.NextDouble();
                }
            }
        }
    }
}
