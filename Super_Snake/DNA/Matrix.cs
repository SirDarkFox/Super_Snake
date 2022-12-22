using Newtonsoft.Json;
using Super_Snake.Metrics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.DNA
{
    public class Matrix
    {
        [JsonProperty]
        Random _random;

        public int Rows { get; set; }
        public int Columns { get; set; }

        [JsonProperty]
        double[,] _data;

        public Matrix()
        {

        }

        public Matrix(int rows, int columns, Random random)
        {
            Rows = rows;
            Columns = columns;
            _data = new double[rows, columns];

            _random = random;
        }

        public Matrix(double[] array)
        {
            Rows = array.Length;
            Columns = 1;
            _data = new double[Rows, Columns];

            for (int i = 0; i < Rows; i++)
            {
                _data[i, 0] = array[i];
            }
        }

        public double this[int index1, int index2]
        {
            get => _data[index1, index2];
            set => _data[index1, index2] = value;
        }

        public void Randomize()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _data[i, j] = -1 + (1 + 1) * _random.NextDouble();
                }
            }
        }

        public Matrix Dot(Matrix secondMatrix)
        {
            Matrix result = new Matrix(Rows, secondMatrix.Columns, secondMatrix._random);

            if (Columns == secondMatrix.Rows)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < secondMatrix.Columns; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < Columns; k++)
                        {
                            sum += _data[i, k] * secondMatrix[k, j];
                        }
                        result[i, j] = sum;
                    }
                }
            }
            else
            {
                throw new Exception();
            }
            return result;
        }

        public Matrix AddBias()
        {
            Matrix result = new Matrix(Rows + 1, 1, _random);

            for (int i = 0; i < Rows; i++)
            {
                result[i, 0] = _data[i, 0];
            }
            result[Rows, 0] = 1;
            return result;
        }

        public double[] ToArray()
        {
            double[] arr = new double[Rows * Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    arr[j + i * Columns] = _data[i, j];
                }
            }
            return arr;
        }

        public Matrix Clone()
        {
            Matrix clone = new Matrix(Rows, Columns, _random);

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    clone._data[i, j] = _data[i, j];
                }
            }
            return clone;
        }
    }
}
