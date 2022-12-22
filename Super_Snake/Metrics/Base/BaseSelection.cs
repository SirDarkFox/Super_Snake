using Newtonsoft.Json;
using Super_Snake.DNA;
using Super_Snake.SnakeCore;
using Super_Snake.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.Metrics.Base
{
    [JsonConverter(typeof(SelectionConverter))]
    public abstract class BaseSelection
    {
        public int ObjType { get; set; }
        public abstract Snake Select(Snake[] snakes, double fitnessSum, Random random); 
    }
}
