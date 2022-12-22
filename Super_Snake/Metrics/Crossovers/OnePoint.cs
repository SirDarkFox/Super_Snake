using Super_Snake.DNA;
using Super_Snake.Metrics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.Metrics.Crossovers
{
    public class OnePoint : BaseCrossover
    {
        public OnePoint()
        {
            ObjType = 1;
        }

        public override Matrix DoCrossover(Matrix matrix1, Matrix matrix2, Random random)
        {
            Matrix result = new Matrix(matrix1.Rows, matrix1.Columns, random);

            int randC = random.Next(result.Columns);

            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Columns; j++)
                {
                    if (j <= randC)
                    {
                        result[i, j] = matrix1[i, j];
                    }
                    else 
                    {
                        result[i, j] = matrix2[i, j];
                    }
                }
            }
            return result;
        }
    }
}
