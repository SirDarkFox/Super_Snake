using Newtonsoft.Json;
using Super_Snake.DNA;
using Super_Snake.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.Metrics.Base
{
    [JsonConverter(typeof(CrossoverConverter))]
    public abstract class BaseCrossover
    {
        public int ObjType { get; set; }
        public abstract Matrix DoCrossover(Matrix matrix1, Matrix matrix2, Random random);
    }
}
