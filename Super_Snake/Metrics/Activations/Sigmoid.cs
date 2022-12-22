using Super_Snake.DNA;
using Super_Snake.Metrics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.Metrics.Activations
{
    public class Sigmoid : BaseActivation
    {
        public Sigmoid()
        {
            ObjType = 2;
        }

        public override Matrix Activate(Matrix matrix, Random random)
        {
            Matrix result = new Matrix(matrix.Rows, matrix.Columns, random);

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    result[i, j] = 1 / (1 + Math.Pow(Math.E, -matrix[i, j]));
                }
            }

            return result;
        }
    }
}
