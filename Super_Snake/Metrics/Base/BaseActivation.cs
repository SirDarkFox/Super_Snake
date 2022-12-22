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
    [JsonConverter(typeof(ActivationConverter))]
    public abstract class BaseActivation
    {
        public int ObjType { get; set; }
        public abstract Matrix Activate(Matrix matrix, Random random);
    }
}
