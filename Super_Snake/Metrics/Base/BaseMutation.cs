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
    [JsonConverter(typeof(MutationConverter))]
    public abstract class BaseMutation
    {
        public int ObjType { get; set; }
        public abstract void Mutate(Matrix matrix, double mutationRate, Random random);
    }
}
